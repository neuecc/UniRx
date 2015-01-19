using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UniRx
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

        public static IConnectableObservable<T> Publish<T>(this IObservable<T> source, T initialValue)
        {
            return source.Multicast(new BehaviorSubject<T>(initialValue));
        }

        public static IConnectableObservable<T> PublishLast<T>(this IObservable<T> source)
        {
            return source.Multicast(new AsyncSubject<T>());
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source)
        {
            return source.Multicast(new ReplaySubject<T>());
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize, scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, TimeSpan window)
        {
            return source.Multicast(new ReplaySubject<T>(window));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(window, scheduler));
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, int bufferSize, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<T>(bufferSize, window, scheduler));
        }

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