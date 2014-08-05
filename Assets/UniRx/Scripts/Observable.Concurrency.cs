using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<T> Synchronize<T>(this IObservable<T> source)
        {
            return source.Synchronize(new object());
        }

        public static IObservable<T> Synchronize<T>(this IObservable<T> source, object gate)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(
                    x => { lock (gate) observer.OnNext(x); },
                    x => { lock (gate) observer.OnError(x); },
                    () => { lock (gate) observer.OnCompleted(); });
            });
        }

        public static IObservable<T> ObserveOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var group = new CompositeDisposable();

                var first = source.Subscribe(x =>
                {
                    var d = scheduler.Schedule(() => observer.OnNext(x));
                    group.Add(d);
                }, ex =>
                {
                    var d = scheduler.Schedule(() => observer.OnError(ex));
                    group.Add(d);
                }, () =>
                {
                    var d = scheduler.Schedule(() => observer.OnCompleted());
                    group.Add(d);
                });

                group.Add(first);

                return group;
            });
        }

        public static IObservable<T> SubscribeOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var m = new SingleAssignmentDisposable();
                var d = new SerialDisposable();
                d.Disposable = m;

                m.Disposable = scheduler.Schedule(() =>
                {
                    d.Disposable = new ScheduledDisposable(scheduler, source.Subscribe(observer));
                });

                return d;
            });
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.DelaySubscription(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var d = new MultipleAssignmentDisposable();
                var dt = Scheduler.Normalize(dueTime);

                d.Disposable = scheduler.Schedule(dt, () =>
                {
                    d.Disposable = source.Subscribe(observer);
                });

                return d;
            });
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, DateTimeOffset dueTime)
        {
            return source.DelaySubscription(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var d = new MultipleAssignmentDisposable();

                d.Disposable = scheduler.Schedule(dueTime, () =>
                {
                    d.Disposable = source.Subscribe(observer);
                });

                return d;
            });
        }
    }
}