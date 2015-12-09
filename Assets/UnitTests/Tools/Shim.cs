using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class TestClassAttribute : Attribute
    {
    }
    public class TestMethodAttribute : Attribute
    {
    }

    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string message)
        {
        }

        public static void AreNotEqual<T>(T notExpected, T actual, string message)
        {
        }

        public static void IsTrue(bool condition, string message)
        {
        }
        public static void IsNull(object value, string message)
        {
        }

        public static void IsNotNull(object value, string message)
        {
        }

        public static void Fail(string msg)
        {
        }

        public static void AreSame(object expected, object actual, string message)
        {
        }

        public static void AreNotSame(object expected, object actual, string message)
        {
        }

        public static void IsInstanceOfType(object value, Type expectedType, string message)
        {
        }

        public static void IsNotInstanceOfType(object value, Type expectedType, string message)
        {
        }
    }

    public static class CollectionAssert
    {
        public static void AreEqual(ICollection expected, ICollection actual, string message)
        {
        }

        public static void AreEqual(ICollection expected, ICollection actual, IComparer comparer, string message)
        {
        }

        public static void AreNotEqual(ICollection notExpected, ICollection actual, string message)
        {
        }

        public static void AreNotEqual(ICollection notExpected, ICollection actual, IComparer comparer, string message)
        {
        }
    }
}