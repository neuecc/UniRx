using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityRx
{
    public static partial class Observable
    {
        /// <summary>
        /// Convert to awaitable IEnumerator.
        /// </summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source)
        {
            var running = true;
            source.Subscribe(
                ex => { running = false; },
                () => { running = false; });

            while (running)
            {
                yield return null;
            }
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return source.ToObservable(Scheduler.CurrentThread);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                IEnumerator<T> e;
                try
                {
                    e = source.GetEnumerator();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return Disposable.Empty;
                }

                var flag = new BooleanDisposable();

                scheduler.Schedule(self =>
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
    }
}