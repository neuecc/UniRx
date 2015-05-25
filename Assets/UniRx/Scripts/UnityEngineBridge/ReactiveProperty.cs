using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>
    {
        T Value { get; }
    }

    public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    [Serializable]
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable
    {
        [NonSerialized]
        bool canPublishValueOnSubscribe = false;

        [NonSerialized]
        bool isDisposed = false;

        [SerializeField]
        T value = default(T);

        [NonSerialized]
        Subject<T> publisher = null;

        [NonSerialized]
        IDisposable sourceConnection = null;

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value == null)
                {
                    if (this.value != null)
                    {
                        SetValue(value);
                        canPublishValueOnSubscribe = true;

                        if (isDisposed) return; // don't notify but set value 
                        if (publisher != null)
                        {
                            publisher.OnNext(this.value);
                        }
                    }
                }
                else
                {
                    if (this.value == null || !this.value.Equals(value)) // don't use EqualityComparer<T>.Default
                    {
                        SetValue(value);
                        canPublishValueOnSubscribe = true;

                        if (isDisposed) return;
                        if (publisher != null)
                        {
                            publisher.OnNext(this.value);
                        }
                    }
                }
            }
        }

        public ReactiveProperty()
            : this(default(T))
        {
            // default constructor 'can' publish value on subscribe.
            // because sometimes value is deserialized from UnityEngine.
        }

        public ReactiveProperty(T initialValue)
        {
            value = initialValue;
            canPublishValueOnSubscribe = true;
        }

        public ReactiveProperty(IObservable<T> source)
        {
            // initialized from source's ReactiveProperty `doesn't` publish value on subscribe.
            // because there ReactiveProeprty is `Future/Task/Promise`.

            canPublishValueOnSubscribe = false;
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(x =>
            {
                Value = x;
            }, publisher.OnError, publisher.OnCompleted);
        }

        public ReactiveProperty(IObservable<T> source, T initialValue)
        {
            canPublishValueOnSubscribe = false;
            Value = initialValue;
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(x =>
            {
                Value = x;
            }, publisher.OnError, publisher.OnCompleted);
        }

        protected virtual void SetValue(T value)
        {
            this.value = value;
        }

        public void SetValueAndForceNotify(T value)
        {
            SetValue(value);

            if (isDisposed) return;

            if (publisher != null)
            {
                publisher.OnNext(this.value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (publisher == null)
            {
                publisher = new Subject<T>();
            }

            var subscription = publisher.Subscribe(observer);
            if (canPublishValueOnSubscribe)
            {
                observer.OnNext(value); // raise latest value on subscribe
            }
            return subscription;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (sourceConnection != null)
                {
                    sourceConnection.Dispose();
                    sourceConnection = null;
                }
                if (publisher != null)
                {
                    // when dispose, notify OnCompleted
                    try
                    {
                        publisher.OnCompleted();
                    }
                    finally
                    {
                        publisher.Dispose();
                        publisher = null;
                    }
                }
            }
        }

        public override string ToString()
        {
            return (value == null) ? "null" : value.ToString();
        }
    }

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable
    {
        bool canPublishValueOnSubscribe = false;

        bool isDisposed = false;

        T value = default(T);

        Subject<T> publisher = null;

        IDisposable sourceConnection = null;

        public T Value
        {
            get
            {
                return value;
            }
        }

        public ReadOnlyReactiveProperty(IObservable<T> source)
        {
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(x =>
            {
                value = x;
                canPublishValueOnSubscribe = true;
                publisher.OnNext(x);
            }, publisher.OnError, publisher.OnCompleted);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, T initialValue)
        {
            value = initialValue;
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(x =>
            {
                value = x;
                canPublishValueOnSubscribe = true;
                publisher.OnNext(x);
            }, publisher.OnError, publisher.OnCompleted);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (publisher == null)
            {
                publisher = new Subject<T>();
            }

            var subscription = publisher.Subscribe(observer);
            if (canPublishValueOnSubscribe)
            {
                observer.OnNext(value); // raise latest value on subscribe
            }
            return subscription;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (sourceConnection != null)
                {
                    sourceConnection.Dispose();
                    sourceConnection = null;
                }
                if (publisher != null)
                {
                    // when dispose, notify OnCompleted
                    try
                    {
                        publisher.OnCompleted();
                    }
                    finally
                    {
                        publisher.Dispose();
                        publisher = null;
                    }
                }
            }
        }

        public override string ToString()
        {
            return (value == null) ? "null" : value.ToString();
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

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        // for multiple toggle or etc..

        /// <summary>
        /// Lastest values of each sequence are all true.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllTrue(this IEnumerable<IObservable<bool>> sources)
        {
            return sources.CombineLatest().Select(xs =>
            {
                foreach (var item in xs)
                {
                    if (item == false) return false;
                }
                return true;
            });
        }


        /// <summary>
        /// Lastest values of each sequence are all false.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllFalse(this IEnumerable<IObservable<bool>> sources)
        {
            return sources.CombineLatest().Select(xs =>
            {
                foreach (var item in xs)
                {
                    if (item == true) return false;
                }
                return true;
            });
        }
    }
}