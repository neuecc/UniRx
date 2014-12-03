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

        public static IObservable<T> Amb<T>(params IObservable<T>[] sources)
        {
            return Amb(sources.AsEnumerable());
        }

        public static IObservable<T> Amb<T>(IEnumerable<IObservable<T>> sources)
        {
            var result = Observable.Never<T>();
            foreach (var item in sources)
            {
                var second = item;
                result = result.Amb(second);
            }
            return result;
        }

        public static IObservable<T> Amb<T>(this IObservable<T> source, IObservable<T> second)
        {
            return Observable.Create<T>(observer =>
            {
                var choice = AmbState.Neither;
                var gate = new Object();

                var leftSubscription = new SingleAssignmentDisposable();
                var rightSubscription = new SingleAssignmentDisposable();

                leftSubscription.Disposable = source.Subscribe(x =>
                {
                    lock (gate)
                    {
                        if (choice == AmbState.Neither)
                        {
                            choice = AmbState.Left;
                            rightSubscription.Dispose();
                            // We can avoid lock every call but I'm not confident in AOT Safety.
                            // I'll try, check...
                            // left.Observer = observer;
                        }
                    }

                    if (choice == AmbState.Left) observer.OnNext(x);
                }, ex =>
                {
                    lock (gate)
                    {
                        if (choice == AmbState.Neither)
                        {
                            choice = AmbState.Left;
                            rightSubscription.Dispose();
                        }
                    }

                    if (choice == AmbState.Left) observer.OnError(ex);
                }, () =>
                {
                    lock (gate)
                    {
                        if (choice == AmbState.Neither)
                        {
                            choice = AmbState.Left;
                            rightSubscription.Dispose();
                        }
                    }

                    if (choice == AmbState.Left) observer.OnCompleted();
                });

                rightSubscription.Disposable = second.Subscribe(x =>
                {
                    lock (gate)
                    {
                        if (choice == AmbState.Neither)
                        {
                            choice = AmbState.Right;
                            leftSubscription.Dispose();
                        }
                    }

                    if (choice == AmbState.Right) observer.OnNext(x);
                }, ex =>
                {
                    lock (gate)
                    {
                        if (choice == AmbState.Neither)
                        {
                            choice = AmbState.Right;
                            leftSubscription.Dispose();
                        }
                    }

                    if (choice == AmbState.Right) observer.OnError(ex);
                }, () =>
                {
                    lock (gate)
                    {
                        if (choice == AmbState.Neither)
                        {
                            choice = AmbState.Right;
                            leftSubscription.Dispose();
                        }
                    }

                    if (choice == AmbState.Right) observer.OnCompleted();
                });

                return new CompositeDisposable { leftSubscription, rightSubscription };
            });
        }

        enum AmbState
        {
            Left, Right, Neither
        }
    }
}