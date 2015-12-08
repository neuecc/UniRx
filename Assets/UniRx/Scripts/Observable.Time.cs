using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx
{
    // Timer, Interval, etc...
    public static partial class Observable
    {
        // needs LongRunnning...

        public static IObservable<long> Interval(TimeSpan period)
        {
            return new TimerObservable(period, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Interval(TimeSpan period, IScheduler scheduler)
        {
            return new TimerObservable(period, period, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return new TimerObservable(dueTime, null, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime)
        {
            return new TimerObservable(dueTime, null, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return new TimerObservable(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
        {
            return new TimerObservable(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, null, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, null, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, period, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, period, scheduler);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source)
        {
            return Timestamp<TSource>(source, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return new TimestampObservable<TSource>(source, scheduler);
        }

        public static IObservable<UniRx.TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source)
        {
            return TimeInterval(source, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<UniRx.TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return new UniRx.Operators.TimeIntervalObservable<TSource>(source, scheduler);
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Delay(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Delay<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new DelayObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<T> Sample<T>(this IObservable<T> source, TimeSpan interval)
        {
            return source.Sample(interval, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Sample<T>(this IObservable<T> source, TimeSpan interval, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var latestValue = default(T);
                var isUpdated = false;
                var isCompleted = false;
                var gate = new object();

                var scheduling = scheduler.Schedule(interval, self =>
                {
                    lock (gate)
                    {
                        if (isUpdated)
                        {
                            var value = latestValue;
                            isUpdated = false;
                            observer.OnNext(value);
                        }
                        if (isCompleted)
                        {
                            observer.OnCompleted();
                        }
                    }
                    self(interval);
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

        public static IObservable<TSource> Throttle<TSource>(this IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.Throttle(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Throttle<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            // this code is borrowed from Rx Official(rx.codeplex.com)
            return Observable.Create<TSource>(observer =>
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
                        d.Disposable = scheduler.Schedule(dueTime, () =>
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

        public static IObservable<TSource> ThrottleFirst<TSource>(this IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.ThrottleFirst(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> ThrottleFirst<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Observable.Create<TSource>(observer =>
            {
                var gate = new object();
                var open = true;
                var cancelable = new SerialDisposable();

                var subscription = source.Subscribe(x =>
                {
                    lock (gate)
                    {
                        if (!open) return;
                        observer.OnNext(x);
                        open = false;
                    }

                    var d = new SingleAssignmentDisposable();
                    cancelable.Disposable = d;
                    d.Disposable = scheduler.Schedule(dueTime, () =>
                    {
                        lock (gate)
                        {
                            open = true;
                        }
                    });

                },
                    exception =>
                    {
                        cancelable.Dispose();

                        lock (gate)
                        {
                            observer.OnError(exception);
                        }
                    },
                    () =>
                    {
                        cancelable.Dispose();

                        lock (gate)
                        {
                            observer.OnCompleted();

                        }
                    });

                return new CompositeDisposable(subscription, cancelable);
            });
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                object gate = new object();
                var objectId = 0ul;
                var isTimeout = false;

                Func<ulong, IDisposable> runTimer = (timerId) =>
                {
                    return scheduler.Schedule(dueTime, () =>
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

        public static IObservable<T> Timeout<T>(this IObservable<T> source, DateTimeOffset dueTime)
        {
            return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                var isFinished = false;
                var sourceSubscription = new SingleAssignmentDisposable();

                var timerD = scheduler.Schedule(dueTime, () =>
                {
                    lock (gate)
                    {
                        if (isFinished) return;
                        isFinished = true;
                    }
                    sourceSubscription.Dispose();
                    observer.OnError(new TimeoutException());
                });

                sourceSubscription.Disposable = source.Subscribe(x =>
                {
                    lock (gate)
                    {
                        if (!isFinished) observer.OnNext(x);
                    }
                }, ex =>
                {
                    lock (gate)
                    {
                        if (isFinished) return;
                        isFinished = true;
                    }
                    observer.OnError(ex);
                }, () =>
                 {
                     lock (gate)
                     {
                         if (!isFinished)
                         {
                             isFinished = true;
                             timerD.Dispose();
                         }
                         observer.OnCompleted();
                     }
                 });

                return new CompositeDisposable { timerD, sourceSubscription };
            });
        }
    }
}