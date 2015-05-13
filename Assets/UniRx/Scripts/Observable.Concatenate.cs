using System;
using System.Collections.Generic;
using System.Linq; // memo, remove LINQ(for avoid AOT)
using System.Text;

namespace UniRx
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
            return sources.Merge(maxConcurrent: 1);
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
                var e = sources.AsSafeEnumerable().GetEnumerator();
                var subscription = new SerialDisposable();
                var gate = new object();

                var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(self =>
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

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            return Merge(sources, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources, IScheduler scheduler)
        {
            return Merge(sources.ToObservable(scheduler));
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent)
        {
            return Merge(sources, maxConcurrent, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler)
        {
            return Merge(sources.ToObservable(scheduler), maxConcurrent);
        }

        public static IObservable<TSource> Merge<TSource>(params IObservable<TSource>[] sources)
        {
            return Merge(Scheduler.DefaultSchedulers.ConstantTimeOperations, sources);
        }

        public static IObservable<TSource> Merge<TSource>(IScheduler scheduler, params IObservable<TSource>[] sources)
        {
            return Merge(sources.ToObservable(scheduler));
        }

        public static IObservable<T> Merge<T>(this IObservable<T> first, IObservable<T> second)
        {
            return Merge(new[] { first, second });
        }

        public static IObservable<T> Merge<T>(this IObservable<T> first, IObservable<T> second, IScheduler scheduler)
        {
            return Merge(scheduler, new[] { first, second });
        }

        public static IObservable<T> Merge<T>(this IObservable<IObservable<T>> sources)
        {
            // this code is borrwed from RxOfficial(rx.codeplex.com)
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                var isStopped = false;
                var m = new SingleAssignmentDisposable();
                var group = new CompositeDisposable() { m };

                m.Disposable = sources.Subscribe(
                    innerSource =>
                    {
                        var innerSubscription = new SingleAssignmentDisposable();
                        group.Add(innerSubscription);
                        innerSubscription.Disposable = innerSource.Subscribe(
                            x =>
                            {
                                lock (gate)
                                    observer.OnNext(x);
                            },
                            exception =>
                            {
                                lock (gate)
                                    observer.OnError(exception);
                            },
                            () =>
                            {
                                group.Remove(innerSubscription);   // modification MUST occur before subsequent check
                                if (isStopped && group.Count == 1) // isStopped must be checked before group Count to ensure outer is not creating more groups
                                    lock (gate)
                                        observer.OnCompleted();
                            });
                    },
                    exception =>
                    {
                        lock (gate)
                            observer.OnError(exception);
                    },
                    () =>
                    {
                        isStopped = true;     // modification MUST occur before subsequent check
                        if (group.Count == 1)
                            lock (gate)
                                observer.OnCompleted();
                    });

                return group;
            });
        }

        public static IObservable<T> Merge<T>(this IObservable<IObservable<T>> sources, int maxConcurrent)
        {
            // this code is borrwed from RxOfficial(rx.codeplex.com)
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                var q = new Queue<IObservable<T>>();
                var isStopped = false;
                var group = new CompositeDisposable();
                var activeCount = 0;

                var subscribe = default(Action<IObservable<T>>);
                subscribe = xs =>
                {
                    var subscription = new SingleAssignmentDisposable();
                    group.Add(subscription);
                    subscription.Disposable = xs.Subscribe(
                        x =>
                        {
                            lock (gate)
                                observer.OnNext(x);
                        },
                        exception =>
                        {
                            lock (gate)
                                observer.OnError(exception);
                        },
                        () =>
                        {
                            group.Remove(subscription);
                            lock (gate)
                            {
                                if (q.Count > 0)
                                {
                                    var s = q.Dequeue();
                                    subscribe(s);
                                }
                                else
                                {
                                    activeCount--;
                                    if (isStopped && activeCount == 0)
                                        observer.OnCompleted();
                                }
                            }
                        });
                };

                group.Add(sources.Subscribe(
                    innerSource =>
                    {
                        lock (gate)
                        {
                            if (activeCount < maxConcurrent)
                            {
                                activeCount++;
                                subscribe(innerSource);
                            }
                            else
                                q.Enqueue(innerSource);
                        }
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
                            if (activeCount == 0)
                                observer.OnCompleted();
                        }
                    }));

                return group;
            });
        }

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

        public static IObservable<IList<T>> Zip<T>(this IEnumerable<IObservable<T>> sources)
        {
            return Zip(sources.ToArray());
        }

        public static IObservable<IList<T>> Zip<T>(params IObservable<T>[] sources)
        {
            return Observable.Create<IList<T>>(observer =>
            {
                var gate = new object();
                var length = sources.Length;
                var queues = new Queue<T>[length];
                for (int i = 0; i < length; i++)
                {
                    queues[i] = new Queue<T>();
                }
                var isDone = new bool[length];

                Action<int> dequeue = index =>
                {
                    lock (gate)
                    {
                        if (queues.All(x => x.Count > 0))
                        {
                            var result = queues.Select(x => x.Dequeue()).ToList();
                            observer.OnNext(result);
                            return;
                        }

                        if (isDone.Where((x, i) => i != index).All(x => x))
                        {
                            observer.OnCompleted();
                            return;
                        }
                    }
                };

                var subscriptions = sources
                    .Select((source, index) =>
                    {
                        var d = new SingleAssignmentDisposable();

                        d.Disposable = source.Subscribe(x =>
                        {
                            lock (gate)
                            {
                                queues[index].Enqueue(x);
                                dequeue(index);
                            }
                        }, ex =>
                        {
                            lock (gate)
                            {
                                observer.OnError(ex);
                            }
                        }, () =>
                        {
                            lock (gate)
                            {
                                isDone[index] = true;
                                if (isDone.All(x => x))
                                {
                                    observer.OnCompleted();
                                }
                                else
                                {
                                    d.Dispose();
                                }
                            }
                        });

                        return d;
                    })
                    .ToArray();

                return new CompositeDisposable(subscriptions) { Disposable.Create(()=>
                {
                    lock(gate)
                    {
                        foreach(var item in queues)
                        {
                            item.Clear();
                        }
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

        public static IObservable<IList<T>> CombineLatest<T>(this IEnumerable<IObservable<T>> sources)
        {
            return CombineLatest(sources.ToArray());
        }

        public static IObservable<IList<TSource>> CombineLatest<TSource>(params IObservable<TSource>[] sources)
        {
            // this code is borrwed from RxOfficial(rx.codeplex.com)
            return Observable.Create<IList<TSource>>(observer =>
            {
                var srcs = sources.ToArray();

                var N = srcs.Length;

                var hasValue = new bool[N];
                var hasValueAll = false;

                var values = new List<TSource>(N);
                for (int i = 0; i < N; i++)
                    values.Add(default(TSource));

                var isDone = new bool[N];

                var next = new Action<int>(i =>
                {
                    hasValue[i] = true;

                    if (hasValueAll || (hasValueAll = hasValue.All(x => x)))
                    {
                        var res = values.ToList();
                        observer.OnNext(res);
                    }
                    else if (isDone.Where((x, j) => j != i).All(x => x))
                    {
                        observer.OnCompleted();
                        return;
                    }
                });

                var done = new Action<int>(i =>
                {
                    isDone[i] = true;

                    if (isDone.All(x => x))
                    {
                        observer.OnCompleted();
                        return;
                    }
                });

                var subscriptions = new SingleAssignmentDisposable[N];

                var gate = new object();

                for (int i = 0; i < N; i++)
                {
                    var j = i;
                    subscriptions[j] = new SingleAssignmentDisposable
                    {
                        Disposable = srcs[j].Synchronize(gate).Subscribe(
                            x =>
                            {
                                values[j] = x;
                                next(j);
                            },
                            observer.OnError,
                            () =>
                            {
                                done(j);
                            }
                        )
                    };
                }

                return new CompositeDisposable(subscriptions);
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

        /// <summary>
        /// <para>Specialized for single async operations like Task.WhenAll, Zip.Take(1).</para>
        /// <para>If sequence is empty, return T[0] array.</para>
        /// </summary>
        public static IObservable<T[]> WhenAll<T>(params IObservable<T>[] sources)
        {
            if (sources.Length == 0) return Observable.Return(new T[0]);

            return Observable.Create<T[]>(observer =>
            {
                var gate = new object();
                var length = sources.Length;
                var completedCount = 0;
                var values = new T[length];

                var subscriptions = new IDisposable[length];
                for (int index = 0; index < length; index++)
                {
                    var source = sources[index];
                    var capturedIndex = index;
                    var d = new SingleAssignmentDisposable();
                    d.Disposable = source.Subscribe(x =>
                    {
                        lock (gate)
                        {
                            values[capturedIndex] = x;
                        }
                    }, ex =>
                    {
                        lock (gate)
                        {
                            observer.OnError(ex);
                        }
                    }, () =>
                    {
                        lock (gate)
                        {
                            completedCount++;
                            if (completedCount == length)
                            {
                                observer.OnNext(values);
                                observer.OnCompleted();
                            }
                        }
                    });
                    subscriptions[index] = d;
                }

                return new CompositeDisposable(subscriptions);
            });
        }

        /// <summary>
        /// <para>Specialized for single async operations like Task.WhenAll, Zip.Take(1).</para>
        /// <para>If sequence is empty, return T[0] array.</para>
        /// </summary>
        public static IObservable<T[]> WhenAll<T>(this IEnumerable<IObservable<T>> sources)
        {
            var array = sources as IObservable<T>[];
            if (array != null) return WhenAll(array);

            return Observable.Create<T[]>(observer =>
            {
                var _sources = sources as IList<IObservable<T>>;
                if (_sources == null)
                {
                    _sources = new List<IObservable<T>>();
                    foreach (var item in sources)
                    {
                        _sources.Add(item);
                    }
                }

                var gate = new object();
                var length = _sources.Count;
                var completedCount = 0;
                var values = new T[length];

                if (length == 0)
                {
                    observer.OnNext(values);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                var subscriptions = new IDisposable[length];
                for (int index = 0; index < length; index++)
                {
                    var source = _sources[index];
                    var capturedIndex = index;
                    var d = new SingleAssignmentDisposable();
                    d.Disposable = source.Subscribe(x =>
                    {
                        lock (gate)
                        {
                            values[capturedIndex] = x;
                        }
                    }, ex =>
                    {
                        lock (gate)
                        {
                            observer.OnError(ex);
                        }
                    }, () =>
                    {
                        lock (gate)
                        {
                            completedCount++;
                            if (completedCount == length)
                            {
                                observer.OnNext(values);
                                observer.OnCompleted();
                            }
                        }
                    });
                    subscriptions[index] = d;
                }

                return new CompositeDisposable(subscriptions);
            });

        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, T value)
        {
            // optimized path
            return Observable.Create<T>(observer =>
            {
                observer.OnNext(value);
                return source.Subscribe(observer);
            });
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, Func<T> valueFactory)
        {
            return Observable.Create<T>(observer =>
            {
                T value;
                try
                {
                    value = valueFactory();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return Disposable.Empty;
                }
                observer.OnNext(value);
                return source.Subscribe(observer);
            });
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, params T[] values)
        {
            return StartWith(source, Scheduler.DefaultSchedulers.ConstantTimeOperations, values);
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, IEnumerable<T> values)
        {
            return StartWith(source, Scheduler.DefaultSchedulers.ConstantTimeOperations, values);
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, IScheduler scheduler, T value)
        {
            return Observable.Return(value, scheduler).Concat(source);
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, IScheduler scheduler, IEnumerable<T> values)
        {
            var array = values as T[];
            if (array == null)
            {
                array = values.ToArray();
            }

            return StartWith(source, scheduler, array);
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, IScheduler scheduler, params T[] values)
        {
            return values.ToObservable(scheduler).Concat(source);
        }
    }
}