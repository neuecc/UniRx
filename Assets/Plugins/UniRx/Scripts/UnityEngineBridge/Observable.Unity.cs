#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define SupportCustomYieldInstruction
#endif

using System;
using System.Collections;
using System.Collections.Generic;
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

    public enum MainThreadDispatchType
    {
        /// <summary>yield return null</summary>
        Update,
        FixedUpdate,
        EndOfFrame,
        GameObjectUpdate,
        LateUpdate,
#if SupportCustomYieldInstruction
        /// <summary>Same as Update</summary>
        [Obsolete]
        AfterUpdate
#endif
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

    public class ObservableYieldInstruction<T> : IEnumerator<T>
    {
        readonly IDisposable subscription;
        readonly bool reThrowOnError;
        readonly CancellationToken cancel;
        T current;
        T result;
        bool moveNext;
        bool hasResult;
        Exception error;

        public ObservableYieldInstruction(IObservable<T> source, bool reThrowOnError, CancellationToken cancel)
        {
            this.moveNext = true;
            this.reThrowOnError = reThrowOnError;
            this.cancel = cancel;
            try
            {
                this.subscription = source.Subscribe(new ToYieldInstruction(this));
            }
            catch
            {
                moveNext = false;
                throw;
            }
        }

        public bool HasError
        {
            get { return error != null; }
        }

        public bool HasResult
        {
            get { return hasResult; }
        }

        public bool IsCanceled
        {
            get
            {
                if (hasResult) return false;
                if (error != null) return false;
                return cancel.IsCancellationRequested;
            }
        }

        public T Result
        {
            get { return result; }
        }

        T IEnumerator<T>.Current
        {
            get
            {
                return current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return current;
            }
        }

        public Exception Error
        {
            get
            {
                return error;
            }
        }

        bool IEnumerator.MoveNext()
        {
            if (!moveNext) return false;

            if (cancel.IsCancellationRequested)
            {
                subscription.Dispose();
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        class ToYieldInstruction : IObserver<T>
        {
            readonly ObservableYieldInstruction<T> parent;

            public ToYieldInstruction(ObservableYieldInstruction<T> parent)
            {
                this.parent = parent;
            }

            public void OnNext(T value)
            {
                parent.current = value;
            }

            public void OnError(Exception error)
            {
                parent.moveNext = false;
                parent.error = error;
                if (parent.reThrowOnError)
                {
                    throw error;
                }
            }

            public void OnCompleted()
            {
                parent.moveNext = false;
                parent.hasResult = true;
                parent.result = parent.current;
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

#if SupportCustomYieldInstruction

        class EveryAfterUpdateInvoker : IEnumerator
        {
            long count = -1;
            readonly IObserver<long> observer;
            readonly CancellationToken cancellationToken;

            public EveryAfterUpdateInvoker(IObserver<long> observer, CancellationToken cancellationToken)
            {
                this.observer = observer;
                this.cancellationToken = cancellationToken;
            }

            public bool MoveNext()
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (count != -1) // ignore first/immediate invoke
                    {
                        observer.OnNext(count++);
                    }
                    else
                    {
                        count++;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public object Current
            {
                get
                {
                    return null;
                }
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

#endif



        /// <summary>From has no callback coroutine to IObservable. If publishEveryYield = true then publish OnNext every yield return else return once on enumeration completed.</summary>
        public static IObservable<Unit> FromCoroutine(Func<IEnumerator> coroutine, bool publishEveryYield = false)
        {
            return FromCoroutine<Unit>((observer, cancellationToken) => WrapEnumerator(coroutine(), observer, cancellationToken, publishEveryYield));
        }

        /// <summary>
        /// MicroCoroutine is lightweight, fast coroutine dispatcher.
        /// IEnumerator supports only yield return null.
        /// If publishEveryYield = true then publish OnNext every yield return else return once on enumeration completed.
        /// </summary>
        public static IObservable<Unit> FromMicroCoroutine(Func<IEnumerator> coroutine, bool publishEveryYield = false, FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromMicroCoroutine<Unit>((observer, cancellationToken) => WrapEnumerator(coroutine(), observer, cancellationToken, publishEveryYield), frameCountType);
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
#if SupportCustomYieldInstruction
                    else if (current is IEnumerator)
                    {
                        yield return current;
                    }
#endif
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
            return new UniRx.Operators.FromCoroutineObservable<T>(coroutine);
        }

        /// <summary>
        /// MicroCoroutine is lightweight, fast coroutine dispatcher.
        /// IEnumerator supports only yield return null.
        /// </summary>
        public static IObservable<T> FromMicroCoroutine<T>(Func<IObserver<T>, IEnumerator> coroutine, FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromMicroCoroutine<T>((observer, _) => coroutine(observer));
        }

        /// <summary>
        /// MicroCoroutine is lightweight, fast coroutine dispatcher.
        /// IEnumerator supports only yield return null.
        /// </summary>
        public static IObservable<T> FromMicroCoroutine<T>(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine, FrameCountType frameCountType = FrameCountType.Update)
        {
            return new UniRx.Operators.FromMicroCoroutineObservable<T>(coroutine, frameCountType);
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

        /// <summary>
        /// EveryUpdate calls coroutine's yield return null timing. It is after all Update and before LateUpdate.
        /// </summary>
        public static IObservable<long> EveryUpdate()
        {
            return FromMicroCoroutine<long>((observer, cancellationToken) => EveryCycleCore(observer, cancellationToken), FrameCountType.Update);
        }

        public static IObservable<long> EveryFixedUpdate()
        {
            return FromMicroCoroutine<long>((observer, cancellationToken) => EveryCycleCore(observer, cancellationToken), FrameCountType.FixedUpdate);
        }

        public static IObservable<long> EveryEndOfFrame()
        {
            return FromMicroCoroutine<long>((observer, cancellationToken) => EveryCycleCore(observer, cancellationToken), FrameCountType.EndOfFrame);
        }

        static IEnumerator EveryCycleCore(IObserver<long> observer, CancellationToken cancellationToken)
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

        /// <summary>
        /// EveryGameObjectUpdate calls from MainThreadDispatcher's Update.
        /// </summary>
        public static IObservable<long> EveryGameObjectUpdate()
        {
            return MainThreadDispatcher.UpdateAsObservable().Scan(-1L, (x, y) => x + 1);
        }

        /// <summary>
        /// EveryLateUpdate calls from MainThreadDispatcher's OnLateUpdate.
        /// </summary>
        public static IObservable<long> EveryLateUpdate()
        {
            return MainThreadDispatcher.LateUpdateAsObservable().Scan(-1L, (x, y) => x + 1);
        }

#if SupportCustomYieldInstruction

        /// <summary>
        /// [Obsolete]Same as EveryUpdate.
        /// </summary>
        [Obsolete]
        public static IObservable<long> EveryAfterUpdate()
        {
            return FromCoroutine<long>((observer, cancellationToken) => new EveryAfterUpdateInvoker(observer, cancellationToken));
        }

#endif

        #region Observable.Time Frame Extensions

        // Interval, Timer, Delay, Sample, Throttle, Timeout

        public static IObservable<Unit> NextFrame(FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromMicroCoroutine<Unit>((observer, cancellation) => NextFrameCore(observer, cancellation), frameCountType);
        }

        static IEnumerator NextFrameCore(IObserver<Unit> observer, CancellationToken cancellation)
        {
            yield return null;

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
            return FromMicroCoroutine<long>((observer, cancellation) => TimerFrameCore(observer, dueTimeFrameCount, cancellation), frameCountType);
        }

        public static IObservable<long> TimerFrame(int dueTimeFrameCount, int periodFrameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return FromMicroCoroutine<long>((observer, cancellation) => TimerFrameCore(observer, dueTimeFrameCount, periodFrameCount, cancellation), frameCountType);
        }

        static IEnumerator TimerFrameCore(IObserver<long> observer, int dueTimeFrameCount, CancellationToken cancel)
        {
            // normalize
            if (dueTimeFrameCount <= 0) dueTimeFrameCount = 0;

            var currentFrame = 0;
            
            // initial phase
            while (!cancel.IsCancellationRequested)
            {
                if (currentFrame++ == dueTimeFrameCount)
                {
                    observer.OnNext(0);
                    observer.OnCompleted();
                    break;
                }
                yield return null;
            }
        }

        static IEnumerator TimerFrameCore(IObserver<long> observer, int dueTimeFrameCount, int periodFrameCount, CancellationToken cancel)
        {
            // normalize
            if (dueTimeFrameCount <= 0) dueTimeFrameCount = 0;
            if (periodFrameCount <= 0) periodFrameCount = 1;

            var sendCount = 0L;
            var currentFrame = 0;

            // initial phase
            while (!cancel.IsCancellationRequested)
            {
                if (currentFrame++ == dueTimeFrameCount)
                {
                    observer.OnNext(sendCount++);
                    currentFrame = -1;
                    break;
                }
                yield return null;
            }

            // period phase
            while (!cancel.IsCancellationRequested)
            {
                if (++currentFrame == periodFrameCount)
                {
                    observer.OnNext(sendCount++);
                    currentFrame = 0;
                }
                yield return null;
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

#if SupportCustomYieldInstruction

        /// <summary>
        /// Convert to yieldable IEnumerator. e.g. yield return source.ToYieldInstruction();.
        /// If needs last result, you can take ObservableYieldInstruction.HasResult/Result property.
        /// This overload throws exception if received OnError events(same as coroutine).
        /// </summary>
        public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this IObservable<T> source)
        {
            return new ObservableYieldInstruction<T>(source, true, CancellationToken.Empty);
        }

        /// <summary>
        /// Convert to yieldable IEnumerator. e.g. yield return source.ToYieldInstruction();.
        /// If needs last result, you can take ObservableYieldInstruction.HasResult/Result property.
        /// This overload throws exception if received OnError events(same as coroutine).
        /// </summary>
        public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this IObservable<T> source, CancellationToken cancel)
        {
            return new ObservableYieldInstruction<T>(source, true, cancel);
        }

        /// <summary>
        /// Convert to yieldable IEnumerator. e.g. yield return source.ToYieldInstruction();.
        /// If needs last result, you can take ObservableYieldInstruction.HasResult/Result property.
        /// If throwOnError = false, you can take ObservableYieldInstruction.HasError/Error property.
        /// </summary>
        public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this IObservable<T> source, bool throwOnError)
        {
            return new ObservableYieldInstruction<T>(source, throwOnError, CancellationToken.Empty);
        }

        /// <summary>
        /// Convert to yieldable IEnumerator. e.g. yield return source.ToYieldInstruction();.
        /// If needs last result, you can take ObservableYieldInstruction.HasResult/Result property.
        /// If throwOnError = false, you can take ObservableYieldInstruction.HasError/Error property.
        /// </summary>
        public static ObservableYieldInstruction<T> ToYieldInstruction<T>(this IObservable<T> source, bool throwOnError, CancellationToken cancel)
        {
            return new ObservableYieldInstruction<T>(source, throwOnError, cancel);
        }

#endif

        /// <summary>Convert to awaitable IEnumerator.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, CancellationToken cancel = default(CancellationToken))
        {
            return ToAwaitableEnumerator<T>(source, Stubs<T>.Ignore, Stubs.Throw, cancel);
        }

        /// <summary>Convert to awaitable IEnumerator.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, Action<T> onResult, CancellationToken cancel = default(CancellationToken))
        {
            return ToAwaitableEnumerator<T>(source, onResult, Stubs.Throw, cancel);
        }

        /// <summary>Convert to awaitable IEnumerator.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            return ToAwaitableEnumerator<T>(source, Stubs<T>.Ignore, onError, cancel);
        }

        /// <summary>Convert to awaitable IEnumerator.</summary>
        public static IEnumerator ToAwaitableEnumerator<T>(this IObservable<T> source, Action<T> onResult, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            var enumerator = new ObservableYieldInstruction<T>(source, false, cancel);
            var e = (IEnumerator<T>)enumerator;
            while (e.MoveNext() && !cancel.IsCancellationRequested)
            {
                yield return null;
            }

            if (cancel.IsCancellationRequested)
            {
                enumerator.Dispose();
                yield break;
            }

            if (enumerator.HasResult)
            {
                onResult(enumerator.Result);
            }
            else if (enumerator.HasError)
            {
                onError(enumerator.Error);
            }
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, CancellationToken cancel = default(CancellationToken))
        {
            return StartAsCoroutine<T>(source, Stubs<T>.Ignore, Stubs.Throw, cancel);
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, Action<T> onResult, CancellationToken cancel = default(CancellationToken))
        {
            return StartAsCoroutine<T>(source, onResult, Stubs.Throw, cancel);
        }

        /// <summary>AutoStart observable as coroutine.</summary>
        public static Coroutine StartAsCoroutine<T>(this IObservable<T> source, Action<Exception> onError, CancellationToken cancel = default(CancellationToken))
        {
            return StartAsCoroutine<T>(source, Stubs<T>.Ignore, onError, cancel);
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

        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source, MainThreadDispatchType dispatchType)
        {
            switch (dispatchType)
            {
                case MainThreadDispatchType.Update:
                    return source.ObserveOnMainThread(); // faster path

                // others, bit slower

                case MainThreadDispatchType.FixedUpdate:
                    return source.SelectMany(_ => EveryFixedUpdate().Take(1), (x, _) => x);
                case MainThreadDispatchType.EndOfFrame:
                    return source.SelectMany(_ => EveryEndOfFrame().Take(1), (x, _) => x);
                case MainThreadDispatchType.GameObjectUpdate:
                    return source.SelectMany(_ => MainThreadDispatcher.UpdateAsObservable().Take(1), (x, _) => x);
                case MainThreadDispatchType.LateUpdate:
                    return source.SelectMany(_ => MainThreadDispatcher.LateUpdateAsObservable().Take(1), (x, _) => x);
#if SupportCustomYieldInstruction
#pragma warning disable 612 // Type or member is obsolete
                case MainThreadDispatchType.AfterUpdate:
                    return source.SelectMany(_ => EveryAfterUpdate().Take(1), (x, _) => x);
#pragma warning restore 612 // Type or member is obsolete
#endif
                default:
                    throw new ArgumentException("type is invalid");
            }
        }

        public static IObservable<T> SubscribeOnMainThread<T>(this IObservable<T> source)
        {
            return source.SubscribeOn(SchedulerUnity.MainThread);
        }

        // I can't avoid Unity 5.3's uNET weaver bug, pending...

        //public static IObservable<T> SubscribeOnMainThread<T>(this IObservable<T> source, MainThreadDispatchType dispatchType)
        //{
        //    switch (dispatchType)
        //    {
        //        case MainThreadDispatchType.Update:
        //            return source.SubscribeOnMainThread(); // faster path

        //        // others, bit slower

        //        case MainThreadDispatchType.FixedUpdate:
        //            return new UniRx.Operators.SubscribeOnMainThreadObservable<T>(source, EveryFixedUpdate().Take(1));
        //        case MainThreadDispatchType.EndOfFrame:
        //            return new UniRx.Operators.SubscribeOnMainThreadObservable<T>(source, EveryEndOfFrame().Take(1));
        //        case MainThreadDispatchType.GameObjectUpdate:
        //            return new UniRx.Operators.SubscribeOnMainThreadObservable<T>(source, MainThreadDispatcher.UpdateAsObservable().Select(_ => 0L).Take(1));
        //        case MainThreadDispatchType.LateUpdate:
        //            return new UniRx.Operators.SubscribeOnMainThreadObservable<T>(source, MainThreadDispatcher.LateUpdateAsObservable().Select(_ => 0L).Take(1));
        //        case MainThreadDispatchType.AfterUpdate:
        //            return new UniRx.Operators.SubscribeOnMainThreadObservable<T>(source, EveryAfterUpdate().Take(1));
        //        default:
        //            throw new ArgumentException("type is invalid");
        //    }
        //}

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
}