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
                    if (publisher != null)
                    {
                        publisher.OnNext(this.value);
                    }
                    return;
                }

                if (value == null)
                {
                    if (this.value != null)
                    {
                        SetValue(value);

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

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
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

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
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


        // for inspectable properties

        public static IntReactiveProperty ToInspectableReactiveProperty(this IObservable<int> source, int initialValue = default(int))
        {
          return new IntReactiveProperty(source, initialValue);
        }

        public static LongReactiveProperty ToInspectableReactiveProperty(this IObservable<long> source, long initialValue = default(long))
        {
          return new LongReactiveProperty(source, initialValue);
        }

        public static ByteReactiveProperty ToInspectableReactiveProperty(this IObservable<byte> source, byte initialValue = default(byte))
        {
          return new ByteReactiveProperty(source, initialValue);
        }

        public static FloatReactiveProperty ToInspectableReactiveProperty(this IObservable<float> source, float initialValue = default(float))
        {
          return new FloatReactiveProperty(source, initialValue);
        }

        public static DoubleReactiveProperty ToInspectableReactiveProperty(this IObservable<double> source, double initialValue = default(double))
        {
          return new DoubleReactiveProperty(source, initialValue);
        }

        public static StringReactiveProperty ToInspectableReactiveProperty(this IObservable<string> source, string initialValue = default(string))
        {
          return new StringReactiveProperty(source, initialValue);
        }

        public static BoolReactiveProperty ToInspectableReactiveProperty(this IObservable<bool> source, bool initialValue = default(bool))
        {
          return new BoolReactiveProperty(source, initialValue);
        }

        public static Vector2ReactiveProperty ToInspectableReactiveProperty(this IObservable<Vector2> source, Vector2 initialValue = default(Vector2))
        {
          return new Vector2ReactiveProperty(source, initialValue);
        }

        public static Vector3ReactiveProperty ToInspectableReactiveProperty(this IObservable<Vector3> source, Vector3 initialValue = default(Vector3))
        {
          return new Vector3ReactiveProperty(source, initialValue);
        }

        public static Vector4ReactiveProperty ToInspectableReactiveProperty(this IObservable<Vector4> source, Vector4 initialValue = default(Vector4))
        {
          return new Vector4ReactiveProperty(source, initialValue);
        }

        public static ColorReactiveProperty ToInspectableReactiveProperty(this IObservable<Color> source, Color initialValue = default(Color))
        {
          return new ColorReactiveProperty(source, initialValue);
        }

        public static RectReactiveProperty ToInspectableReactiveProperty(this IObservable<Rect> source, Rect initialValue = default(Rect))
        {
          return new RectReactiveProperty(source, initialValue);
        }

        public static AnimationCurveReactiveProperty ToInspectableReactiveProperty(this IObservable<AnimationCurve> source, AnimationCurve initialValue = default(AnimationCurve))
        {
          return new AnimationCurveReactiveProperty(source, initialValue);
        }

        public static BoundsReactiveProperty ToInspectableReactiveProperty(this IObservable<Bounds> source, Bounds initialValue = default(Bounds))
        {
          return new BoundsReactiveProperty(source, initialValue);
        }

        public static QuaternionReactiveProperty ToInspectableReactiveProperty(this IObservable<Quaternion> source, Quaternion initialValue = default(Quaternion))
        {
          return new QuaternionReactiveProperty(source, initialValue);
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