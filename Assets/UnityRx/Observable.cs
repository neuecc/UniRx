using System;
using System.Collections;

namespace UnityRx
{
    // TODO:Subject? Scheduler?

    // Select, Where, SelectMany, Zip, Merge, CombineLatest, Switch, ObserveOn, Retry, Defer, Etc...


    public static class Observable
    {
        // TODO:need scheduler
        public static IObservable<int> Range(int start, int count)
        {
            return AnonymousObservable.Create<int>(observer =>
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        observer.OnNext(start++);
                    }
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                // TODO:Cancellable!
                return Disposable.Empty;
            });

        }

        public static IObservable<TR> Select<T, TR>(this IObservable<T> source, Func<T, TR> selector)
        {
            return AnonymousObservable.Create<TR>(observer =>
            {
                return source.Subscribe(AnonymousObserver.Create<T>(x =>
                {
                    var v = selector(x);
                    observer.OnNext(v);
                }, observer.OnError, observer.OnCompleted));
            });
        }

        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return AnonymousObservable.Create<T>(observer =>
            {
                return source.Subscribe(AnonymousObserver.Create<T>(x =>
                {
                    if (predicate(x))
                    {
                        observer.OnNext(x);
                    }
                }, observer.OnError, observer.OnCompleted));
            });
        }

        public static IObservable<TR> SelectMany<T, TR>(this IObservable<T> source, Func<T, IObservable<TR>> selector)
        {
            return source.Select(selector).Merge();
        }

        public static IObservable<TR> SelectMany<T, TC, TR>(this IObservable<T> source, Func<T, IObservable<TC>> collectionSelector, Func<T, TC, TR> selector)
        {
            return source.SelectMany(x => collectionSelector(x).Select(y => selector(x, y)));
        }

        public static IObservable<T> Merge<T>(this IObservable<IObservable<T>> sources)
        {
            return AnonymousObservable.Create<T>(observer =>
            {
                var group = new CompositeDisposable();

                var first = sources.Subscribe(innerSource =>
                {
                    var d = innerSource.Subscribe(observer.OnNext);
                    group.Add(d);
                }, observer.OnError, observer.OnCompleted);

                group.Add(first);

                return group;
            });
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Delay(dueTime, Scheduler.GameLoop);
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return AnonymousObservable.Create<T>(observer =>
            {
                var group = new CompositeDisposable();

                var first = source.Subscribe(x =>
                {
                    var d = scheduler.Schedule(() => observer.OnNext(x), dueTime);
                    group.Add(d);
                }, observer.OnError, observer.OnCompleted);

                group.Add(first);

                return group;
            });
        }

        public static IObservable<T> ObserveOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return AnonymousObservable.Create<T>(observer =>
            {
                var group = new CompositeDisposable();

                var first = source.Subscribe(x =>
                {
                    var d = scheduler.Schedule(() => observer.OnNext(x));
                    group.Add(d);
                }, observer.OnError, observer.OnCompleted);

                group.Add(first);

                return group;
            });
        }

        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source)
        {
            return source;
            // throw new NotImplementedException();
        }
    }
}