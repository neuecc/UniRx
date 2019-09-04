using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine.Assertions; // use UnityEngineAssertion.

// no namespace.

[System.Diagnostics.DebuggerStepThroughAttribute]
public static partial class ChainingAssertion
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

    /// <summary>Assert.IsTrue(expected(actual)).</summary>
    public static void Is<T>(this T actual, Func<T, bool> expected, string message = "")
    {
        Assert.IsTrue(expected(actual), message);
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, params T[] expected)
    {
        Is(actual, expected.AsEnumerable());
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message = "")
    {
        NUnit.Framework.CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), message);
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer, string message = "")
    {
        Is(actual, expected, comparer.Equals, message);
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<T, T, bool> equalityComparison, string message = "")
    {
        NUnit.Framework.CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
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
    public static void IsNot<T>(this IEnumerable<T> actual, params T[] notExpected)
    {
        IsNot(actual, notExpected.AsEnumerable());
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, string message = "")
    {
        NUnit.Framework.CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), message);
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, IEqualityComparer<T> comparer, string message = "")
    {
        IsNot(actual, notExpected, comparer.Equals, message);
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, Func<T, T, bool> equalityComparison, string message = "")
    {
        NUnit.Framework.CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
    }

    /// <summary>Assert.IsNull</summary>
    public static void IsNull<T>(this T value, string message = "")
        where T : class
    {
        Assert.IsNull(value, message);
    }

    /// <summary>Assert.IsNotNull</summary>
    public static void IsNotNull<T>(this T value, string message = "")
        where T : class
    {
        Assert.IsNotNull(value, message);
    }

    /// <summary>Assert.IsTrue</summary>
    public static void IsTrue(this bool value, string message = "")
    {
        Assert.IsTrue(value, message);
    }

    /// <summary>Assert.IsFalse</summary>
    public static void IsFalse(this bool value, string message = "")
    {
        Assert.IsFalse(value, message);
    }

    /// <summary>Assert.AreApproximatelyEqual</summary>
    public static void IsApproximatelyEqual(this float actual, float expected, string message = "")
    {
        Assert.AreApproximatelyEqual(expected, actual, message);
    }

    /// <summary>Assert.AreApproximatelyEqual</summary>
    public static void IsApproximatelyEqual(this float actual, float expected, float tolerance, string message = "")
    {
        Assert.AreApproximatelyEqual(expected, actual, tolerance, message);
    }

    /// <summary>Assert.AreApproximatelyEqual</summary>
    public static void IsApproximatelyEqual(this double actual, float expected, string message = "")
    {
        Assert.AreApproximatelyEqual(expected, (float)actual, message);
    }

    /// <summary>Assert.AreApproximatelyEqual</summary>
    public static void IsApproximatelyEqual(this double actual, float expected, float tolerance, string message = "")
    {
        Assert.AreApproximatelyEqual(expected, (float)actual, tolerance, message);
    }

    /// <summary>Assert.AreNotApproximatelyEqual</summary>
    public static void IsNotApproximatelyEqual(this float actual, float expected, string message = "")
    {
        Assert.AreNotApproximatelyEqual(expected, actual, message);
    }

    /// <summary>Assert.AreNotApproximatelyEqual</summary>
    public static void IsNotApproximatelyEqual(this float actual, float expected, float tolerance, string message = "")
    {
        Assert.AreNotApproximatelyEqual(expected, actual, tolerance, message);
    }

    /// <summary>Assert.AreSame</summary>
    public static void IsSameReferenceAs<T>(this T actual, T expected, string message = "")
    {
        NUnit.Framework.Assert.AreSame(expected, actual, message);
    }

    /// <summary>Assert.AreNotSame</summary>
    public static void IsNotSameReferenceAs<T>(this T actual, T notExpected, string message = "")
    {
        NUnit.Framework.Assert.AreNotSame(notExpected, actual, message);
    }

    /// <summary>Assert.IsInstanceOf</summary>
    public static TExpected IsInstanceOf<TExpected>(this object value, string message = "")
    {
        NUnit.Framework.Assert.IsInstanceOf<TExpected>(value, message);
        return (TExpected)value;
    }

    /// <summary>Assert.IsNotInstanceOf</summary>
    public static void IsNotInstanceOf<TWrong>(this object value, string message = "")
    {
        NUnit.Framework.Assert.IsNotInstanceOf<TWrong>(value, message);
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
                : Equals(x, y) ? 0 : -1;
        }
    }


    #region StructuralEqual

    /// <summary>Assert by deep recursive value equality compare</summary>
    public static void IsStructuralEqual(this object actual, object expected, string message = "")
    {
        message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
        if (ReferenceEquals(actual, expected)) return;

        if (actual == null) throw new AssertionException("actual is null", message);
        if (expected == null) throw new AssertionException("actual is not null", message);
        if (actual.GetType() != expected.GetType())
        {
            var msg = string.Format("expected type is {0} but actual type is {1}",
                expected.GetType().Name, actual.GetType().Name);
            throw new AssertionException(msg, message);
        }

        var r = StructuralEqual(actual, expected, new[] { actual.GetType().Name }); // root type
        if (!r.IsEquals)
        {
            var msg = string.Format("is not structural equal, failed at {0}, actual = {1} expected = {2}",
                string.Join(".", r.Names.ToArray()), r.Left, r.Right);
            throw new AssertionException(msg, message);
        }
    }

    /// <summary>Assert by deep recursive value equality compare</summary>
    public static void IsNotStructuralEqual(this object actual, object expected, string message = "")
    {
        message = (string.IsNullOrEmpty(message) ? "" : ", " + message);
        if (ReferenceEquals(actual, expected)) throw new AssertionException("actual is same reference", message);

        if (actual == null) return;
        if (expected == null) return;
        if (actual.GetType() != expected.GetType())
        {
            return;
        }

        var r = StructuralEqual(actual, expected, new[] { actual.GetType().Name }); // root type
        if (r.IsEquals)
        {
            throw new AssertionException("is structural equal", message);
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
        if (ReferenceEquals(left, right)) return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
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

            object lv;
            object rv;

            if (mi is FieldInfo)
            {
                var i = mi as FieldInfo;
                lv = i.GetValue(left);
                rv = i.GetValue(right);
            }
            else
            {
                var i = mi as PropertyInfo;
                lv = i.GetValue(left, null);
                rv = i.GetValue(right, null);
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
