using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityRx
{
    public static partial class Observable
    {
        // TODO:Range, Repeat, Return, Unfold, etc...

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




        // TODO:need scheduler
        public static IObservable<int> Range(int start, int count)
        {
            return Observable.Create<int>(observer =>
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        observer.OnNext(start++);
                    }
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                // TODO:Cancellable!
                return Disposable.Empty;
            });
        }

        // TODO:Converter?

        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source)
        {
            throw new NotImplementedException();
        }
    }
}