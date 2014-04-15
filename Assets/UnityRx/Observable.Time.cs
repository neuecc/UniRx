using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    // Timer, Interval, etc...
    public static partial class Observable
    {
        // LongRunnning...

        public static IObservable<long> Interval(TimeSpan period)
        {
            return TimerCore(period, period, Scheduler.ThreadPool);
        }

        public static IObservable<long> Interval(TimeSpan period, IScheduler scheduler)
        {
            return TimerCore(period, period, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return TimerCore(dueTime, Scheduler.ThreadPool);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime)
        {
            return TimerCore(dueTime, Scheduler.ThreadPool);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return TimerCore(dueTime, period, Scheduler.ThreadPool);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
        {
            return TimerCore(dueTime, period, Scheduler.ThreadPool);
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
            return Timestamp<TSource>(source, Scheduler.ThreadPool);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return source.Select(x => new Timestamped<TSource>(x, scheduler.Now));
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Delay(dueTime, Scheduler.ThreadPool);
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
    }
}