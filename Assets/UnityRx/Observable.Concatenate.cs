using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    // concatenate multiple observable
    // merge, concat, zip...
    public static partial class Observable
    {
        public static IObservable<TSource> Concat<TSource>(params IObservable<TSource>[] sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");

            return ConcatCore(sources);
        }

        public static IObservable<TSource> Concat<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");

            return ConcatCore(sources);
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<IObservable<TSource>> sources)
        {
            // needs concurrent count = 1
            return sources.Merge();
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<TSource> first, IObservable<TSource> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return ConcatCore(new[] { first, second });
        }

        static IObservable<T> ConcatCore<T>(IEnumerable<IObservable<T>> sources)
        {
            return Observable.Create<T>(observer =>
            {
                var isDisposed = false;
                var e = sources.GetEnumerator();
                var subscription = new SerialDisposable();
                var gate = new object();

                var schedule = Scheduler.Immediate.Schedule(self =>
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
                        d.Disposable = source.Subscribe(observer.OnNext, observer.OnError, self); // OnCompleted, run self
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

        // needs multiple zip

        public static IObservable<TResult> Zip<TLeft, TRight, TResult>(this IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
        {
            return Observable.Create<TResult>(observer =>
            {
                var gate = new object();
                var leftQ = new Queue<TLeft>();
                bool leftCompleted = false;
                var rightQ = new Queue<TRight>();
                var rightCompleted = false;

                Action dequeue = () =>
                {
                    TLeft lv;
                    TRight rv;
                    TResult v;
                    if (leftQ.Count != 0 && rightQ.Count != 0)
                    {
                        lv = leftQ.Dequeue();
                        rv = rightQ.Dequeue();
                    }
                    else if (leftCompleted || rightCompleted)
                    {
                        observer.OnCompleted();
                        return;
                    }
                    else
                    {
                        return;
                    }
                    try
                    {
                        v = selector(lv, rv);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnNext(v);
                };

                var lsubscription = left.Synchronize(gate).Subscribe(x =>
                {
                    leftQ.Enqueue(x);
                    dequeue();
                }, observer.OnError, () =>
                {
                    leftCompleted = true;
                    if (rightCompleted) observer.OnCompleted();
                });


                var rsubscription = right.Synchronize(gate).Subscribe(x =>
                {
                    rightQ.Enqueue(x);
                    dequeue();
                }, observer.OnError, () =>
                {
                    rightCompleted = true;
                    if (leftCompleted) observer.OnCompleted();
                });

                return new CompositeDisposable { lsubscription, rsubscription, Disposable.Create(()=>
                {
                    lock(gate)
                    {
                        leftQ.Clear();
                        rightQ.Clear();
                    }
                })};
            });
        }

        public static IObservable<TResult> CombineLatest<TLeft, TRight, TResult>(this IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
        {
            return Observable.Create<TResult>(observer =>
            {
                var gate = new object();

                var leftValue = default(TLeft);
                var leftStarted = false;
                bool leftCompleted = false;

                var rightValue = default(TRight);
                var rightStarted = false;
                var rightCompleted = false;

                Action run = () =>
                {
                    if ((leftCompleted && !leftStarted) || (rightCompleted && !rightStarted))
                    {
                        observer.OnCompleted();
                        return;
                    }
                    else if (!(leftStarted && rightStarted))
                    {
                        return;
                    }

                    TResult v;
                    try
                    {
                        v = selector(leftValue, rightValue);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnNext(v);
                };

                var lsubscription = left.Synchronize(gate).Subscribe(x =>
                {
                    leftStarted = true;
                    leftValue = x;
                    run();
                }, observer.OnError, () =>
                {
                    leftCompleted = true;
                    if (rightCompleted) observer.OnCompleted();
                });

                var rsubscription = right.Synchronize(gate).Subscribe(x =>
                {
                    rightStarted = true;
                    rightValue = x;
                    run();
                }, observer.OnError, () =>
                {
                    rightCompleted = true;
                    if (leftCompleted) observer.OnCompleted();
                });

                return new CompositeDisposable { lsubscription, rsubscription };
            });
        }

        public static IObservable<T> Switch<T>(this IObservable<IObservable<T>> sources)
        {
            // this code is borrwed from RxOfficial(rx.codeplex.com)
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                var innerSubscription = new SerialDisposable();
                var isStopped = false;
                var latest = 0UL;
                var hasLatest = false;
                var subscription = sources.Subscribe(
                    innerSource =>
                    {
                        var id = default(ulong);
                        lock (gate)
                        {
                            id = unchecked(++latest);
                            hasLatest = true;
                        }

                        var d = new SingleAssignmentDisposable();
                        innerSubscription.Disposable = d;
                        d.Disposable = innerSource.Subscribe(
                        x =>
                        {
                            lock (gate)
                            {
                                if (latest == id)
                                    observer.OnNext(x);
                            }
                        },
                        exception =>
                        {
                            lock (gate)
                            {
                                if (latest == id)
                                    observer.OnError(exception);
                            }
                        },
                        () =>
                        {
                            lock (gate)
                            {
                                if (latest == id)
                                {
                                    hasLatest = false;

                                    if (isStopped)
                                        observer.OnCompleted();
                                }
                            }
                        });
                    },
                    exception =>
                    {
                        lock (gate)
                            observer.OnError(exception);
                    },
                    () =>
                    {
                        lock (gate)
                        {
                            isStopped = true;
                            if (!hasLatest)
                                observer.OnCompleted();
                        }
                    });

                return new CompositeDisposable(subscription, innerSubscription);
            });
        }
    }
}