using System;
using System.Collections;
using System.Threading;

namespace UnityRx
{
    public interface IObserver<in T>
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(T value);
    }

    public static class Observer
    {
        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return Create<T>(onNext, onError, onCompleted, Disposable.Empty);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted, IDisposable disposable)
        {
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onError == null) throw new ArgumentNullException("onError");
            if (onCompleted == null) throw new ArgumentNullException("onCompleted");
            if (disposable == null) throw new ArgumentNullException("disposable");

            return new AnonymousObserver<T>(onNext, onError, onCompleted, disposable);
        }

        class AnonymousObserver<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;
            readonly IDisposable disposable;

            int isStopped = 0;

            public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted, IDisposable disposable)
            {
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
                this.disposable = disposable;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    var noError = false;
                    try
                    {
                        onNext(value);
                        noError = true;
                    }
                    finally
                    {
                        if (!noError)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        onError(error);
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        onCompleted();
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }

    public static partial class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            return source.Subscribe(Observer.Create<T>(_ => { }, e => { throw e; }, () => { }));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(Observer.Create(onNext, e => { throw e; }, () => { }));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(Observer.Create(onNext, onError, () => { }));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(Observer.Create(onNext, e => { throw e; }, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(Observer.Create(onNext, onError, onCompleted));
        }
    }
}