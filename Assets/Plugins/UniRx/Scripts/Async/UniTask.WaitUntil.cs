#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask WaitUntil(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitUntilPromise(predicate, timing, cancellationToken);
            return promise.Task;
        }

        public static UniTask WaitWhile(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitWhilePromise(predicate, timing, cancellationToken);
            return promise.Task;
        }

        public static UniTask<U> WaitUntilValueChanged<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = null, CancellationToken cancellationToken = default(CancellationToken))
          where T : class
        {
            var unityObject = target as UnityEngine.Object;
            var isUnityObject = !object.ReferenceEquals(target, null); // don't use (unityObject == null)

            if (isUnityObject)
            {
                return new WaitUntilValueChangedUnityObjectPromise<T, U>(target, monitorFunction, monitorTiming, equalityComparer, cancellationToken).Task;
            }
            else
            {
                return new WaitUntilValueChangedStandardObjectPromise<T, U>(target, monitorFunction, monitorTiming, equalityComparer, cancellationToken).Task;
            }
        }

        public static UniTask<(bool isDestroyed, U value)> WaitUntilValueChangedWithIsDestroyed<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = null, CancellationToken cancellationToken = default(CancellationToken))
          where T : UnityEngine.Object
        {
            return new WaitUntilValueChangedUnityObjectWithReturnIsDestoryedPromise<T, U>(target, monitorFunction, monitorTiming, equalityComparer, cancellationToken).Task;
        }

        class WaitUntilPromise : ReusablePromise<AsyncUnit>, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            readonly PlayerLoopTiming timing;
            ExceptionDispatchInfo exception;
            CancellationToken cancellation;

            public WaitUntilPromise(Func<bool> predicate, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.timing = timing;
                this.cancellation = cancellation;
                this.exception = null;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (cancellation.IsCancellationRequested) return true;
                    if (exception != null) return true;

                    PlayerLoopHelper.AddAction(timing, this);
                    return false;
                }
            }

            public override AsyncUnit GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
                exception?.Throw();

                return base.GetResult();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }

                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }

                if (result)
                {
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }

        class WaitWhilePromise : ReusablePromise<AsyncUnit>, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            readonly PlayerLoopTiming timing;
            ExceptionDispatchInfo exception;
            CancellationToken cancellation;

            public WaitWhilePromise(Func<bool> predicate, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.timing = timing;
                this.cancellation = cancellation;
                this.exception = null;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (cancellation.IsCancellationRequested) return true;
                    if (exception != null) return true;

                    PlayerLoopHelper.AddAction(timing, this);
                    return false;
                }
            }

            public override AsyncUnit GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
                exception?.Throw();

                return base.GetResult();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }


                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }

                if (!result)
                {
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }

        class WaitUntilValueChangedUnityObjectPromise<T, U> : ReusablePromise<U>, IPlayerLoopItem
        {
            readonly T target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly U initialValue;
            readonly PlayerLoopTiming timing;
            ExceptionDispatchInfo exception;
            CancellationToken cancellation;

            public WaitUntilValueChangedUnityObjectPromise(T target, Func<T, U> monitorFunction, PlayerLoopTiming timing, IEqualityComparer<U> equalityComparer, CancellationToken cancellation)
            {
                this.target = target;
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UniRxAsyncDefaultEqualityComparer.GetDefault<U>();
                this.initialValue = monitorFunction(target);
                this.cancellation = cancellation;
                this.timing = timing;
                this.exception = null;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (cancellation.IsCancellationRequested) return true;
                    if (exception != null) return true;

                    PlayerLoopHelper.AddAction(timing, this);
                    return false;
                }
            }

            public override U GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
                if (exception != null) exception.Throw();

                return base.GetResult();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TryInvokeContinuation(default(U));
                    return false;
                }

                U nextValue = default(U);

                if (target != null)
                {
                    try
                    {
                        nextValue = monitorFunction(target);
                        if (equalityComparer.Equals(initialValue, nextValue))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ExceptionDispatchInfo.Capture(ex);
                        return false;
                    }
                }
                else
                {
                    exception = ExceptionDispatchInfo.Capture(new InvalidOperationException("Monitoring target is already destoyed."));
                    TryInvokeContinuation(default(U));
                    return false;
                }

                TryInvokeContinuation(nextValue);
                return false;
            }
        }

        class WaitUntilValueChangedUnityObjectWithReturnIsDestoryedPromise<T, U> : ReusablePromise<(bool, U)>, IPlayerLoopItem
          where T : UnityEngine.Object
        {
            readonly T target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly U initialValue;
            readonly PlayerLoopTiming timing;
            bool isDestroyed;
            ExceptionDispatchInfo exception;
            CancellationToken cancellation;

            public WaitUntilValueChangedUnityObjectWithReturnIsDestoryedPromise(T target, Func<T, U> monitorFunction, PlayerLoopTiming timing, IEqualityComparer<U> equalityComparer, CancellationToken cancellation)
            {
                this.target = target;
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UniRxAsyncDefaultEqualityComparer.GetDefault<U>();
                this.initialValue = monitorFunction(target);
                this.cancellation = cancellation;
                this.timing = timing;
                this.exception = null;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (cancellation.IsCancellationRequested) return true;
                    if (exception != null) return true;

                    PlayerLoopHelper.AddAction(timing, this);
                    return false;
                }
            }

            public override (bool, U) GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
                if (exception != null) exception.Throw();

                return base.GetResult();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TryInvokeContinuation((false, default(U)));
                    return false;
                }

                U nextValue = default(U);

                if (target != null)
                {
                    try
                    {
                        nextValue = monitorFunction(target);
                        if (equalityComparer.Equals(initialValue, nextValue))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ExceptionDispatchInfo.Capture(ex);
                        return false;
                    }
                }
                else
                {
                    TryInvokeContinuation((true, default(U)));
                    return false;
                }

                TryInvokeContinuation((false, nextValue));
                return false;
            }
        }

        class WaitUntilValueChangedStandardObjectPromise<T, U> : ReusablePromise<U>, IPlayerLoopItem
          where T : class
        {
            readonly WeakReference<T> target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly U initialValue;
            readonly PlayerLoopTiming timing;
            ExceptionDispatchInfo exception;
            CancellationToken cancellation;

            public WaitUntilValueChangedStandardObjectPromise(T target, Func<T, U> monitorFunction, PlayerLoopTiming timing, IEqualityComparer<U> equalityComparer, CancellationToken cancellation)
            {
                this.target = new WeakReference<T>(target);
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UniRxAsyncDefaultEqualityComparer.GetDefault<U>();
                this.initialValue = monitorFunction(target);
                this.cancellation = cancellation;
                this.timing = timing;
                this.exception = null;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (cancellation.IsCancellationRequested) return true;
                    if (exception != null) return true;

                    PlayerLoopHelper.AddAction(timing, this);
                    return false;
                }
            }

            public override U GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
                if (exception != null) exception.Throw();

                return base.GetResult();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TryInvokeContinuation(default(U));
                    return false;
                }

                U nextValue = default(U);

                if (target.TryGetTarget(out var t))
                {
                    try
                    {
                        nextValue = monitorFunction(t);
                        if (equalityComparer.Equals(initialValue, nextValue))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ExceptionDispatchInfo.Capture(ex);
                        return false;
                    }
                }
                else
                {
                    exception = ExceptionDispatchInfo.Capture(new InvalidOperationException("Monitoring target is garbage collected."));
                    TryInvokeContinuation(default(U));
                    return false;
                }

                TryInvokeContinuation(nextValue);
                return false;
            }
        }

        static class UniRxAsyncDefaultEqualityComparer
        {
            public static readonly IEqualityComparer<Vector2> Vector2 = new Vector2EqualityComparer();
            public static readonly IEqualityComparer<Vector3> Vector3 = new Vector3EqualityComparer();
            public static readonly IEqualityComparer<Vector4> Vector4 = new Vector4EqualityComparer();
            public static readonly IEqualityComparer<Color> Color = new ColorEqualityComparer();
            public static readonly IEqualityComparer<Rect> Rect = new RectEqualityComparer();
            public static readonly IEqualityComparer<Bounds> Bounds = new BoundsEqualityComparer();
            public static readonly IEqualityComparer<Quaternion> Quaternion = new QuaternionEqualityComparer();

            static readonly RuntimeTypeHandle vector2Type = typeof(Vector2).TypeHandle;
            static readonly RuntimeTypeHandle vector3Type = typeof(Vector3).TypeHandle;
            static readonly RuntimeTypeHandle vector4Type = typeof(Vector4).TypeHandle;
            static readonly RuntimeTypeHandle colorType = typeof(Color).TypeHandle;
            static readonly RuntimeTypeHandle rectType = typeof(Rect).TypeHandle;
            static readonly RuntimeTypeHandle boundsType = typeof(Bounds).TypeHandle;
            static readonly RuntimeTypeHandle quaternionType = typeof(Quaternion).TypeHandle;

            static class Cache<T>
            {
                public static readonly IEqualityComparer<T> Comparer;

                static Cache()
                {
                    var comparer = GetDefaultHelper(typeof(T));
                    if (comparer == null)
                    {
                        Comparer = EqualityComparer<T>.Default;
                    }
                    else
                    {
                        Comparer = (IEqualityComparer<T>)comparer;
                    }
                }
            }

            public static IEqualityComparer<T> GetDefault<T>()
            {
                return Cache<T>.Comparer;
            }

            static object GetDefaultHelper(Type type)
            {
                var t = type.TypeHandle;

                if (t.Equals(vector2Type)) return (object)UniRxAsyncDefaultEqualityComparer.Vector2;
                if (t.Equals(vector3Type)) return (object)UniRxAsyncDefaultEqualityComparer.Vector3;
                if (t.Equals(vector4Type)) return (object)UniRxAsyncDefaultEqualityComparer.Vector4;
                if (t.Equals(colorType)) return (object)UniRxAsyncDefaultEqualityComparer.Color;
                if (t.Equals(rectType)) return (object)UniRxAsyncDefaultEqualityComparer.Rect;
                if (t.Equals(boundsType)) return (object)UniRxAsyncDefaultEqualityComparer.Bounds;
                if (t.Equals(quaternionType)) return (object)UniRxAsyncDefaultEqualityComparer.Quaternion;

                return null;
            }

            class Vector2EqualityComparer : IEqualityComparer<Vector2>
            {
                public bool Equals(Vector2 self, Vector2 vector)
                {
                    return self.x.Equals(vector.x) && self.y.Equals(vector.y);
                }

                public int GetHashCode(Vector2 obj)
                {
                    return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2;
                }
            }

            class Vector3EqualityComparer : IEqualityComparer<Vector3>
            {
                public bool Equals(Vector3 self, Vector3 vector)
                {
                    return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z);
                }

                public int GetHashCode(Vector3 obj)
                {
                    return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2;
                }
            }

            class Vector4EqualityComparer : IEqualityComparer<Vector4>
            {
                public bool Equals(Vector4 self, Vector4 vector)
                {
                    return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z) && self.w.Equals(vector.w);
                }

                public int GetHashCode(Vector4 obj)
                {
                    return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2 ^ obj.w.GetHashCode() >> 1;
                }
            }

            class ColorEqualityComparer : IEqualityComparer<Color>
            {
                public bool Equals(Color self, Color other)
                {
                    return self.r.Equals(other.r) && self.g.Equals(other.g) && self.b.Equals(other.b) && self.a.Equals(other.a);
                }

                public int GetHashCode(Color obj)
                {
                    return obj.r.GetHashCode() ^ obj.g.GetHashCode() << 2 ^ obj.b.GetHashCode() >> 2 ^ obj.a.GetHashCode() >> 1;
                }
            }

            class RectEqualityComparer : IEqualityComparer<Rect>
            {
                public bool Equals(Rect self, Rect other)
                {
                    return self.x.Equals(other.x) && self.width.Equals(other.width) && self.y.Equals(other.y) && self.height.Equals(other.height);
                }

                public int GetHashCode(Rect obj)
                {
                    return obj.x.GetHashCode() ^ obj.width.GetHashCode() << 2 ^ obj.y.GetHashCode() >> 2 ^ obj.height.GetHashCode() >> 1;
                }
            }

            class BoundsEqualityComparer : IEqualityComparer<Bounds>
            {
                public bool Equals(Bounds self, Bounds vector)
                {
                    return self.center.Equals(vector.center) && self.extents.Equals(vector.extents);
                }

                public int GetHashCode(Bounds obj)
                {
                    return obj.center.GetHashCode() ^ obj.extents.GetHashCode() << 2;
                }
            }

            class QuaternionEqualityComparer : IEqualityComparer<Quaternion>
            {
                public bool Equals(Quaternion self, Quaternion vector)
                {
                    return self.x.Equals(vector.x) && self.y.Equals(vector.y) && self.z.Equals(vector.z) && self.w.Equals(vector.w);
                }

                public int GetHashCode(Quaternion obj)
                {
                    return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2 ^ obj.w.GetHashCode() >> 1;
                }
            }
        }
    }
}
#endif