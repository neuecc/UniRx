using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UniRx
{
    // Standard Query Operators

    // onNext implementation guide. enclose otherFunc but onNext is not catch.
    // try{ otherFunc(); } catch { onError() }
    // onNext();

    public static partial class Observable
    {
        static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1); // from .NET 4.5

        public static IObservable<TR> Select<T, TR>(this IObservable<T> source, Func<T, TR> selector)
        {
            return Select(source, (x, i) => selector(x));
        }

        public static IObservable<TR> Select<T, TR>(this IObservable<T> source, Func<T, int, TR> selector)
        {
            return Observable.Create<TR>(observer =>
            {
                var index = 0;
                return source.Subscribe(Observer.Create<T>(x =>
                {
                    var v = default(TR);
                    try
                    {
                        v = selector(x, index++);
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
            return Where(source, (x, i) => predicate(x));
        }

        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, int, bool> predicate)
        {
            return Observable.Create<T>(observer =>
            {
                var index = 0;
                return source.Subscribe(Observer.Create<T>(x =>
                {
                    var isBypass = default(bool);
                    try
                    {
                        isBypass = predicate(x, index++);
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
        public static IObservable<TR> SelectMany<T, TR>(this IObservable<T> source, IObservable<TR> other)
        {
            return SelectMany(source, _ => other);
        }

        public static IObservable<TR> SelectMany<T, TR>(this IObservable<T> source, Func<T, IObservable<TR>> selector)
        {
            return source.Select(selector).Merge();
        }

        public static IObservable<TResult> SelectMany<TSource, TResult>(this IObservable<TSource> source, Func<TSource, int, IObservable<TResult>> selector)
        {
            return source.Select(selector).Merge();
        }

        public static IObservable<TR> SelectMany<T, TC, TR>(this IObservable<T> source, Func<T, IObservable<TC>> collectionSelector, Func<T, TC, TR> resultSelector)
        {
            return source.SelectMany(x => collectionSelector(x).Select(y => resultSelector(x, y)));
        }

        public static IObservable<TResult> SelectMany<TSource, TCollection, TResult>(this IObservable<TSource> source, Func<TSource, int, IObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            return source.SelectMany((x, i) => collectionSelector(x, i).Select((y, i2) => resultSelector(x, i, y, i2)));
        }

        public static IObservable<TResult> SelectMany<TSource, TResult>(this IObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return new AnonymousObservable<TResult>(observer =>
                source.Subscribe(
                    x =>
                    {
                        var xs = default(IEnumerable<TResult>);
                        try
                        {
                            xs = selector(x);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        var e = xs.AsSafeEnumerable().GetEnumerator();

                        try
                        {
                            var hasNext = true;
                            while (hasNext)
                            {
                                hasNext = false;
                                var current = default(TResult);

                                try
                                {
                                    hasNext = e.MoveNext();
                                    if (hasNext)
                                    {
                                        current = e.Current;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    observer.OnError(exception);
                                    return;
                                }

                                if (hasNext)
                                {
                                    observer.OnNext(current);
                                }
                            }
                        }
                        finally
                        {
                            if (e != null)
                            {
                                e.Dispose();
                            }
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted
                )
            );
        }

        public static IObservable<TResult> SelectMany<TSource, TResult>(this IObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            return Observable.Create<TResult>(observer =>
            {
                var index = 0;

                return source.Subscribe(
                    x =>
                    {
                        var xs = default(IEnumerable<TResult>);
                        try
                        {
                            xs = selector(x, checked(index++));
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        var e = xs.AsSafeEnumerable().GetEnumerator();

                        try
                        {
                            var hasNext = true;
                            while (hasNext)
                            {
                                hasNext = false;
                                var current = default(TResult);

                                try
                                {
                                    hasNext = e.MoveNext();
                                    if (hasNext)
                                    {
                                        current = e.Current;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    observer.OnError(exception);
                                    return;
                                }

                                if (hasNext)
                                    observer.OnNext(current);
                            }
                        }
                        finally
                        {
                            if (e != null)
                                e.Dispose();
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted
                );
            });
        }

        public static IObservable<TResult> SelectMany<TSource, TCollection, TResult>(this IObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new AnonymousObservable<TResult>(observer =>
                source.Subscribe(
                    x =>
                    {
                        var xs = default(IEnumerable<TCollection>);
                        try
                        {
                            xs = collectionSelector(x);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        var e = xs.AsSafeEnumerable().GetEnumerator();

                        try
                        {
                            var hasNext = true;
                            while (hasNext)
                            {
                                hasNext = false;
                                var current = default(TResult);

                                try
                                {
                                    hasNext = e.MoveNext();
                                    if (hasNext)
                                    {
                                        current = resultSelector(x, e.Current);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    observer.OnError(exception);
                                    return;
                                }

                                if (hasNext)
                                {
                                    observer.OnNext(current);
                                }
                            }
                        }
                        finally
                        {
                            if (e != null)
                            {
                                e.Dispose();
                            }
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted
                )
            );
        }

        public static IObservable<TResult> SelectMany<TSource, TCollection, TResult>(this IObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            return Observable.Create<TResult>(observer =>
            {
                var index = 0;

                return source.Subscribe(
                    x =>
                    {
                        var xs = default(IEnumerable<TCollection>);
                        try
                        {
                            xs = collectionSelector(x, checked(index++));
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        var e = xs.AsSafeEnumerable().GetEnumerator();

                        try
                        {
                            var eIndex = 0;
                            var hasNext = true;
                            while (hasNext)
                            {
                                hasNext = false;
                                var current = default(TResult);

                                try
                                {
                                    hasNext = e.MoveNext();
                                    if (hasNext)
                                        current = resultSelector(x, index, e.Current, checked(eIndex++));
                                }
                                catch (Exception exception)
                                {
                                    observer.OnError(exception);
                                    return;
                                }

                                if (hasNext)
                                    observer.OnNext(current);
                            }
                        }
                        finally
                        {
                            if (e != null)
                                e.Dispose();
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted
                );
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

        public static IObservable<T> Do<T>(this IObservable<T> source, IObserver<T> observer)
        {
            return Do(source, observer.OnNext, observer.OnError, observer.OnCompleted);
        }


        public static IObservable<T> Do<T>(this IObservable<T> source, Action<T> onNext)
        {
            return Do(source, onNext, Stubs.Throw, Stubs.Nop);
        }

        public static IObservable<T> Do<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return Do(source, onNext, onError, Stubs.Nop);
        }

        public static IObservable<T> Do<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return Do(source, onNext, Stubs.Throw, onCompleted);
        }

        public static IObservable<T> Do<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(x =>
                {
                    try
                    {
                        if (onNext != Stubs.Ignore<T>)
                        {
                            onNext(x);
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnNext(x);
                }, ex =>
                {
                    try
                    {
                        onError(ex);
                    }
                    catch (Exception ex2)
                    {
                        observer.OnError(ex2);
                        return;
                    }
                    observer.OnError(ex);
                }, () =>
                {
                    try
                    {
                        onCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnCompleted();
                });
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
                    if (x.Kind == NotificationKind.OnNext)
                    {
                        observer.OnNext(x.Value);
                    }
                    else if (x.Kind == NotificationKind.OnError)
                    {
                        observer.OnError(x.Exception);
                    }
                    else if (x.Kind == NotificationKind.OnCompleted)
                    {
                        observer.OnCompleted();
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }

        public static IObservable<T> DefaultIfEmpty<T>(this IObservable<T> source)
        {
            return DefaultIfEmpty(source, default(T));
        }

        public static IObservable<T> DefaultIfEmpty<T>(this IObservable<T> source, T defaultValue)
        {
            return Observable.Create<T>(observer =>
            {
                var hasValue = false;

                return source.Subscribe(x => { hasValue = true; observer.OnNext(x); }, observer.OnError, () =>
                {
                    if (!hasValue)
                    {
                        observer.OnNext(defaultValue);
                    }
                    observer.OnCompleted();
                });
            });
        }

        public static IObservable<TSource> Distinct<TSource>(this IObservable<TSource> source)
        {
            return Distinct<TSource>(source, (IEqualityComparer<TSource>)null);
        }

        public static IObservable<TSource> Distinct<TSource>(this IObservable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            // don't use x => x for avoid iOS AOT issue.
            return Observable.Create<TSource>(observer =>
            {
                var hashSet = (comparer == null)
                    ? new HashSet<TSource>()
                    : new HashSet<TSource>(comparer);
                return source.Subscribe(
                    x =>
                    {
                        var key = default(TSource);
                        var hasAdded = false;

                        try
                        {
                            key = x;
                            hasAdded = hashSet.Add(key);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        if (hasAdded)
                        {
                            observer.OnNext(x);
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted
                );
            });
        }

        public static IObservable<TSource> Distinct<TSource, TKey>(this IObservable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Distinct(source, keySelector, null);
        }

        public static IObservable<TSource> Distinct<TSource, TKey>(this IObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return Observable.Create<TSource>(observer =>
            {
                var hashSet = (comparer == null)
                    ? new HashSet<TKey>()
                    : new HashSet<TKey>(comparer);
                return source.Subscribe(
                    x =>
                    {
                        var key = default(TKey);
                        var hasAdded = false;

                        try
                        {
                            key = keySelector(x);
                            hasAdded = hashSet.Add(key);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        if (hasAdded)
                        {
                            observer.OnNext(x);
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted
                );
            });
        }

        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source)
        {
            return source.DistinctUntilChanged((IEqualityComparer<T>)null);
        }

        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source, IEqualityComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Observable.Create<T>(observer =>
            {
                var isFirst = true;
                var prevKey = default(T);
                return source.Subscribe(x =>
                {
                    T currentKey;
                    try
                    {
                        currentKey = x;
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
                            sameKey = (comparer == null)
                                ? currentKey.Equals(prevKey)
                                : comparer.Equals(currentKey, prevKey);
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

        public static IObservable<T> DistinctUntilChanged<T, TKey>(this IObservable<T> source, Func<T, TKey> keySelector)
        {
            return DistinctUntilChanged<T, TKey>(source, keySelector, null);
        }

        public static IObservable<T> DistinctUntilChanged<T, TKey>(this IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Observable.Create<T>(observer =>
            {
                var isFirst = true;
                var prevKey = default(TKey);
                return source.Subscribe(x =>
                {
                    TKey currentKey;
                    try
                    {
                        currentKey = keySelector(x);
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
                            sameKey = (comparer == null)
                                ? currentKey.Equals(prevKey)
                                : comparer.Equals(currentKey, prevKey);
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

        public static IObservable<T> IgnoreElements<T>(this IObservable<T> source)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(Stubs.Ignore<T>, observer.OnError, observer.OnCompleted);
            });
        }
    }
}