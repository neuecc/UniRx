// ChainingAssertion for Unity
// https://github.com/neuecc/ChainingAssertion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeUnitTestToolkit
{
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public static partial class AssertEx
    {
        /// <summary>Assert.AreEqual, if T is IEnumerable then CollectionAssert.AreEqual</summary>
        public static void Is<T>(this T actual, T expected, string message = "")
        {
            if (typeof(T) != typeof(string)
#if !UNITY_METRO
                && typeof(IEnumerable).IsAssignableFrom(typeof(T))
#endif
                )
            {
                ((IEnumerable)actual).Cast<object>().Is(((IEnumerable)expected).Cast<object>(), message);
                return;
            }

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

        /// <summary>Assert.AreNotEqual, if T is IEnumerable then CollectionAssert.AreNotEqual</summary>
        public static void IsNot<T>(this T actual, T notExpected, string message = "")
        {
            if (typeof(T) != typeof(string)
#if !UNITY_METRO
                && typeof(IEnumerable).IsAssignableFrom(typeof(T))
#endif
                )
            {
                ((IEnumerable)actual).Cast<object>().IsNot(((IEnumerable)notExpected).Cast<object>(), message);
                return;
            }

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

        /// <summary>Asset Collefction is empty?</summary>
        public static void IsEmpty<T>(this IEnumerable<T> source)
        {
            source.Any().IsFalse();
        }

        /// <summary>Alternative of ExpectedExceptionAttribute(allow derived type)</summary>
        public static T Catch<T>(Action testCode, string message = "") where T : Exception
        {
            var exception = ExecuteCode(testCode);
            var headerMsg = "Failed Throws<" + typeof(T).Name + ">.";
            var additionalMsg = string.IsNullOrEmpty(message) ? "" : ", " + message;

            if (exception == null)
            {
                var formatted = headerMsg + " No exception was thrown" + additionalMsg;
                Assert.Fail(formatted);
            }
#if !UNITY_METRO
            else if (!typeof(T).IsInstanceOfType(exception))
            {
                var formatted = string.Format("{0} Catched:{1}{2}", headerMsg, exception.GetType().Name, additionalMsg);
                Assert.Fail(formatted);
            }
#endif

            return (T)exception;
        }

        /// <summary>Alternative of ExpectedExceptionAttribute(not allow derived type)</summary>
        public static T Throws<T>(Action testCode, string message = "") where T : Exception
        {
            var exception = Catch<T>(testCode, message);

            if (!typeof(T).Equals(exception.GetType()))
            {
                var headerMsg = "Failed Throws<" + typeof(T).Name + ">.";
                var additionalMsg = string.IsNullOrEmpty(message) ? "" : ", " + message;
                var formatted = string.Format("{0} Catched:{1}{2}", headerMsg, exception.GetType().Name, additionalMsg);
                Assert.Fail(formatted);
            }

            return (T)exception;
        }

        /// <summary>does not throw any exceptions</summary>
        public static void DoesNotThrow(Action testCode, string message = "")
        {
            var exception = ExecuteCode(testCode);
            if (exception != null)
            {
                var formatted = string.Format("Failed DoesNotThrow. Catched:{0}{1}", exception.GetType().Name, string.IsNullOrEmpty(message) ? "" : ", " + message);
                Assert.Fail(formatted);
            }
        }

        /// <summary>execute action and return exception when catched otherwise return null</summary>
        private static Exception ExecuteCode(Action testCode)
        {
            try
            {
                testCode();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
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

