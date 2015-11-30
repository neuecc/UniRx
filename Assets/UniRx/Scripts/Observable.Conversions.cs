using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // in future, should remove LINQ(for avoid AOT)
using UniRx.Operators;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<T> AsObservable<T>(this IObservable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            // optimize, don't double wrap
            if (source is UniRx.Operators.AsObservable<T>)
            {
                return source;
            }

            return new AsObservable<T>(source);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return source.ToObservable(Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                IEnumerator<T> e;
                try
                {
                    e = source.AsSafeEnumerable().GetEnumerator();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return Disposable.Empty;
                }

                var flag = new SingleAssignmentDisposable();

                flag.Disposable = scheduler.Schedule(self =>
                {
                    if (flag.IsDisposed)
                    {
                        e.Dispose();
                        return;
                    }

                    bool hasNext;
                    var current = default(T);
                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext) current = e.Current;
                    }
                    catch (Exception ex)
                    {
                        e.Dispose();
                        observer.OnError(ex);
                        return;
                    }

                    if (hasNext)
                    {
                        observer.OnNext(current);
                        self();
                    }
                    else
                    {
                        e.Dispose();
                        observer.OnCompleted();
                    }
                });

                return flag;
            });
        }

        public static IObservable<TResult> Cast<TSource, TResult>(this IObservable<TSource> source)
        {
            return source.Select(x => (TResult)(object)x);
        }

        /// <summary>
        /// witness is for type inference.
        /// </summary>
        public static IObservable<TResult> Cast<TSource, TResult>(this IObservable<TSource> source, TResult witness)
        {
            return source.Select(x => (TResult)(object)x);
        }

        public static IObservable<TResult> OfType<TSource, TResult>(this IObservable<TSource> source)
        {
            return source.Where(x => x is TResult).Select(x => (TResult)(object)x);
        }

        /// <summary>
        /// witness is for type inference.
        /// </summary>
        public static IObservable<TResult> OfType<TSource, TResult>(this IObservable<TSource> source, TResult witness)
        {
            return source.Where(x => x is TResult).Select(x => (TResult)(object)x);
        }

        /// <summary>
        /// Converting .Select(_ => Unit.Default) sequence.
        /// </summary>
        public static IObservable<Unit> AsUnitObservable<T>(this IObservable<T> source)
        {
            return new AsUnitObservable<T>(source);
        }
    }
}