// ChainingAssertion for Unity
// https://github.com/neuecc/ChainingAssertion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeUnitTestToolkit
{
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public static partial class AssertExtensions
    {
        /// <summary>Assert.AreEqual.</summary>
        public static void Is<T>(this T actual, T expected, string message = "")
        {
            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>Assert.IsTrue(predicate(value))</summary>
        public static void Is<T>(this T value, Func<T, bool> predicate, string message = "")
        {
            Assert.IsTrue(predicate(value), message);
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void IsCollection<T>(this IEnumerable<T> actual, params T[] expected)
        {
            IsCollection(actual, expected.AsEnumerable());
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void IsCollection<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message = "")
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), message);
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void IsCollection<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer, string message = "")
        {
            IsCollection(actual, expected, comparer.Equals, message);
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void IsCollection<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<T, T, bool> equalityComparison, string message = "")
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
        }

        /// <summary>Assert.AreNotEqual</summary>
        public static void IsNot<T>(this T actual, T notExpected, string message = "")
        {
            Assert.AreNotEqual(notExpected, actual, message);
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void IsNotCollection<T>(this IEnumerable<T> actual, params T[] notExpected)
        {
            IsNotCollection(actual, notExpected.AsEnumerable());
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void IsNotCollection<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, string message = "")
        {
            CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), message);
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void IsNotCollection<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, IEqualityComparer<T> comparer, string message = "")
        {
            IsNotCollection(actual, notExpected, comparer.Equals, message);
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void IsNotCollection<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, Func<T, T, bool> equalityComparison, string message = "")
        {
            CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
        }

        /// <summary>Asset Collefction is empty?</summary>
        public static void IsEmpty<T>(this IEnumerable<T> source)
        {
            source.Any().IsFalse();
        }

        /// <summary>Assert.IsNull</summary>
        public static void IsNull<T>(this T value, string message = "")
        {
            Assert.IsNull(value, message);
        }

        /// <summary>Assert.IsNotNull</summary>
        public static void IsNotNull<T>(this T value, string message = "")
        {
            Assert.IsNotNull(value, message);
        }

        /// <summary>Is(true)</summary>
        public static void IsTrue(this bool value, string message = "")
        {
            value.Is(true, message);
        }

        /// <summary>Is(false)</summary>
        public static void IsFalse(this bool value, string message = "")
        {
            value.Is(false, message);
        }

        /// <summary>Assert.AreSame</summary>
        public static void IsSameReferenceAs<T>(this T actual, T expected, string message = "")
        {
            Assert.AreSame(expected, actual, message);
        }

        /// <summary>Assert.AreNotSame</summary>
        public static void IsNotSameReferenceAs<T>(this T actual, T notExpected, string message = "")
        {
            Assert.AreNotSame(notExpected, actual, message);
        }

        /// <summary>Assert.IsInstanceOf</summary>
        public static TExpected IsInstanceOf<TExpected>(this object value, string message = "")
        {
            Assert.IsInstanceOfType(value, typeof(TExpected), message);
            return (TExpected)value;
        }

        /// <summary>Assert.IsNotInstanceOf</summary>
        public static void IsNotInstanceOf<TWrong>(this object value, string message = "")
        {
            Assert.IsNotInstanceOfType(value, typeof(TWrong), message);
        }

        /// <summary>EqualityComparison to IComparer Converter for CollectionAssert</summary>
        private class ComparisonComparer<T> : IComparer
        {
            readonly Func<T, T, bool> comparison;

            public ComparisonComparer(Func<T, T, bool> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(object x, object y)
            {
                return (comparison != null)
                    ? comparison((T)x, (T)y) ? 0 : -1
                    : object.Equals(x, y) ? 0 : -1;
            }
        }
    }
}