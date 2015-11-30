using System;
using System.Collections.Generic;
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
            return ToObservable(source, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source, IScheduler scheduler)
        {
            return new ToObservable<T>(source, scheduler);
        }

        public static IObservable<TResult> Cast<TSource, TResult>(this IObservable<TSource> source)
        {
            return new Cast<TSource, TResult>(source);
        }

        /// <summary>
        /// witness is for type inference.
        /// </summary>
        public static IObservable<TResult> Cast<TSource, TResult>(this IObservable<TSource> source, TResult witness)
        {
            return new Cast<TSource, TResult>(source);
        }

        public static IObservable<TResult> OfType<TSource, TResult>(this IObservable<TSource> source)
        {
            return new OfType<TSource, TResult>(source);
        }

        /// <summary>
        /// witness is for type inference.
        /// </summary>
        public static IObservable<TResult> OfType<TSource, TResult>(this IObservable<TSource> source, TResult witness)
        {
            return new OfType<TSource, TResult>(source);
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