using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityRx
{
    public interface IObservable<out T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }

    // Standard Query Operators

    // onNext implementation guide. enclose otherFunc but onNext is not catch.
    // try{ otherFunc(); } catch { onError() }
    // onNext();

    public static partial class Observable
    {
        static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1); // from .NET 4.5

        public static IObservable<TR> Select<T, TR>(this IObservable<T> source, Func<T, TR> selector)
        {
            return Observable.Create<TR>(observer =>
            {
                return source.Subscribe(Observer.Create<T>(x =>
                {
                    var v = default(TR);
                    try
                    {
                        v = selector(x);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnNext(v);
                }, observer.OnError, observer.OnCompleted));
            });
        }

        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(Observer.Create<T>(x =>
                {
                    var isBypass = default(bool);
                    try
                    {
                        isBypass = predicate(x);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    if (isBypass)
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
            return Observable.Create<T>(observer =>
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
            return source.Delay(dueTime, Scheduler.Immediate); // TODO:not immediate!
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var group = new CompositeDisposable();

                var first = source.Subscribe(x =>
                {
                    var d = scheduler.Schedule(dueTime, () => observer.OnNext(x));
                    group.Add(d);
                }, observer.OnError, observer.OnCompleted);

                group.Add(first);

                return group;
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
                }, observer.OnError, observer.OnCompleted);

                group.Add(first);

                return group;
            });
        }

        public static IObservable<T[]> ToArray<T>(this IObservable<T> source)
        {
            return Observable.Create<T[]>(observer =>
            {
                var list = new List<T>();
                return source.Subscribe(x => list.Add(x), observer.OnError, () =>
                {
                    observer.OnNext(list.ToArray());
                    observer.OnCompleted();
                });
            });
        }

        public static IObservable<T> Do<T>(this IObservable<T> source, Action<T> action)
        {
            return source.Select(x =>
            {
                action(x);
                return x;
            });
        }

        public static IObservable<Notification<T>> Materialize<T>(this IObservable<T> source)
        {
            return Observable.Create<Notification<T>>(observer =>
            {
                return source.Subscribe(
                    x => observer.OnNext(Notification.CreateOnNext(x)),
                    x =>
                    {
                        observer.OnNext(Notification.CreateOnError<T>(x));
                        observer.OnCompleted();
                    },
                    () =>
                    {
                        observer.OnNext(Notification.CreateOnCompleted<T>());
                        observer.OnCompleted();
                    });
            });
        }

        public static IObservable<T> Dematerialize<T>(this IObservable<Notification<T>> source)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(x =>
                {
                    switch (x.Kind)
                    {
                        case NotificationKind.OnNext:
                            observer.OnNext(x.Value);
                            break;
                        case NotificationKind.OnError:
                            observer.OnError(x.Exception);
                            break;
                        case NotificationKind.OnCompleted:
                            observer.OnCompleted();
                            break;
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }

        public static IObservable<TSource> Scan<TSource>(this IObservable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            return Observable.Create<TSource>(observer =>
            {
                bool isFirst = true;
                TSource prev = default(TSource);
                return source.Subscribe(x =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        prev = x;
                        observer.OnNext(x);
                    }
                    else
                    {
                        try
                        {
                            prev = func(prev, x); // prev as current
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return;
                        }

                        observer.OnNext(prev);
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }

        public static IObservable<TAccumulate> Scan<TSource, TAccumulate>(this IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return Observable.Create<TAccumulate>(observer =>
            {
                var prev = seed;
                observer.OnNext(seed);

                return source.Subscribe(x =>
                {
                    try
                    {
                        prev = func(prev, x); // prev as next
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnNext(prev);
                }, observer.OnError, observer.OnCompleted);
            });
        }

        // TODO:needs comparer overload

        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source)
        {
            return source.DistinctUntilChanged(x => x);
        }

        public static IObservable<T> DistinctUntilChanged<T, TKey>(this IObservable<T> source, Func<T, TKey> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return Observable.Create<T>(observer =>
            {
                var isFirst = true;
                var prevKey = default(TKey);
                return source.Subscribe(x =>
                {
                    TKey currentKey;
                    try
                    {
                        currentKey = selector(x);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    var sameKey = false;
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        try
                        {
                            sameKey = currentKey.Equals(prevKey);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return;
                        }
                    }
                    if (!sameKey)
                    {
                        prevKey = currentKey;
                        observer.OnNext(x);
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }

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

        public static T Wait<T>(this IObservable<T> source)
        {
            return source.WaitCore(throwOnEmpty: true, timeout: InfiniteTimeSpan);
        }

        public static T Wait<T>(this IObservable<T> source, TimeSpan timeout)
        {
            return source.WaitCore(throwOnEmpty: true, timeout: timeout);
        }

        static T WaitCore<T>(this IObservable<T> source, bool throwOnEmpty, TimeSpan timeout)
        {
            if (source == null) throw new ArgumentNullException("source");

            var semaphore = new System.Threading.ManualResetEvent(false);

            var seenValue = false;
            var value = default(T);
            var ex = default(Exception);

            using (source.Subscribe(
                onNext: x => { seenValue = true; value = x; },
                onError: x => { ex = x; semaphore.Set(); },
                onCompleted: () => semaphore.Set()))
            {
                var waitComplete = (timeout == InfiniteTimeSpan)
                    ? semaphore.WaitOne()
                    : semaphore.WaitOne(timeout);

                if (!waitComplete)
                {
                    throw new TimeoutException("OnCompleted not fired.");
                }
            }

            if (ex != null) throw ex;
            if (throwOnEmpty && !seenValue) throw new InvalidOperationException("No Elements.");

            return value;
        }
    }
}