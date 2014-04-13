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
            // TODO:concurrent count
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
    }
}