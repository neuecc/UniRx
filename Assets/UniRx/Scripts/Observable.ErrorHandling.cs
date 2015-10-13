﻿using System;
using System.Collections;
using System.Collections.Generic;

#if SystemReactive
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using UniRx;

namespace System.Reactive.Linq
#else
namespace UniRx
#endif
{
    public static partial class Observable
    {
        public static IObservable<T> Finally<T>(this IObservable<T> source, Action finallyAction)
        {
            return Observable.Create<T>(observer =>
            {
                IDisposable subscription;
                try
                {
                    subscription = source.Subscribe(observer);
                }
                catch
                {
                    // This behaviour is not same as .NET Official Rx
                    finallyAction();
                    throw;
                }

                return Disposable.Create(() =>
                {
                    try
                    {
                        subscription.Dispose();
                    }
                    finally
                    {
                        finallyAction();
                    }
                });
            });
        }

        public static IObservable<T> Catch<T, TException>(this IObservable<T> source, Func<TException, IObservable<T>> errorHandler)
            where TException : Exception
        {
            return Observable.Create<T>(observer =>
            {
                var serialDisposable = new SerialDisposable();

                var rootDisposable = new SingleAssignmentDisposable();
                serialDisposable.Disposable = rootDisposable;

                rootDisposable.Disposable = source.Subscribe(observer.OnNext,
                    exception =>
                    {
                        var e = exception as TException;
                        if (e != null)
                        {
                            IObservable<T> next;
                            try
                            {
                                if (errorHandler == Stubs.CatchIgnore<T>)
                                {
                                    next = Observable.Empty<T>(); // for avoid iOS AOT
                                }
                                else
                                {
                                    next = errorHandler(e);
                                }
                            }
                            catch (Exception ex)
                            {
                                observer.OnError(ex);
                                return;
                            }

                            var d = new SingleAssignmentDisposable();
                            serialDisposable.Disposable = d;
                            d.Disposable = next.Subscribe(observer);
                        }
                        else
                        {
                            observer.OnError(exception);
                        }
                    }, observer.OnCompleted);

                return serialDisposable;
            });
        }

        public static IObservable<TSource> Catch<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            // this code is borrowed from RxOfficial(rx.codeplex.com) and modified
            return Observable.Create<TSource>(observer =>
            {
                var gate = new object();
                var isDisposed = false;
                var e = sources.AsSafeEnumerable().GetEnumerator();
                var subscription = new SerialDisposable();
                var lastException = default(Exception);

                var cancelable = Scheduler.DefaultSchedulers.TailRecursion.Schedule(self =>
                {
                    lock (gate)
                    {
                        var current = default(IObservable<TSource>);
                        var hasNext = false;
                        var ex = default(Exception);

                        if (!isDisposed)
                        {
                            try
                            {
                                hasNext = e.MoveNext();
                                if (hasNext)
                                    current = e.Current;
                                else
                                    e.Dispose();
                            }
                            catch (Exception exception)
                            {
                                ex = exception;
                                e.Dispose();
                            }
                        }
                        else
                            return;

                        if (ex != null)
                        {
                            observer.OnError(ex);
                            return;
                        }

                        if (!hasNext)
                        {
                            if (lastException != null)
                                observer.OnError(lastException);
                            else
                                observer.OnCompleted();
                            return;
                        }

                        var d = new SingleAssignmentDisposable();
                        subscription.Disposable = d;
                        d.Disposable = current.Subscribe(observer.OnNext, exception =>
                        {
                            lastException = exception;
                            self();
                        }, observer.OnCompleted);
                    }
                });

                return new CompositeDisposable(subscription, cancelable, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        e.Dispose();
                        isDisposed = true;
                    }
                }));
            });
        }

        /// <summary>Catch exception and return Observable.Empty.</summary>
        public static IObservable<TSource> CatchIgnore<TSource>(this IObservable<TSource> source)
        {
            return source.Catch<TSource, Exception>(Stubs.CatchIgnore<TSource>);
        }

        /// <summary>Catch exception and return Observable.Empty.</summary>
        public static IObservable<TSource> CatchIgnore<TSource, TException>(this IObservable<TSource> source, Action<TException> errorAction)
            where TException : Exception
        {
            var result = source.Catch((TException ex) =>
            {
                errorAction(ex);
                return Observable.Empty<TSource>();
            });
            return result;
        }

        public static IObservable<TSource> Retry<TSource>(this IObservable<TSource> source)
        {
            return RepeatInfinite(source).Catch();
        }

        public static IObservable<TSource> Retry<TSource>(this IObservable<TSource> source, int retryCount)
        {
            return System.Linq.Enumerable.Repeat(source, retryCount).Catch();
        }

        /// <summary>
        /// <para>Repeats the source observable sequence until it successfully terminates.</para>
        /// <para>This is same as Retry().</para>
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource>(
            this IObservable<TSource> source)
        {
            var result = source.Retry();
            return result;
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, TimeSpan.Zero);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, TimeSpan delay)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, int.MaxValue, delay);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, retryCount, TimeSpan.Zero);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay)
            where TException : Exception
        {
            return source.OnErrorRetry(onError, retryCount, delay, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time(work on delayScheduler) during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay, IScheduler delayScheduler)
            where TException : Exception
        {
            var result = Observable.Defer(() =>
            {
                var dueTime = (delay.Ticks < 0) ? TimeSpan.Zero : delay;
                var count = 0;

                IObservable<TSource> self = null;
                self = source.Catch((TException ex) =>
                {
                    onError(ex);

                    return (++count < retryCount)
                        ? (dueTime == TimeSpan.Zero)
                            ? self.SubscribeOn(Scheduler.CurrentThread)
                            : self.DelaySubscription(dueTime, delayScheduler).SubscribeOn(Scheduler.CurrentThread)
                        : Observable.Throw<TSource>(ex);
                });
                return self;
            });

            return result;
        }
    }
}