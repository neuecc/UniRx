using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    // Timer, Interval, etc...
    public static partial class Observable
    {
        // needs LongRunnning...

        public static IObservable<long> Interval(TimeSpan period)
        {
            return TimerCore(period, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Interval(TimeSpan period, IScheduler scheduler)
        {
            return TimerCore(period, period, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return TimerCore(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime)
        {
            return TimerCore(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return TimerCore(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
        {
            return TimerCore(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, IScheduler scheduler)
        {
            return TimerCore(dueTime, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, IScheduler scheduler)
        {
            return TimerCore(dueTime, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
        {
            return TimerCore(dueTime, period, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
        {
            return TimerCore(dueTime, period, scheduler);
        }

        static IObservable<long> TimerCore(TimeSpan dueTime, IScheduler scheduler)
        {
            var time = Scheduler.Normalize(dueTime);

            return Observable.Create<long>(observer =>
            {
                return scheduler.Schedule(time, self =>
                {
                    observer.OnNext(0);
                    observer.OnCompleted();
                });
            });
        }

        static IObservable<long> TimerCore(DateTimeOffset dueTime, IScheduler scheduler)
        {
            return Observable.Create<long>(observer =>
            {
                return scheduler.Schedule(dueTime, self =>
                {
                    observer.OnNext(0);
                    observer.OnCompleted();
                });
            });
        }

        static IObservable<long> TimerCore(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
        {
            var timeD = Scheduler.Normalize(dueTime);
            var timeP = Scheduler.Normalize(period);

            return Observable.Create<long>(observer =>
            {
                var count = 0;
                return scheduler.Schedule(timeD, self =>
                {
                    observer.OnNext(count);
                    count++;
                    self(timeP);
                });
            });
        }

        static IObservable<long> TimerCore(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
        {
            var timeP = Scheduler.Normalize(period);

            return Observable.Create<long>(observer =>
            {
                var nextTime = dueTime;
                var count = 0L;

                return scheduler.Schedule(nextTime, self =>
                {
                    if (timeP > TimeSpan.Zero)
                    {
                        nextTime = nextTime + period;
                        var now = scheduler.Now;
                        if (nextTime <= now)
                        {
                            nextTime = now + period;
                        }
                    }

                    observer.OnNext(count);
                    count++;
                    self(nextTime);
                });
            });
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source)
        {
            return Timestamp<TSource>(source, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return source.Select(x => new Timestamped<TSource>(x, scheduler.Now));
        }

        public static IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source)
        {
            return TimeInterval(source, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return Defer(() =>
            {
                var last = scheduler.Now;
                return source.Select(x =>
                {
                    var now = scheduler.Now;
                    var span = now.Subtract(last);
                    last = now;
                    return new TimeInterval<TSource>(x, span);
                });
            });
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Delay(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Delay<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            // This code is borrowed from Rx(rx.codeplex.com)
            return Observable.Create<TSource>(observer =>
            {
                var gate = new object();
                var q = new Queue<Timestamped<Notification<TSource>>>();
                var active = false;
                var running = false;
                var cancelable = new SerialDisposable();
                var exception = default(Exception);

                var subscription = source.Materialize().Timestamp(scheduler).Subscribe(notification =>
                {
                    var shouldRun = false;

                    lock (gate)
                    {
                        if (notification.Value.Kind == NotificationKind.OnError)
                        {
                            q.Clear();
                            q.Enqueue(notification);
                            exception = notification.Value.Exception;
                            shouldRun = !running;
                        }
                        else
                        {
                            q.Enqueue(new Timestamped<Notification<TSource>>(notification.Value, notification.Timestamp.Add(dueTime)));
                            shouldRun = !active;
                            active = true;
                        }
                    }

                    if (shouldRun)
                    {
                        if (exception != null)
                            observer.OnError(exception);
                        else
                        {
                            var d = new SingleAssignmentDisposable();
                            cancelable.Disposable = d;
                            d.Disposable = scheduler.Schedule(dueTime, self =>
                            {
                                lock (gate)
                                {
                                    if (exception != null)
                                        return;
                                    running = true;
                                }
                                Notification<TSource> result;

                                do
                                {
                                    result = null;
                                    lock (gate)
                                    {
                                        if (q.Count > 0 && q.Peek().Timestamp.CompareTo(scheduler.Now) <= 0)
                                            result = q.Dequeue().Value;
                                    }

                                    if (result != null)
                                        result.Accept(observer);
                                } while (result != null);

                                var shouldRecurse = false;
                                var recurseDueTime = TimeSpan.Zero;
                                var e = default(Exception);
                                lock (gate)
                                {
                                    if (q.Count > 0)
                                    {
                                        shouldRecurse = true;
                                        recurseDueTime = TimeSpan.FromTicks(Math.Max(0, q.Peek().Timestamp.Subtract(scheduler.Now).Ticks));
                                    }
                                    else
                                        active = false;
                                    e = exception;
                                    running = false;
                                }
                                if (e != null)
                                    observer.OnError(e);
                                else if (shouldRecurse)
                                    self(recurseDueTime);
                            });
                        }
                    }
                });

                return new CompositeDisposable(subscription, cancelable);
            });
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

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var beforeTime = scheduler.Now;

                Func<IDisposable> runTimer = () => scheduler.Schedule(dueTime, () =>
                {
                    if (scheduler.Now - beforeTime >= dueTime)
                    {
                        observer.OnError(new TimeoutException());
                    }
                });

                var timerDisposable = new SerialDisposable();
                timerDisposable.Disposable = runTimer();

                var sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = source.Subscribe(x =>
                {
                    timerDisposable.Disposable = Disposable.Empty; // Cancel Old Timer
                    observer.OnNext(x);
                    beforeTime = scheduler.Now;
                    timerDisposable.Disposable = runTimer();
                }, observer.OnError, observer.OnCompleted);

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
                var timerD = scheduler.Schedule(dueTime, () =>
                {
                    observer.OnError(new TimeoutException());
                });

                var sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = source.Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);

                return new CompositeDisposable { timerD, sourceSubscription };
            });
        }
    }
}