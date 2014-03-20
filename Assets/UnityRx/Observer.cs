using System;
using System.Collections;

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
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onError == null) throw new ArgumentNullException("onError");
            if (onCompleted == null) throw new ArgumentNullException("onCompleted");

            return new AnonymousObserver<T>(onNext, onError, onCompleted);
        }

        class AnonymousObserver<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnCompleted()
            {
                onCompleted();
            }

            public void OnError(Exception error)
            {
                onError(error);
            }

            public void OnNext(T value)
            {
                onNext(value);
            }
        }
    }

    public static partial class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            return source.Subscribe(Observer.Create<T>(_ => { }, _ => { }, () => { }));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(Observer.Create(onNext, _ => { }, () => { }));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(Observer.Create(onNext, onError, () => { }));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(Observer.Create(onNext, _ => { }, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(Observer.Create(onNext, onError, onCompleted));
        }
    }
}