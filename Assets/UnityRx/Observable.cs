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

        // needs comparer overload

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
    }
}