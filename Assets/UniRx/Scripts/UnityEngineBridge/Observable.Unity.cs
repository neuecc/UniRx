using System;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
                    return new WaitForFixedUpdate();
                case FrameCountType.EndOfFrame:
                    return new WaitForEndOfFrame();
                case FrameCountType.Update:
                default:
                    return null;
            }
        }
    }

    public static partial class Observable
    {
        readonly static HashSet<Type> YieldInstructionTypes = new HashSet<Type>
        {
            typeof(WWW),
            typeof(WaitForEndOfFrame),
            typeof(WaitForFixedUpdate),
            typeof(WaitForSeconds),
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
            return Observable.FromCoroutine<T>((observer, cancellationToken) => WrapEnumeratorYieldValue<T>(coroutine(), observer, cancellationToken, nullAsNextUpdate));
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
            return Observable.Create<T>(observer =>
            {
                var cancel = new BooleanDisposable();

                MainThreadDispatcher.SendStartCoroutine(coroutine(observer, new CancellationToken(cancel)));

                return cancel;
            });
        }

        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, IEnumerator coroutine, bool publishEveryYield = false)
        {
            return source.SelectMany(Observable.FromCoroutine(() => coroutine, publishEveryYield));
        }

        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, Func<IEnumerator> selector, bool publishEveryYield = false)
        {
            return source.SelectMany(Observable.FromCoroutine(() => selector(), publishEveryYield));
        }

        /// <summary>
        /// Note: publishEveryYield is always false. If you want to set true, use Observable.FromCoroutine(() => selector(x), true). This is workaround of Unity compiler's bug.
        /// </summary>
        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, Func<T, IEnumerator> selector)
        {
            return source.SelectMany(x => Observable.FromCoroutine(() => selector(x), false));
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
            var count = 0L;
            while (!cancellationToken.IsCancellationRequested)
            {
                yield return null;
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
            do
            {
                yield return new UnityEngine.WaitForFixedUpdate();
                observer.OnNext(count++);
            } while (!cancellationToken.IsCancellationRequested);
        }

        public static IObservable<long> EveryEndOfFrame()
        {
            return FromCoroutine<long>((observer, cancellationToken) => EveryEndOfFrameCore(observer, cancellationToken));
        }

        static IEnumerator EveryEndOfFrameCore(IObserver<long> observer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            var count = 0L;
            do
            {
                yield return new UnityEngine.WaitForFixedUpdate();
                observer.OnNext(count++);
            } while (!cancellationToken.IsCancellationRequested);
        }

        #region Observable.Time Frame Extensions

        // Interval, Timer, Delay, Sample, Throttle, Timeout

        public static IObservable<Unit> NextFrame(FrameCountType frameCountType = FrameCountType.Update)
        {
            return Observable.FromCoroutine<Unit>((observer, cancellation) => NextFrameCore(observer, frameCountType, cancellation));
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
            return Observable.FromCoroutine<long>((observer, cancellation) => TimerFrameCore(observer, dueTimeFrameCount, frameCountType, cancellation));
        }

        public static IObservable<long> TimerFrame(int dueTimeFrameCount, int periodFrameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return Observable.FromCoroutine<long>((observer, cancellation) => TimerFrameCore(observer, dueTimeFrameCount, periodFrameCount, frameCountType, cancellation));
        }

        static IEnumerator TimerFrameCore(IObserver<long> observer, int dueTimeFrameCount, FrameCountType frameCountType, CancellationToken cancel)
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
                yield return frameCountType.GetYieldInstruction();
            }
        }

        static IEnumerator TimerFrameCore(IObserver<long> observer, int dueTimeFrameCount, int periodFrameCount, FrameCountType frameCountType, CancellationToken cancel)
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
                yield return frameCountType.GetYieldInstruction();
            }

            // period phase
            while (!cancel.IsCancellationRequested)
            {
                if (++currentFrame == periodFrameCount)
                {
                    observer.OnNext(sendCount++);
                    currentFrame = 0;
                }
                yield return frameCountType.GetYieldInstruction();
            }
        }

        public static IObservable<T> DelayFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");

            return Observable.Create<T>(observer =>
            {
                var cancel = new BooleanDisposable();

                source.Materialize().Subscribe(x =>
                {
                    if (x.Kind == NotificationKind.OnError)
                    {
                        observer.OnError(x.Exception);
                        cancel.Dispose();
                        return;
                    }

                    MainThreadDispatcher.StartCoroutine(DelayFrameCore(() => x.Accept(observer), frameCount, frameCountType, cancel));
                });

                return cancel;
            });
        }

        static IEnumerator DelayFrameCore(Action onNext, int frameCount, FrameCountType frameCountType, ICancelable cancel)
        {
            while (!cancel.IsDisposed && frameCount-- != 0)
            {
                yield return frameCountType.GetYieldInstruction();
            }
            if (!cancel.IsDisposed)
            {
                onNext();
            }
        }

        public static IObservable<T> SampleFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return Observable.Create<T>(observer =>
            {
                var latestValue = default(T);
                var isUpdated = false;
                var isCompleted = false;
                var gate = new object();

                var scheduling = new SingleAssignmentDisposable();
                scheduling.Disposable = Observable.IntervalFrame(frameCount, frameCountType)
                    .Subscribe(_ =>
                    {
                        lock (gate)
                        {
                            if (isUpdated)
                            {
                                var value = latestValue;
                                isUpdated = false;
                                try
                                {
                                    observer.OnNext(value);
                                }
                                catch
                                {
                                    scheduling.Dispose();
                                }
                            }
                            if (isCompleted)
                            {
                                observer.OnCompleted();
                                scheduling.Dispose();
                            }
                        }
                    });

                var sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = source.Subscribe(x =>
                {
                    lock (gate)
                    {
                        latestValue = x;
                        isUpdated = true;
                    }
                }, e =>
                {
                    lock (gate)
                    {
                        observer.OnError(e);
                        scheduling.Dispose();
                    }
                }
                , () =>
                {
                    lock (gate)
                    {
                        isCompleted = true;
                        sourceSubscription.Dispose();
                    }
                });

                return new CompositeDisposable { scheduling, sourceSubscription };
            });
        }

        public static IObservable<TSource> ThrottleFrame<TSource>(this IObservable<TSource> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return new AnonymousObservable<TSource>(observer =>
            {
                var gate = new object();
                var value = default(TSource);
                var hasValue = false;
                var cancelable = new SerialDisposable();
                var id = 0UL;

                var subscription = source.Subscribe(x =>
                    {
                        ulong currentid;
                        lock (gate)
                        {
                            hasValue = true;
                            value = x;
                            id = unchecked(id + 1);
                            currentid = id;
                        }
                        var d = new SingleAssignmentDisposable();
                        cancelable.Disposable = d;
                        d.Disposable = Observable.TimerFrame(frameCount, frameCountType)
                            .Subscribe(_ =>
                            {
                                lock (gate)
                                {
                                    if (hasValue && id == currentid)
                                        observer.OnNext(value);
                                    hasValue = false;
                                }
                            });
                    },
                    exception =>
                    {
                        cancelable.Dispose();

                        lock (gate)
                        {
                            observer.OnError(exception);
                            hasValue = false;
                            id = unchecked(id + 1);
                        }
                    },
                    () =>
                    {
                        cancelable.Dispose();

                        lock (gate)
                        {
                            if (hasValue)
                                observer.OnNext(value);
                            observer.OnCompleted();
                            hasValue = false;
                            id = unchecked(id + 1);
                        }
                    });

                return new CompositeDisposable(subscription, cancelable);
            });
        }

        public static IObservable<T> TimeoutFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return Observable.Create<T>(observer =>
            {
                object gate = new object();
                var objectId = 0ul;
                var isTimeout = false;

                Func<ulong, IDisposable> runTimer = (timerId) =>
                {
                    return Observable.TimerFrame(frameCount, frameCountType)
                        .Subscribe(_ =>
                        {
                            lock (gate)
                            {
                                if (objectId == timerId)
                                {
                                    isTimeout = true;
                                }
                            }
                            if (isTimeout)
                            {
                                observer.OnError(new TimeoutException());
                            }
                        });
                };

                var timerDisposable = new SerialDisposable();
                timerDisposable.Disposable = runTimer(objectId);

                var sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = source.Subscribe(x =>
                {
                    bool timeout;
                    lock (gate)
                    {
                        timeout = isTimeout;
                        objectId++;
                    }
                    if (timeout) return;

                    timerDisposable.Disposable = Disposable.Empty; // cancel old timer
                    observer.OnNext(x);
                    timerDisposable.Disposable = runTimer(objectId);
                }, ex =>
                {
                    bool timeout;
                    lock (gate)
                    {
                        timeout = isTimeout;
                        objectId++;
                    }
                    if (timeout) return;

                    timerDisposable.Dispose();
                    observer.OnError(ex);
                }, () =>
                {
                    bool timeout;
                    lock (gate)
                    {
                        timeout = isTimeout;
                        objectId++;
                    }
                    if (timeout) return;

                    timerDisposable.Dispose();
                    observer.OnCompleted();
                });

                return new CompositeDisposable { timerDisposable, sourceSubscription };
            });
        }

        public static IObservable<T> DelayFrameSubscription<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update)
        {
            return Observable.Create<T>(observer =>
            {
                var d = new MultipleAssignmentDisposable();
                d.Disposable = Observable.TimerFrame(frameCount, frameCountType)
                    .Subscribe(_ =>
                    {
                        d.Disposable = source.Subscribe(observer);
                    });

                return d;
            });
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
                .Finally(() => running = false)
                .Subscribe(onResult, onError, Stubs.Nop);

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
            return source.ObserveOn(Scheduler.MainThread);
        }

        public static IObservable<T> SubscribeOnMainThread<T>(this IObservable<T> source)
        {
            return source.SubscribeOn(Scheduler.MainThread);
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
            return Observable.Create<T>(observer =>
            {
                var isFirstSubscribe = true;
                var isDisposed = false;
                var isStopped = false;
                var e = sources.AsSafeEnumerable().GetEnumerator();
                var subscription = new SerialDisposable();
                var schedule = new SingleAssignmentDisposable();
                var gate = new object();

                var stopper = trigger.Subscribe(_ =>
                {
                    lock (gate)
                    {
                        isStopped = true;
                        e.Dispose();
                        subscription.Dispose();
                        schedule.Dispose();
                        observer.OnCompleted();
                    }
                }, observer.OnError);

                schedule.Disposable = Scheduler.CurrentThread.Schedule(self =>
                {
                    lock (gate)
                    {
                        if (isDisposed) return;
                        if (isStopped) return;

                        var current = default(IObservable<T>);
                        var hasNext = false;
                        var ex = default(Exception);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                            {
                                current = e.Current;
                                if (current == null) throw new InvalidOperationException("sequence is null.");
                            }
                            else
                            {
                                e.Dispose();
                            }
                        }
                        catch (Exception exception)
                        {
                            ex = exception;
                            e.Dispose();
                        }

                        if (ex != null)
                        {
                            stopper.Dispose();
                            observer.OnError(ex);
                            return;
                        }

                        if (!hasNext)
                        {
                            stopper.Dispose();
                            observer.OnCompleted();
                            return;
                        }

                        var source = e.Current;
                        var d = new SingleAssignmentDisposable();
                        subscription.Disposable = d;

                        var repeatObserver = Observer.Create<T>(observer.OnNext, observer.OnError, self);

                        if (isFirstSubscribe)
                        {
                            isFirstSubscribe = false;
                            d.Disposable = source.Subscribe(repeatObserver);
                        }
                        else
                        {
                            MainThreadDispatcher.SendStartCoroutine(SubscribeAfterEndOfFrame(d, source, repeatObserver, lifeTimeChecker));
                        }
                    }
                });

                return new CompositeDisposable(schedule, subscription, stopper, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        isDisposed = true;
                        e.Dispose();
                    }
                }));
            });
        }

        static IEnumerator SubscribeAfterEndOfFrame<T>(SingleAssignmentDisposable d, IObservable<T> source, IObserver<T> observer, GameObject lifeTimeChecker)
        {
            yield return new WaitForEndOfFrame();
            if (!d.IsDisposed && lifeTimeChecker != null)
            {
                d.Disposable = source.Subscribe(observer);
            }
        }
    }
}
