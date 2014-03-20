using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityRx
{
    public static partial class Observable
    {
        // TODO:Range, Repeat, Return, Unfold, Defer, Empty, Never etc...

        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null) throw new ArgumentNullException("subscribe");

            return new AnonymousObservable<T>(subscribe);
        }

        class AnonymousObservable<T> : IObservable<T>
        {
            readonly Func<IObserver<T>, IDisposable> subscribe;

            public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe)
            {
                this.subscribe = subscribe;
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return subscribe(observer);
            }
        }

        public static IObservable<T> Start<T>(Func<T> function)
        {
            return Start(function, Scheduler.ThreadPool);
        }

        public static IObservable<T> Start<T>(Func<T> function, IScheduler scheduler)
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

            return subject;
        }



        public static IObservable<int> Range(int start, int count)
        {
            return Range(start, count, Scheduler.Immediate); // TODO:Change to CurrentThreadScheduler
        }

        public static IObservable<int> Range(int start, int count, IScheduler scheduler)
        {
            return Observable.Create<int>(observer =>
            {
                return scheduler.Schedule(0, (i, self) =>
                {
                    if (i < count)
                    {
                        observer.OnNext(start + i);
                        self(i + 1);
                    }
                    else
                    {
                        observer.OnCompleted();
                    }
                });
            });
        }

        // TODO:Converter?

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source)
        {
            // TODO:Change to CurrentThread Scheduler?
            return source.ToObservable(Scheduler.Immediate);
        }

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var e = source.GetEnumerator();

                return scheduler.Schedule(self =>
                {

                    bool moveNext;
                    var current = default(T);
                    try
                    {
                        moveNext = e.MoveNext();
                        if (moveNext) current = e.Current;
                    }
                    catch (Exception ex)
                    {
                        e.Dispose();
                        observer.OnError(ex);
                        return;
                    }

                    if (moveNext)
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
            });
        }
    }
}