// modified for Unity

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    #region Extensions

    [System.Diagnostics.DebuggerStepThroughAttribute]
    public static partial class AssertEx
    {
        /// <summary>Assert.AreEqual, if T is IEnumerable then CollectionAssert.AreEqual</summary>
        public static void Is<T>(this T actual, T expected, string message = "")
        {
            if (typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
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
            if (typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
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
            else if (!typeof(T).IsInstanceOfType(exception))
            {
                var formatted = string.Format("{0} Catched:{1}{2}", headerMsg, exception.GetType().Name, additionalMsg);
                Assert.Fail(formatted);
            }

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

        private class ReflectAccessor<T>
        {
            public Func<object> GetValue { get; private set; }
            public Action<object> SetValue { get; private set; }

            public ReflectAccessor(T target, string name)
            {
                var field = typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    GetValue = () => field.GetValue(target);
                    SetValue = value => field.SetValue(target, value);
                    return;
                }

                var prop = typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null)
                {
                    GetValue = () => prop.GetValue(target, null);
                    SetValue = value => prop.SetValue(target, value, null);
                    return;
                }

                throw new ArgumentException(string.Format("\"{0}\" not found : Type <{1}>", name, typeof(T).Name));
            }
        }

        #region StructuralEqual

        /// <summary>Assert by deep recursive value equality compare</summary>
        public static void IsStructuralEqual(this object actual, object expected, string message = "")
        {
            message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
            if (object.ReferenceEquals(actual, expected)) return;

            if (actual == null) { Assert.Fail("actual is null" + message); return; }
            if (expected == null) { Assert.Fail("actual is not null" + message); return; }
            if (actual.GetType() != expected.GetType())
            {
                var msg = string.Format("expected type is {0} but actual type is {1}{2}",
                    expected.GetType().Name, actual.GetType().Name, message);
                Assert.Fail(msg);
                return;
            }

            var r = StructuralEqual(actual, expected, new[] { actual.GetType().Name }); // root type
            if (!r.IsEquals)
            {
                var msg = string.Format("is not structural equal, failed at {0}, actual = {1} expected = {2}{3}",
                    string.Join(".", r.Names.ToArray()), r.Left, r.Right, message);
                Assert.Fail(msg);
                return;
            }
        }

        /// <summary>Assert by deep recursive value equality compare</summary>
        public static void IsNotStructuralEqual(this object actual, object expected, string message = "")
        {
            message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
            if (object.ReferenceEquals(actual, expected))
            {
                Assert.Fail("actual is same reference" + message);
                return;
            }

            if (actual == null) return;
            if (expected == null) return;
            if (actual.GetType() != expected.GetType())
            {
                return;
            }

            var r = StructuralEqual(actual, expected, new[] { actual.GetType().Name }); // root type
            if (r.IsEquals)
            {
                Assert.Fail("is structural equal" + message);
            }
        }

        static EqualInfo SequenceEqual(IEnumerable leftEnumerable, IEnumerable rightEnumarable, IEnumerable<string> names)
        {
            var le = leftEnumerable.GetEnumerator();
            using (le as IDisposable)
            {
                var re = rightEnumarable.GetEnumerator();

                using (re as IDisposable)
                {
                    var index = 0;
                    while (true)
                    {
                        object lValue = null;
                        object rValue = null;
                        var lMove = le.MoveNext();
                        var rMove = re.MoveNext();
                        if (lMove) lValue = le.Current;
                        if (rMove) rValue = re.Current;

                        if (lMove && rMove)
                        {
                            var result = StructuralEqual(lValue, rValue, names.Concat(new[] { "[" + index + "]" }));
                            if (!result.IsEquals)
                            {
                                return result;
                            }
                        }

                        if ((lMove == true && rMove == false) || (lMove == false && rMove == true))
                        {
                            return new EqualInfo { IsEquals = false, Left = lValue, Right = rValue, Names = names.Concat(new[] { "[" + index + "]" }) };
                        }
                        if (lMove == false && rMove == false) break;
                        index++;
                    }
                }
            }
            return new EqualInfo { IsEquals = true, Left = leftEnumerable, Right = rightEnumarable, Names = names };
        }

        static EqualInfo StructuralEqual(object left, object right, IEnumerable<string> names)
        {
            // type and basic checks
            if (object.ReferenceEquals(left, right)) return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
            if (left == null || right == null) return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };
            var lType = left.GetType();
            var rType = right.GetType();
            if (lType != rType) return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };

            var type = left.GetType();

            // not object(int, string, etc...)
            if (Type.GetTypeCode(type) != TypeCode.Object)
            {
                return new EqualInfo { IsEquals = left.Equals(right), Left = left, Right = right, Names = names };
            }

            // is sequence
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return SequenceEqual((IEnumerable)left, (IEnumerable)right, names);
            }

            // IEquatable<T>
            var equatable = typeof(IEquatable<>).MakeGenericType(type);
            if (equatable.IsAssignableFrom(type))
            {
                var result = (bool)equatable.GetMethod("Equals").Invoke(left, new[] { right });
                return new EqualInfo { IsEquals = result, Left = left, Right = right, Names = names };
            }

            // is object
            var fields = left.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            var properties = left.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetGetMethod(false) != null);
            var members = fields.Cast<MemberInfo>().Concat(properties.Cast<MemberInfo>());

            foreach (var mi in members)
            {
                var concatNames = names.Concat(new[] { (string)mi.Name });
                object lv = null;
                object rv = null;
                if (mi is FieldInfo)
                {
                    var fi = (FieldInfo)mi;
                    fi.GetValue(left);
                    fi.GetValue(right);
                }
                else if (mi is PropertyInfo)
                {
                    var pi = (PropertyInfo)mi;
                    pi.GetValue(left, null);
                    pi.GetValue(right, null);
                }

                var result = StructuralEqual(lv, rv, concatNames);
                if (!result.IsEquals)
                {
                    return result;
                }
            }

            return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
        }

        private class EqualInfo
        {
            public object Left;
            public object Right;
            public bool IsEquals;
            public IEnumerable<string> Names;
        }

        #endregion
    }

    #endregion
}