using System;
using System.Collections.Generic;
#if !UniRxLibrary
using UnityEngine;
#endif

namespace UniRx
{
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>
    {
        T Value { get; }
        bool HasValue { get; }
    }

    public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    [Serializable]
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable, IOptimizedObservable<T>
    {
#if !UniRxLibrary
        static readonly IEqualityComparer<T> defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();
#else
        static readonly IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;
#endif

        [NonSerialized]
        bool canPublishValueOnSubscribe = false;

        [NonSerialized]
        bool isDisposed = false;

#if !UniRxLibrary
        [SerializeField]
#endif
        T value = default(T);

        [NonSerialized]
        Subject<T> publisher = null;

        [NonSerialized]
        IDisposable sourceConnection = null;

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return defaultEqualityComparer;
            }
        }

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!canPublishValueOnSubscribe)
                {
                    canPublishValueOnSubscribe = true;
                    SetValue(value);

                    if (isDisposed) return; // don't notify but set value
                    var p = publisher;
                    if (p != null)
                    {
                        p.OnNext(this.value);
                    }
                    return;
                }

                if (!EqualityComparer.Equals(this.value, value))
                {
                    SetValue(value);

                    if (isDisposed) return;
                    var p = publisher;
                    if (p != null)
                    {
                        p.OnNext(this.value);
                    }
                }
            }
        }

        public bool HasValue
        {
            get
            {
                return canPublishValueOnSubscribe;
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
            SetValue(initialValue);
            canPublishValueOnSubscribe = true;
        }

        public ReactiveProperty(IObservable<T> source)
        {
            // initialized from source's ReactiveProperty `doesn't` publish value on subscribe.
            // because there ReactiveProeprty is `Future/Task/Promise`.

            canPublishValueOnSubscribe = false;
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
        }

        public ReactiveProperty(IObservable<T> source, T initialValue)
        {
            canPublishValueOnSubscribe = false;
            Value = initialValue; // Value set canPublishValueOnSubcribe = true
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
        }

        protected virtual void SetValue(T value)
        {
            this.value = value;
        }

        public void SetValueAndForceNotify(T value)
        {
            SetValue(value);

            if (isDisposed) return;

            var p = publisher;
            if (p != null)
            {
                p.OnNext(this.value);
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
                // Interlocked.CompareExchange is bit slower, guarantee threasafety is overkill.
                // System.Threading.Interlocked.CompareExchange(ref publisher, new Subject<T>(), null);
                publisher = new Subject<T>();
            }

            var p = publisher;
            if (p != null)
            {
                var subscription = p.Subscribe(observer);
                if (canPublishValueOnSubscribe)
                {
                    observer.OnNext(value); // raise latest value on subscribe
                }
                return subscription;
            }
            else
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
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
                var sc = sourceConnection;
                if (sc != null)
                {
                    sc.Dispose();
                    sourceConnection = null;
                }
                var p = publisher;
                if (p != null)
                {
                    // when dispose, notify OnCompleted
                    try
                    {
                        p.OnCompleted();
                    }
                    finally
                    {
                        p.Dispose();
                        publisher = null;
                    }
                }
            }
        }

        public override string ToString()
        {
            return (value == null) ? "null" : value.ToString();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        class ReactivePropertyObserver : IObserver<T>
        {
            readonly ReactiveProperty<T> parent;
            int isStopped = 0;

            public ReactivePropertyObserver(ReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            public void OnNext(T value)
            {
                parent.Value = value;
            }

            public void OnError(Exception error)
            {
                if (System.Threading.Interlocked.Increment(ref isStopped) == 1)
                {
                    parent.publisher.OnError(error);
                }
            }

            public void OnCompleted()
            {
                if (System.Threading.Interlocked.Increment(ref isStopped) == 1)
                {
                    parent.publisher.OnCompleted();
                }
            }
        }
    }

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable, IOptimizedObservable<T>
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

        public bool HasValue
        {
            get
            {
                return canPublishValueOnSubscribe;
            }
        }

        public ReadOnlyReactiveProperty(IObservable<T> source)
        {
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, T initialValue)
        {
            value = initialValue;
            canPublishValueOnSubscribe = true;
            publisher = new Subject<T>();
            sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
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
                // Interlocked.CompareExchange is bit slower, guarantee threasafety is overkill.
                // System.Threading.Interlocked.CompareExchange(ref publisher, new Subject<T>(), null);
                publisher = new Subject<T>();
            }

            var p = publisher;
            if (p != null)
            {
                var subscription = p.Subscribe(observer);
                if (canPublishValueOnSubscribe)
                {
                    observer.OnNext(value); // raise latest value on subscribe
                }
                return subscription;
            }
            else
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
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
                var sc = sourceConnection;
                if (sc != null)
                {
                    sc.Dispose();
                    sourceConnection = null;
                }

                var p = publisher;
                if (p != null)
                {
                    // when dispose, notify OnCompleted
                    try
                    {
                        p.OnCompleted();
                    }
                    finally
                    {
                        p.Dispose();
                        publisher = null;
                    }
                }
            }
        }

        public override string ToString()
        {
            return (value == null) ? "null" : value.ToString();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        class ReadOnlyReactivePropertyObserver : IObserver<T>
        {
            readonly ReadOnlyReactiveProperty<T> parent;
            int isStopped = 0;

            public ReadOnlyReactivePropertyObserver(ReadOnlyReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            public void OnNext(T value)
            {
                parent.value = value;
                parent.canPublishValueOnSubscribe = true;
                parent.publisher.OnNext(value);
            }

            public void OnError(Exception error)
            {
                if (System.Threading.Interlocked.Increment(ref isStopped) == 1)
                {
                    parent.publisher.OnError(error);
                }
            }

            public void OnCompleted()
            {
                if (System.Threading.Interlocked.Increment(ref isStopped) == 1)
                {
                    parent.publisher.OnCompleted();
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

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        public static IObservable<T> SkipLatestValueOnSubscribe<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return source.HasValue ? source.Skip(1) : source;
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