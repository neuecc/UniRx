using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UniRx.Operators;

namespace UniRx
{
    // concatenate multiple observable
    // merge, concat, zip...
    public static partial class Observable
    {
        static IEnumerable<IObservable<T>> CombineSources<T>(IObservable<T> first, IObservable<T>[] seconds)
        {
            yield return first;
            for (int i = 0; i < seconds.Length; i++)
            {
                yield return seconds[i];
            }
        }

        public static IObservable<TSource> Concat<TSource>(params IObservable<TSource>[] sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");

            return new ConcatObservable<TSource>(sources);
        }

        public static IObservable<TSource> Concat<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");

            return new ConcatObservable<TSource>(sources);
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<IObservable<TSource>> sources)
        {
            return sources.Merge(maxConcurrent: 1);
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<TSource> first, params IObservable<TSource>[] seconds)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (seconds == null) throw new ArgumentNullException("seconds");

            var concat = first as ConcatObservable<TSource>;
            if (concat != null)
            {
                return concat.Combine(seconds);
            }

            return Concat(CombineSources(first, seconds));
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            return Merge(sources, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources, IScheduler scheduler)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler == Scheduler.CurrentThread);
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent)
        {
            return Merge(sources, maxConcurrent, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        public static IObservable<TSource> Merge<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), maxConcurrent, scheduler == Scheduler.CurrentThread);
        }

        public static IObservable<TSource> Merge<TSource>(params IObservable<TSource>[] sources)
        {
            return Merge(Scheduler.DefaultSchedulers.ConstantTimeOperations, sources);
        }

        public static IObservable<TSource> Merge<TSource>(IScheduler scheduler, params IObservable<TSource>[] sources)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler == Scheduler.CurrentThread);
        }

        public static IObservable<T> Merge<T>(this IObservable<T> first, params IObservable<T>[] seconds)
        {
            return Merge(CombineSources(first, seconds));
        }

        public static IObservable<T> Merge<T>(this IObservable<T> first, IObservable<T> second, IScheduler scheduler)
        {
            return Merge(scheduler, new[] { first, second });
        }

        public static IObservable<T> Merge<T>(this IObservable<IObservable<T>> sources)
        {
            return new MergeObservable<T>(sources, false);
        }

        public static IObservable<T> Merge<T>(this IObservable<IObservable<T>> sources, int maxConcurrent)
        {
            return new MergeObservable<T>(sources, maxConcurrent, false);
        }

        public static IObservable<TResult> Zip<TLeft, TRight, TResult>(this IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
        {
            return new ZipObservable<TLeft, TRight, TResult>(left, right, selector);
        }

        public static IObservable<IList<T>> Zip<T>(this IEnumerable<IObservable<T>> sources)
        {
            return Zip(sources.ToArray());
        }

        public static IObservable<IList<T>> Zip<T>(params IObservable<T>[] sources)
        {
            return new ZipObservable<T>(sources);
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
            }, isRequiredSubscribeOnCurrentThread: left.IsRequiredSubscribeOnCurrentThread() && right.IsRequiredSubscribeOnCurrentThread());
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

            return new WhenAllObservable<T>(sources);
        }

        /// <summary>
        /// <para>Specialized for single async operations like Task.WhenAll, Zip.Take(1).</para>
        /// <para>If sequence is empty, return T[0] array.</para>
        /// </summary>
        public static IObservable<T[]> WhenAll<T>(this IEnumerable<IObservable<T>> sources)
        {
            var array = sources as IObservable<T>[];
            if (array != null) return WhenAll(array);

            return new WhenAllObservable<T>(sources);
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, T value)
        {
            return new StartWithObservable<T>(source, value);
        }

        public static IObservable<T> StartWith<T>(this IObservable<T> source, Func<T> valueFactory)
        {
            return new StartWithObservable<T>(source, valueFactory);
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