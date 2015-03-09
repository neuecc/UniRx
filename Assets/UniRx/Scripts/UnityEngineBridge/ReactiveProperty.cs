using System;
using UnityEngine;

namespace UniRx
{
    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    [Serializable]
    public class ReactiveProperty<T> : IObservable<T>, IDisposable
    {
        [NonSerialized]
        bool isDisposed = false;

        [SerializeField]
        T value = default(T);

        [NonSerialized]
        Subject<T> publisher = null;

        [NonSerialized]
        readonly IDisposable sourceConnection;

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                if (isDisposed) return;
                if (publisher != null)
                {
                    publisher.OnNext(value);
                }
            }
        }

        public ReactiveProperty()
        {
        }

        public ReactiveProperty(T initialValue)
        {
            value = initialValue;
        }

        public ReactiveProperty(IObservable<T> source)
        {
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(x =>
            {
                value = x;
                publisher.OnNext(x);
            }, publisher.OnError, publisher.OnCompleted);
        }

        public ReactiveProperty(IObservable<T> source, T initialValue)
        {
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(publisher);
            value = initialValue;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (publisher == null)
            {
                publisher = new Subject<T>();
            }

            var subscription = publisher.Subscribe(observer);
            observer.OnNext(value); // raise latest value on subscribe
            return subscription;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (sourceConnection != null)
                {
                    sourceConnection.Dispose();
                }
                if (publisher != null)
                {
                    publisher.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// Extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactivePropertyExtensions
    {
        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReactiveProperty<T>(source);
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReactiveProperty<T>(source, initialValue);
        }
    }
}