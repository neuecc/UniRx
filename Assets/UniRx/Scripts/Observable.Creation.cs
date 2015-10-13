﻿using System;
using System.Collections.Generic;

#if SystemReactive
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using UniRx;

namespace System.Reactive.Linq
#else
using UniRx.Operators;

namespace UniRx
#endif
{
    public static partial class Observable
    {
        /// <summary>
        /// Create anonymous observable. Observer is auto detach when error, completed.
        /// </summary>
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new Create<T>(subscribe);
        }

        /// <summary>
        /// Create anonymous observable. Observer is auto detach when error, completed.
        /// </summary>
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new Create<T>(subscribe, isRequiredSubscribeOnCurrentThread);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted.
        /// </summary>
        public static IObservable<T> Empty<T>()
        {
            return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on specified scheduler.
        /// </summary>
        public static IObservable<T> Empty<T>(IScheduler scheduler)
        {
            return new Empty<T>(scheduler);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted. witness is for type inference.
        /// </summary>
        public static IObservable<T> Empty<T>(T witness)
        {
            return Empty<T>(Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted on specified scheduler. witness is for type inference.
        /// </summary>
        public static IObservable<T> Empty<T>(IScheduler scheduler, T witness)
        {
            return Empty<T>(scheduler);
        }

        /// <summary>
        /// Non-Terminating Observable. It's no returns, never finish.
        /// </summary>
        public static IObservable<T> Never<T>()
        {
            return new Never<T>();
        }

        /// <summary>
        /// Non-Terminating Observable. It's no returns, never finish. witness is for type inference.
        /// </summary>
        public static IObservable<T> Never<T>(T witness)
        {
            return Never<T>();
        }

        /// <summary>
        /// Return single sequence Immediately.
        /// </summary>
        public static IObservable<T> Return<T>(T value)
        {
            return Return<T>(value, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Return single sequence on specified scheduler.
        /// </summary>
        public static IObservable<T> Return<T>(T value, IScheduler scheduler)
        {
            return new Return<T>(value, scheduler);
        }

        /// <summary>
        /// Same as Observable.Return(Unit.Default);
        /// </summary>
        public static IObservable<Unit> ReturnUnit()
        {
            return Return(Unit.Default);
        }

        /// <summary>
        /// Empty Observable. Returns only onError.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error)
        {
            return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only onError. witness if for Type inference.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, T witness)
        {
            return Throw<T>(error, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        /// <summary>
        /// Empty Observable. Returns only onError on specified scheduler.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, IScheduler scheduler)
        {
            return new Throw<T>(error, scheduler);
        }

        /// <summary>
        /// Empty Observable. Returns only onError on specified scheduler. witness if for Type inference.
        /// </summary>
        public static IObservable<T> Throw<T>(Exception error, IScheduler scheduler, T witness)
        {
            return Throw<T>(error, scheduler);
        }

        public static IObservable<int> Range(int start, int count)
        {
            return Range(start, count, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<int> Range(int start, int count, IScheduler scheduler)
        {
            return new Range(start, count, scheduler);
        }

        public static IObservable<T> Repeat<T>(T value)
        {
            return Repeat(value, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> Repeat<T>(T value, IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return new Repeat<T>(value, null, scheduler);
        }

        public static IObservable<T> Repeat<T>(T value, int repeatCount)
        {
            return Repeat(value, repeatCount, Scheduler.DefaultSchedulers.Iteration);
        }

        public static IObservable<T> Repeat<T>(T value, int repeatCount, IScheduler scheduler)
        {
            if (repeatCount < 0) throw new ArgumentOutOfRangeException("repeatCount");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return new Repeat<T>(value, repeatCount, scheduler);
        }

        public static IObservable<T> Repeat<T>(this IObservable<T> source)
        {
            return RepeatInfinite(source).Concat();
        }

        static IEnumerable<IObservable<T>> RepeatInfinite<T>(IObservable<T> source)
        {
            while (true)
            {
                yield return source;
            }
        }

        /// <summary>
        /// Same as Repeat() but if arriving contiguous "OnComplete" Repeat stops.
        /// </summary>
        public static IObservable<T> RepeatSafe<T>(this IObservable<T> source)
        {
            return new RepeatSafe<T>(RepeatInfinite(source), source.IsRequiredSubscribeOnCurrentThread());
        }

        public static IObservable<T> Defer<T>(Func<IObservable<T>> observableFactory)
        {
            return new Defer<T>(observableFactory);
        }

        public static IObservable<T> Start<T>(Func<T> function)
        {
            return Start(function, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<T> Start<T>(Func<T> function, IScheduler scheduler)
        {
            return ToAsync(function, scheduler)();
        }

        public static IObservable<Unit> Start(Action action)
        {
            return Start(action, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static IObservable<Unit> Start(Action action, IScheduler scheduler)
        {
            return ToAsync(action, scheduler)();
        }

        public static Func<IObservable<T>> ToAsync<T>(Func<T> function)
        {
            return ToAsync(function, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static Func<IObservable<T>> ToAsync<T>(Func<T> function, IScheduler scheduler)
        {
            return () =>
            {
                var subject = new AsyncSubject<T>();

                scheduler.Schedule(() =>
                {
                    var result = default(T);
                    try
                    {
                        result = function();
                    }
                    catch (Exception exception)
                    {
                        subject.OnError(exception);
                        return;
                    }
                    subject.OnNext(result);
                    subject.OnCompleted();
                });

                return subject.AsObservable();
            };
        }

        public static Func<IObservable<Unit>> ToAsync(Action action)
        {
            return ToAsync(action, Scheduler.DefaultSchedulers.AsyncConversions);
        }

        public static Func<IObservable<Unit>> ToAsync(Action action, IScheduler scheduler)
        {
            return () =>
            {
                var subject = new AsyncSubject<Unit>();

                scheduler.Schedule(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        subject.OnError(exception);
                        return;
                    }
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });

                return subject.AsObservable();
            };
        }
    }
}