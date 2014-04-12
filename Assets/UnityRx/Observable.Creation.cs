using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityRx
{
    public static partial class Observable
    {
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
                var subscription = new SingleAssignmentDisposable();
                var safeObserver = Observer.Create<T>(observer.OnNext, observer.OnError, observer.OnCompleted, subscription);
                subscription.Disposable = subscribe(safeObserver);

                return subscription;
            }
        }

        /// <summary>
        /// Empty Observable. Returns only OnCompleted
        /// </summary>
        public static IObservable<Unit> Empty()
        {
            return Observable.Create<Unit>(observer =>
            {
                observer.OnCompleted();
                return Disposable.Empty;
            });
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
    }
}