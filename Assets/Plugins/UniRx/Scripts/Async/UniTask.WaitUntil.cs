#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask WaitUntil(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitUntilPromise(predicate, cancellationToken);
            PlayerLoopHelper.AddAction(timing, promise);
            return promise.Task;
        }

        public static UniTask WaitWhile(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitWhilePromise(predicate, cancellationToken);
            PlayerLoopHelper.AddAction(timing, promise);
            return promise.Task;
        }

        public static UniTask<U> WaitUntilValueChanged<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
          where T : class
        {
            var promise = new WaitUntilValueChangedAwaiter<T, U>(target, monitorFunction, null);
            PlayerLoopHelper.AddAction(monitorTiming, promise);
            return promise.Task;
        }

        class WaitUntilPromise : Promise<AsyncUnit>, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            CancellationToken cancellation;

            public UniTask Task => new UniTask(this);

            public WaitUntilPromise(Func<bool> predicate, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.cancellation = cancellation;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    SetException(ex);
                    return false;
                }

                if (result)
                {
                    SetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }

        class WaitWhilePromise : Promise<AsyncUnit>, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            CancellationToken cancellation;

            public UniTask Task => new UniTask(this);

            public WaitWhilePromise(Func<bool> predicate, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.cancellation = cancellation;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }


                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    SetException(ex);
                    return false;
                }

                if (!result)
                {
                    SetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }

        class WaitUntilValueChangedAwaiter<T, U> : Promise<U>, IPlayerLoopItem
          where T : class
        {
            readonly T target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly U initialValue;
            CancellationToken cancellation;

            public UniTask<U> Task => new UniTask<U>(this);

            public WaitUntilValueChangedAwaiter(T target, Func<T, U> monitorFunction, IEqualityComparer<U> equalityComparer)
            {
                this.target = target;
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UniRxAsyncDefaultEqualityComparer.GetDefault<U>();
                this.initialValue = monitorFunction(target);
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                U nextValue = default(U);
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
                    SetException(ex);
                    return false;
                }

                SetResult(nextValue);
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