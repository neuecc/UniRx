using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public static class UnityEqualityComparer
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

        static class RuntimeTypeHandlerCache<T>
        {
            public static readonly RuntimeTypeHandle TypeHandle = typeof(T).TypeHandle;
        }

        public static IEqualityComparer<T> GetDefault<T>()
        {
            var t = RuntimeTypeHandlerCache<T>.TypeHandle;

            if (t.Equals(vector2Type)) return (IEqualityComparer<T>)UnityEqualityComparer.Vector2;
            if (t.Equals(vector3Type)) return (IEqualityComparer<T>)UnityEqualityComparer.Vector3;
            if (t.Equals(vector4Type)) return (IEqualityComparer<T>)UnityEqualityComparer.Vector4;
            if (t.Equals(colorType)) return (IEqualityComparer<T>)UnityEqualityComparer.Color;
            if (t.Equals(rectType)) return (IEqualityComparer<T>)UnityEqualityComparer.Rect;
            if (t.Equals(boundsType)) return (IEqualityComparer<T>)UnityEqualityComparer.Bounds;
            if (t.Equals(quaternionType)) return (IEqualityComparer<T>)UnityEqualityComparer.Quaternion;

            return EqualityComparer<T>.Default;
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