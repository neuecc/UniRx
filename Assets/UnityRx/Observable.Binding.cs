using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnityRx
{
    public static partial class Observable
    {
        public static IConnectableObservable<T> Multicast<T>(this IObservable<T> source, ISubject<T> subject)
        {
            return new ConnectableObservable<T>(source, subject);
        }

        public static IConnectableObservable<T> Publish<T>(this IObservable<T> source)
        {
            return source.Multicast(new Subject<T>());
        }

        // not yet
        //public static IConnectableObservable<T> Publish<T>(this IObservable<T> source, T initialValue)
        //{
        //    return source.Multicast(new BehaviourSubject<T>(initialValue));
        //}

        public static IConnectableObservable<T> PublishLast<T>(this IObservable<T> source)
        {
            return source.Multicast(new AsyncSubject<T>());
        }

        // not yet
        //public static IConnectableObservable<T> Replay<T>(this IObservable<T> source)
        //{
        //    return source.Multicast(new ReplaySubject<T>());
        //}

        public static IObservable<T> RefCount<T>(this IConnectableObservable<T> source)
        {
            var connection = default(IDisposable);
            var gate = new object();
            var refCount = 0;

            return Observable.Create<T>(observer =>
            {
                var subscription = source.Subscribe(observer);

                lock (gate)
                {
                    if (++refCount == 1)
                    {
                        connection = source.Connect();
                    }
                }

                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                    lock (gate)
                    {
                        if (--refCount == 0)
                        {
                            connection.Dispose(); // connection isn't null.
                        }
                    }
                });
            });
        }
    }
}