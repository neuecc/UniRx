using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniRx.Triggers;
using UnityEngine;

#if !UniRxLibrary
using SchedulerUnity = UniRx.Scheduler;
#endif

namespace UniRx
{
    public enum FrameCountType
    {
        Update,
        FixedUpdate,
        EndOfFrame,
    }

    public static class FrameCountTypeExtensions
    {
        public static YieldInstruction GetYieldInstruction(this FrameCountType frameCountType)
        {
            switch (frameCountType)
            {
                case FrameCountType.FixedUpdate:
                    return YieldInstructionCache.WaitForFixedUpdate;
                case FrameCountType.EndOfFrame:
                    return YieldInstructionCache.WaitForEndOfFrame;
                case FrameCountType.Update:
                default:
                    return null;
            }
        }
    }

#if UniRxLibrary
    public static partial class ObservableUnity
#else
    public static partial class Observable
#endif
    {
        readonly static HashSet<Type> YieldInstructionTypes = new HashSet<Type>
        {
            typeof(WWW),
            typeof(WaitForEndOfFrame),
            typeof(WaitForFixedUpdate),
            typeof(WaitForSeconds),
            typeof(AsyncOperation),
            typeof(Coroutine)
        };

        /// <summary>From has no callback coroutine to IObservable. If publishEveryYield = true then publish OnNext every yield return else return once on enumeration completed.</summary>
        public static IObservable<Unit> FromCoroutine(Func<IEnumerator> coroutine, bool publishEveryYield = false)
        {
            return FromCoroutine<Unit>((observer, cancellationToken) => WrapEnumerator(coroutine(), observer, cancellationToken, publishEveryYield));
        }

        static IEnumerator WrapEnumerator(IEnumerator enumerator, IObserver<Unit> observer, CancellationToken cancellationToken, bool publishEveryYield)
        {
            var hasNext = default(bool);
            var raisedError = false;
            do
            {
                try
                {
                    hasNext = enumerator.MoveNext();
                }
                catch (Exception ex)
                {
                    try
                    {
                        raisedError = true;
                        observer.OnError(ex);
                    }
                    finally
                    {
                        var d = enumerator as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                        }
                    }
                    yield break;
                }
                if (hasNext && publishEveryYield)
                {
                    try
                    {
                        observer.OnNext(Unit.Default);
                    }
                    catch
                    {
                        var d = enumerator as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                        }
                        throw;
                    }
                }
                if (hasNext)
                {
                    yield return enumerator.Current; // yield inner YieldInstruction
                }
            } while (hasNext && !cancellationToken.IsCancellationRequested);

            try
            {
                if (!raisedError && !cancellationToken.IsCancellationRequested)
                {
                    observer.OnNext(Unit.Default); // last one
                    observer.OnCompleted();
                }
            }
            finally
            {
                var d = enumerator as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
            }
        }

        /// <summary>Convert coroutine to typed IObservable. If nullAsNextUpdate = true then yield return null when Enumerator.Current and no null publish observer.OnNext.</summary>
        public static IObservable<T> FromCoroutineValue<T>(Func<IEnumerator> coroutine, bool nullAsNextUpdate = true)
        {
            return FromCoroutine<T>((observer, cancellationToken) => WrapEnumeratorYieldValue<T>(coroutine(), observer, cancellationToken, nullAsNextUpdate));
        }

        static IEnumerator WrapEnumeratorYieldValue<T>(IEnumerator enumerator, IObserver<T> observer, CancellationToken cancellationToken, bool nullAsNextUpdate)
        {
            var hasNext = default(bool);
            var current = default(object);
            var raisedError = false;
            do
            {
                try
                {
                    hasNext = enumerator.MoveNext();
                    if (hasNext) current = enumerator.Current;
                }
                catch (Exception ex)
                {
                    try
                    {
                        raisedError = true;
                        observer.OnError(ex);
                    }
                    finally
                    {
                        var d = enumerator as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                        }
                    }
                    yield break;
                }

                if (hasNext)
                {
                    if (current != null && YieldInstructionTypes.Contains(current.GetType()))
                    {
                        yield return current;
                    }
                    else if (current == null && nullAsNextUpdate)
                    {
                        yield return null;
                    }
                    else
                    {
                        try
                        {
                            observer.OnNext((T)current);
                        }
                        catch
                        {
                            var d = enumerator as IDisposable;
                            if (d != null)
                            {
                                d.Dispose();
                            }
                            throw;
                        }
                    }
                }
            } while (hasNext && !cancellationToken.IsCancellationRequested);

            try
            {
                if (!raisedError && !cancellationToken.IsCancellationRequested)
                {
                    observer.OnCompleted();
                }
            }
            finally
            {
                var d = enumerator as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
            }
        }

        public static IObservable<T> FromCoroutine<T>(Func<IObserver<T>, IEnumerator> coroutine)
        {
            return FromCoroutine<T>((observer, _) => coroutine(observer));
        }

        public static IObservable<T> FromCoroutine<T>(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
        {
            return new UniRx.Operators.FromCoroutine<T>(coroutine);
        }

        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, IEnumerator coroutine, bool publishEveryYield = false)
        {
            return source.SelectMany(FromCoroutine(() => coroutine, publishEveryYield));
        }

        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, Func<IEnumerator> selector, bool publishEveryYield = false)
        {
            return source.SelectMany(FromCoroutine(() => selector(), publishEveryYield));
        }

        /// <summary>
        /// Note: publishEveryYield is always false. If you want to set true, use Observable.FromCoroutine(() => selector(x), true). This is workaround of Unity compiler's bug.
        /// </summary>
        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, Func<T, IEnumerator> selector)
        {
            return source.SelectMany(x => FromCoroutine(() => selector(x), false));
        }

        public static IObservable<Unit> ToObservable(this IEnumerator coroutine, bool publishEveryYield = false)
        {
            return FromCoroutine<Unit>((observer, cancellationToken) => WrapEnumerator(coroutine, observer, cancellationToken, publishEveryYield));
        }

        // variation of FromCoroutine

        public static IObservable<long> EveryUpdate()
        {
            return FromCoroutine<long>((observer, cancellationToken) => EveryUpdateCore(observer, cancellationToken));
        }

        static IEnumerator EveryUpdateCore(IObserver<long> observer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            var count = 0L;
            while (true)
            {
                yield return null;
                if (cancellationToken.IsCancellationRequested) yield break;

                observer.OnNext(count++);
            }
        }

        public static IObservable<long> EveryFixedUpdate()
        {
            return FromCoroutine<long>((observer, cancellationToken) => EveryFixedUpdateCore(observer, cancellationToken));
        }

        static IEnumerator EveryFixedUpdateCore(IObserver<long> observer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            var count = 0L;
            var yieldInstruction = YieldInstructionCache.WaitForFixedUpdate;
            while (true)
            {
                yield return yieldInstruction;
                if (cancellationToken.IsCancellationRequested) yield break;

                observer.OnNext(count++);
            }
        }

        public static IObservable<long> EveryEndOfFrame()
        {
            return FromCoroutine<long>((observer, cancellationToken) => EveryEndOfFrameCore(observer, cancellationToken));
        }

        static IEnumerator EveryEndOfFrameCore(IObserver<long> observer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            var count = 0L;
            var yieldInstruction = YieldInstructionCache.WaitForEndOfFrame;
            while (true)
            {
                yield return yieldInstruction;
                if (cancellationToken.IsCancellationRequested) yield break;

                observer.OnNext(count++);
            }
        }

        #region Observable.Time Frame Extensions

        // Interval, Timer, Delay, Sample, Throttle, Timeout

        public static IObservable<Unit> NextFrame(FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromCoroutine<Unit>((observer, cancellation) => NextFrameCore(observer, frameCountType, cancellation));
        }

        static IEnumerator NextFrameCore(IObserver<Unit> observer, FrameCountType frameCountType, CancellationToken cancellation)
        {
            yield return frameCountType.GetYieldInstruction();

            if (!cancellation.IsCancellationRequested)
            {
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
            }
        }

        public static IObservable<long> IntervalFrame(int intervalFrameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return TimerFrame(intervalFrameCount, intervalFrameCount, frameCountType);
        }

        public static IObservable<long> TimerFrame(int dueTimeFrameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromCoroutine<long>((observer, cancellation) => TimerFrameCore(observer, dueTimeFrameCount, frameCountType, cancellation));
        }

        public static IObservable<long> TimerFrame(int dueTimeFrameCount, int periodFrameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromCoroutine<long>((observer, cancellation) => TimerFrameCore(observer, dueTimeFrameCount, periodFrameCount, frameCountType, cancellation));
        }

        static IEnumerator TimerFrameCore(IObserver<long> observer, int dueTimeFrameCount, FrameCountType frameCountType, CancellationToken cancel)
        {
            // normalize
            if (dueTimeFrameCount <= 0) dueTimeFrameCount = 0;

            var currentFrame = 0;
            var yieldInstruction = frameCountType.GetYieldInstruction();
            // initial phase
            while (!cancel.IsCancellationRequested)
            {
                if (currentFrame++ == dueTimeFrameCount)
                {
                    observer.OnNext(0);
                    observer.OnCompleted();
                    break;
                }
                yield return yieldInstruction;
            }
        }

        static IEnumerator TimerFrameCore(IObserver<long> observer, int dueTimeFrameCount, int periodFrameCount, FrameCountType frameCountType, CancellationToken cancel)
        {
            // normalize
            if (dueTimeFrameCount <= 0) dueTimeFrameCount = 0;
            if (periodFrameCount <= 0) periodFrameCount = 1;

            var sendCount = 0L;
            var currentFrame = 0;
            var yieldInstruction = frameCountType.GetYieldInstruction();

            // initial phase
            while (!cancel.IsCancellationRequested)
            {
                if (currentFrame++ == dueTimeFrameCount)
                {
                    observer.OnNext(sendCount++);
                    currentFrame = -1;
                    break;
                }
                yield return yieldInstruction;
            }

            // period phase
            while (!cancel.IsCancellationRequested)
            {
                if (++currentFrame == periodFrameCount)
                {
                    observer.OnNext(sendCount++);
                    currentFrame = 0;
                }
                yield return yieldInstruction;
            }
        }

        public static IObservable<T> DelayFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");
            return new UniRx.Operators.DelayFrameObservable<T>(source, frameCount, frameCountType);
        }

        public static IObservable<T> SampleFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");
            return new UniRx.Operators.SampleFrameObservable<T>(source, frameCount, frameCountType);
        }

        public static IObservable<TSource> ThrottleFrame<TSource>(this IObservable<TSource> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");
            return new UniRx.Operators.ThrottleFrameObservable<TSource>(source, frameCount, frameCountType);
        }

        public static IObservable<TSource> ThrottleFirstFrame<TSource>(this IObservable<TSource> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");
            return new UniRx.Operators.ThrottleFirstFrameObservable<TSource>(source, frameCount, frameCountType);
        }

        public static IObservable<T> TimeoutFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");
            return new UniRx.Operators.TimeoutFrameObservable<T>(source, frameCount, frameCountType);
        }

        public static IObservable<T> DelayFrameSubscription<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");
            return new UniRx.Operators.DelayFrameSubscriptionObservable<T>(source, frameCount, frameCountType);
        }

        #endregion

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, CancellationToken cancel = default(CancellationToken))
        {
            return ToAwaitableEnumerator<T>(source, Stubs.Ignore<T>, Stubs.Throw, cancel);
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, Action<T> onResult, CancellationToken cancel = default(CancellationToken))
        {
            return ToAwaitableEnumerator<T>(source, onResult, Stubs.Throw, cancel);
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            return ToAwaitableEnumerator<T>(source, Stubs.Ignore<T>, onError, cancel);
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, Action<T> onResult, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            if (cancel == null) cancel = CancellationToken.Empty;
            var running = true;

            var subscription = source
                .LastOrDefault()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Finally(() =>
                {
                    running = false;
                })
                .Subscribe(t =>
                {
                    onResult(t);
                }, onError, () => { });

            while (running && !cancel.IsCancellationRequested)
            {
                yield return null;
            }

            if (cancel.IsCancellationRequested)
            {
                subscription.Dispose();
            }
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, CancellationToken cancel = default(CancellationToken))
        {
            return StartAsCoroutine<T>(source, Stubs.Ignore<T>, Stubs.Throw, cancel);
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, Action<T> onResult, CancellationToken cancel = default(CancellationToken))
        {
            return StartAsCoroutine<T>(source, onResult, Stubs.Throw, cancel);
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            return StartAsCoroutine<T>(source, Stubs.Ignore<T>, onError, cancel);
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, Action<T> onResult, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            return MainThreadDispatcher.StartCoroutine(source.ToAwaitableEnumerator(onResult, onError, cancel));
        }

        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source)
        {
            return source.ObserveOn(SchedulerUnity.MainThread);
        }

        public static IObservable<T> SubscribeOnMainThread<T>(this IObservable<T> source)
        {
            return source.SubscribeOn(SchedulerUnity.MainThread);
        }

        public static IObservable<bool> EveryApplicationPause()
        {
            return MainThreadDispatcher.OnApplicationPauseAsObservable().AsObservable();
        }

        public static IObservable<bool> EveryApplicationFocus()
        {
            return MainThreadDispatcher.OnApplicationFocusAsObservable().AsObservable();
        }

        /// <summary>publish OnNext(Unit) and OnCompleted() on application quit.</summary>
        public static IObservable<Unit> OnceApplicationQuit()
        {
            return MainThreadDispatcher.OnApplicationQuitAsObservable().Take(1);
        }

        public static IObservable<T> TakeUntilDestroy<T>(this IObservable<T> source, Component target)
        {
            return source.TakeUntil(target.OnDestroyAsObservable());
        }

        public static IObservable<T> TakeUntilDestroy<T>(this IObservable<T> source, GameObject target)
        {
            return source.TakeUntil(target.OnDestroyAsObservable());
        }

        public static IObservable<T> TakeUntilDisable<T>(this IObservable<T> source, Component target)
        {
            return source.TakeUntil(target.OnDisableAsObservable());
        }

        public static IObservable<T> TakeUntilDisable<T>(this IObservable<T> source, GameObject target)
        {
            return source.TakeUntil(target.OnDisableAsObservable());
        }

        public static IObservable<T> RepeatUntilDestroy<T>(this IObservable<T> source, GameObject target)
        {
            return RepeatUntilCore(RepeatInfinite(source), target.OnDestroyAsObservable(), target);
        }

        public static IObservable<T> RepeatUntilDestroy<T>(this IObservable<T> source, Component target)
        {
            return RepeatUntilCore(RepeatInfinite(source), target.OnDestroyAsObservable(), (target != null) ? target.gameObject : null);
        }

        public static IObservable<T> RepeatUntilDisable<T>(this IObservable<T> source, GameObject target)
        {
            return RepeatUntilCore(RepeatInfinite(source), target.OnDisableAsObservable(), target);
        }

        public static IObservable<T> RepeatUntilDisable<T>(this IObservable<T> source, Component target)
        {
            return RepeatUntilCore(RepeatInfinite(source), target.OnDisableAsObservable(), (target != null) ? target.gameObject : null);
        }

        static IObservable<T> RepeatUntilCore<T>(this IEnumerable<IObservable<T>> sources, IObservable<Unit> trigger, GameObject lifeTimeChecker)
        {
            return new UniRx.Operators.RepeatUntilObservable<T>(sources, trigger, lifeTimeChecker);
        }

#if UniRxLibrary

        static IEnumerable<IObservable<T>> RepeatInfinite<T>(IObservable<T> source)
        {
            while (true)
            {
                yield return source;
            }
        }
#endif
    }

#if UniRxLibrary
    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => { throw ex; };

        // Stubs<T>.Ignore can't avoid iOS AOT problem.
        public static void Ignore<T>(T t)
        {
        }

        // marker for CatchIgnore and Catch avoid iOS AOT problem.
        public static IObservable<TSource> CatchIgnore<TSource>(Exception ex)
        {
            return Observable.Empty<TSource>();
        }
    }
#endif
}
