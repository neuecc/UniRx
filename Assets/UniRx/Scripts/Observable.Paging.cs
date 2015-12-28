using System;
using System.Collections.Generic;
using System.Text;
using UniRx.Operators;

namespace UniRx
{
    // Take, Skip, etc..
    public static partial class Observable
    {
        public static IObservable<T> Take<T>(this IObservable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            if (count == 0) return Empty<T>();

            // optimize .Take(count).Take(count)
            var take = source as TakeObservable<T>;
            if (take != null && take.scheduler == null)
            {
                return take.Combine(count);
            }

            return new TakeObservable<T>(source, count);
        }

        public static IObservable<T> Take<T>(this IObservable<T> source, TimeSpan duration)
        {
            return Take(source, duration, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Take<T>(this IObservable<T> source, TimeSpan duration, IScheduler scheduler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            // optimize .Take(duration).Take(duration)
            var take = source as TakeObservable<T>;
            if (take != null && take.scheduler == scheduler)
            {
                return take.Combine(duration);
            }

            return new TakeObservable<T>(source, duration, scheduler);
        }

        public static IObservable<T> TakeWhile<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new TakeWhileObservable<T>(source, predicate);
        }

        public static IObservable<T> TakeWhile<T>(this IObservable<T> source, Func<T, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return new TakeWhileObservable<T>(source, predicate);
        }

        public static IObservable<T> TakeUntil<T, TOther>(this IObservable<T> source, IObservable<TOther> other)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (other == null) throw new ArgumentNullException("other");

            return new TakeUntilObservable<T, TOther>(source, other);
        }

        public static IObservable<T> Skip<T>(this IObservable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            // optimize .Skip(count).Skip(count)
            var skip = source as SkipObservable<T>;
            if (skip != null && skip.scheduler == null)
            {
                return skip.Combine(count);
            }

            return new SkipObservable<T>(source, count);
        }

        public static IObservable<T> Skip<T>(this IObservable<T> source, TimeSpan duration)
        {
            return Skip(source, duration, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Skip<T>(this IObservable<T> source, TimeSpan duration, IScheduler scheduler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            // optimize .Skip(duration).Skip(duration)
            var skip = source as SkipObservable<T>;
            if (skip != null && skip.scheduler == scheduler)
            {
                return skip.Combine(duration);
            }

            return new SkipObservable<T>(source, duration, scheduler);
        }

        public static IObservable<T> SkipWhile<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new SkipWhileObservable<T>(source, predicate);
        }

        public static IObservable<T> SkipWhile<T>(this IObservable<T> source, Func<T, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return new SkipWhileObservable<T>(source, predicate);
        }

        public static IObservable<T> SkipUntil<T, TOther>(this IObservable<T> source, IObservable<TOther> other)
        {
            return new SkipUntilObservable<T, TOther>(source, other);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count <= 0");

            return new BufferObservable<T>(source, count, 0);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, int count, int skip)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count <= 0");
            if (skip <= 0) throw new ArgumentOutOfRangeException("skip <= 0");

            return new BufferObservable<T>(source, count, skip);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan)
        {
            return Buffer(source, timeSpan, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new BufferObservable<T>(source, timeSpan, timeSpan, scheduler);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan, int count)
        {
            return Buffer(source, timeSpan, count, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan, int count, IScheduler scheduler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count <= 0");

            return new BufferObservable<T>(source, timeSpan, count, scheduler);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift)
        {
            return new BufferObservable<T>(source, timeSpan, timeShift, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new BufferObservable<T>(source, timeSpan, timeShift, scheduler);
        }

        public static IObservable<IList<TSource>> Buffer<TSource, TWindowBoundary>(this IObservable<TSource> source, IObservable<TWindowBoundary> windowBoundaries)
        {
            return new BufferObservable<TSource, TWindowBoundary>(source, windowBoundaries);
        }

        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<Pair<T>> Pairwise<T>(this IObservable<T> source)
        {
            return new PairwiseObservable<T>(source);
        }

        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<TR> Pairwise<T, TR>(this IObservable<T> source, Func<T, T, TR> selector)
        {
            return new PairwiseObservable<T, TR>(source, selector);
        }

        // first, last, single

        public static IObservable<T> Last<T>(this IObservable<T> source)
        {
            return new LastObservable<T>(source, false);
        }
        public static IObservable<T> Last<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new LastObservable<T>(source, predicate, false);
        }

        public static IObservable<T> LastOrDefault<T>(this IObservable<T> source)
        {
            return new LastObservable<T>(source, true);
        }

        public static IObservable<T> LastOrDefault<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new LastObservable<T>(source, predicate, true);
        }

        public static IObservable<T> First<T>(this IObservable<T> source)
        {
            return new FirstObservable<T>(source, false);
        }
        public static IObservable<T> First<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new FirstObservable<T>(source, predicate, false);
        }

        public static IObservable<T> FirstOrDefault<T>(this IObservable<T> source)
        {
            return new FirstObservable<T>(source, true);
        }

        public static IObservable<T> FirstOrDefault<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new FirstObservable<T>(source, predicate, true);
        }

        public static IObservable<T> Single<T>(this IObservable<T> source)
        {
            return new SingleObservable<T>(source, false);
        }
        public static IObservable<T> Single<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new SingleObservable<T>(source, predicate, false);
        }

        public static IObservable<T> SingleOrDefault<T>(this IObservable<T> source)
        {
            return new SingleObservable<T>(source, true);
        }

        public static IObservable<T> SingleOrDefault<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return new SingleObservable<T>(source, predicate, true);
        }
    }
}