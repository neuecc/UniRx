using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
    public static partial class Observable
    {
        /// <summary>
        /// Create anonymous observable. Observer is auto detach when error, completed.
        /// </summary>
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new AnonymousObservable<T>(subscribe);
        }

        class AnonymousObservable<T> : IObservable<T>
        {
            readonly Func<IObserver<T>, IDisposable> subscribe;

            public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe)
            {
                this.subscribe = subscribe;
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                var subscription = new SingleAssignmentDisposable();

#if UNITY_5_0
                // In Unity 5.0.2p1/p2 occures IL2CPP compile error.
                // IL2CPP compile error when script contains method group of interface to delegate conversion
                var safeObserver = Observer.Create<T>(x => observer.OnNext(x), ex => observer.OnError(ex), () => observer.OnCompleted(), subscription);
#else
                var safeObserver = Observer.Create<T>(observer.OnNext, observer.OnError, observer.OnCompleted, subscription);
#endif

                if (Scheduler.IsCurrentThreadSchedulerScheduleRequired)
                {
                    Scheduler.CurrentThread.Schedule(() => subscription.Disposable = subscribe(safeObserver));
                }
                else
                {
                    subscription.Disposable = subscribe(safeObserver);
                }

                return subscription;
            }
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on DefaultSchedulers.ConstantTimeOperations.
        /// </summary>
        public static IObservable<T> Empty<T>()
        {
            return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on specified scheduler.
        /// </summary>
        public static IObservable<T> Empty<T>(IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                return scheduler.Schedule(observer.OnCompleted);
            });
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on DefaultSchedulers.ConstantTimeOperations. witness is for type inference.
        /// </summary>
        public static IObservable<T> Empty<T>(T witness)
        {
            return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on specified scheduler. witness is for type inference.
        /// </summary>
        public static IObservable<T> Empty<T>(IScheduler scheduler, T witness)
        {
            return Empty<T>(scheduler);
        }

        /// <summary>
        /// Non-Terminating Observable. It's no returns, never finish.
        /// </summary>
        public static IObservable<T> Never<T>()
        {
            return Observable.Create<T>(observer => Disposable.Empty);
        }

        /// <summary>
        /// Non-Terminating Observable. It's no returns, never finish. witness is for type inference.
        /// </summary>
        public static IObservable<T> Never<T>(T witness)
        {
            return Never<T>();
        }

        /// <summary>
        /// Return single sequence on DefaultSchedulers.ConstantTimeOperations.
        /// </summary>
        public static IObservable<T> Return<T>(T value)
        {
            return Return<T>(value, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Return single sequence on specified scheduler.
        /// </summary>
        public static IObservable<T> Return<T>(T value, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                return scheduler.Schedule(() =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });
            });
        }

        /// <summary>
        /// Empty Observable. Returns only onError on DefaultSchedulers.ConstantTimeOperations.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error)
        {
            return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only onError on DefaultSchedulers.ConstantTimeOperations. witness if for Type inference.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, T witness)
        {
            return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only onError on specified scheduler.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                return scheduler.Schedule(() => observer.OnError(error));
            });
        }

        /// <summary>
        /// Empty Observable. Returns only onError on specified scheduler. witness if for Type inference.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, IScheduler scheduler, T witness)
        {
            return Throw<T>(error, scheduler);
        }

        public static IObservable<int> Range(int start, int count)
        {
            return Range(start, count, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<int> Range(int start, int count, IScheduler scheduler)
        {
            return Observable.Create<int>(observer =>
            {
                var i = 0;
                return scheduler.Schedule((Action self) =>
                {
                    if (i < count)
                    {
                        int v = start + i;
                        observer.OnNext(v);
                        System.Threading.Interlocked.Increment(ref i);
                        self();
                    }
                    else
                    {
                        observer.OnCompleted();
                    }
                });
            });
        }

        public static IObservable<T> Repeat<T>(T value)
        {
            return Repeat(value, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> Repeat<T>(T value, IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return Observable.Create<T>(observer =>
            {
                return scheduler.Schedule(self =>
                {
                    observer.OnNext(value);
                    self();
                });
            });
        }

        public static IObservable<T> Repeat<T>(T value, int repeatCount)
        {
            return Repeat(value, repeatCount, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> Repeat<T>(T value, int repeatCount, IScheduler scheduler)
        {
            if (repeatCount < 0) throw new ArgumentOutOfRangeException("repeatCount");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return Observable.Create<T>(observer =>
            {
                var currentCount = repeatCount;
                return scheduler.Schedule(self =>
                {
                    if (currentCount > 0)
                    {
                        observer.OnNext(value);
                        currentCount--;
                    }

                    if (currentCount == 0)
                    {
                        observer.OnCompleted();
                        return;
                    }

                    self();
                });
            });
        }

        public static IObservable<T> Repeat<T>(this IObservable<T> source)
        {
            return RepeatInfinite(source).Concat();
        }

        static IEnumerable<IObservable<T>> RepeatInfinite<T>(IObservable<T> source)
        {
            while (true)
            {
                yield return source;
            }
        }

        /// <summary>
        /// Same as Repeat() but if arriving contiguous "OnComplete" Repeat stops.
        /// </summary>
        public static IObservable<T> RepeatSafe<T>(this IObservable<T> source)
        {
            return RepeatSafeCore(RepeatInfinite(source));
        }

        static IObservable<T> RepeatSafeCore<T>(IEnumerable<IObservable<T>> sources)
        {
            return Observable.Create<T>(observer =>
            {
                var isDisposed = false;
                var isRunNext = false;
                var e = sources.AsSafeEnumerable().GetEnumerator();
                var subscription = new SerialDisposable();
                var gate = new object();

                var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(self =>
                {
                    lock (gate)
                    {
                        if (isDisposed) return;

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
                            observer.OnError(ex);
                            return;
                        }

                        if (!hasNext)
                        {
                            observer.OnCompleted();
                            return;
                        }

                        var source = e.Current;
                        var d = new SingleAssignmentDisposable();
                        subscription.Disposable = d;
                        d.Disposable = source.Subscribe(x =>
                        {
                            isRunNext = true;
                            observer.OnNext(x);
                        }, observer.OnError, () =>
                        {
                            if (isRunNext && !isDisposed)
                            {
                                isRunNext = false;
                                self();
                            }
                            else
                            {
                                e.Dispose();
                                if (!isDisposed)
                                {
                                    observer.OnCompleted();
                                }
                            }
                        });
                    }
                });

                return new CompositeDisposable(schedule, subscription, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        isDisposed = true;
                        e.Dispose();
                    }
                }));
            });
        }

        public static IObservable<T> Defer<T>(Func<IObservable<T>> observableFactory)
        {
            return Observable.Create<T>(observer =>
            {
                IObservable<T> source;
                try
                {
                    source = observableFactory();
                }
                catch (Exception ex)
                {
                    source = Throw<T>(ex);
                }

                return source.Subscribe(observer);
            });
        }

        public static IObservable<T> Start<T>(Func<T> function)
        {
            return Start(function, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<T> Start<T>(Func<T> function, IScheduler scheduler)
        {
            return ToAsync(function, scheduler)();
        }

        public static IObservable<Unit> Start(Action action)
        {
            return Start(action, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<Unit> Start(Action action, IScheduler scheduler)
        {
            return ToAsync(action, scheduler)();
        }

        public static Func<IObservable<T>> ToAsync<T>(Func<T> function)
        {
            return ToAsync(function, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static Func<IObservable<T>> ToAsync<T>(Func<T> function, IScheduler scheduler)
        {
            return () =>
            {
                var subject = new AsyncSubject<T>();

                scheduler.Schedule(() =>
                {
                    var result = default(T);
                    try
                    {
                        result = function();
                    }
                    catch (Exception exception)
                    {
                        subject.OnError(exception);
                        return;
                    }
                    subject.OnNext(result);
                    subject.OnCompleted();
                });

                return subject.AsObservable();
            };
        }

        public static Func<IObservable<Unit>> ToAsync(Action action)
        {
            return ToAsync(action, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static Func<IObservable<Unit>> ToAsync(Action action, IScheduler scheduler)
        {
            return () =>
            {
                var subject = new AsyncSubject<Unit>();

                scheduler.Schedule(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        subject.OnError(exception);
                        return;
                    }
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });

                return subject.AsObservable();
            };
        }
    }
}