using System;
using System.Collections;

namespace RuntimeUnitTestToolkit
{
    public class AssertFailedException : Exception
    {
        public AssertFailedException(string message)
            : base(message)
        {

        }
    }

    /// <summary>
    /// Primitive Assertion methods.
    /// </summary>
    public static partial class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
            {
                throw new AssertFailedException(string.Format("AreEqual Failed. expected:{0} actual:{1} message:{2}", expected, actual, message));
            }
        }

        public static void AreNotEqual<T>(T notExpected, T actual, string message)
        {
            if (object.Equals(notExpected, actual))
            {
                throw new AssertFailedException(string.Format("AreNotEqual Failed. notExpected:{0} actual:{1} message:{2}", notExpected, actual, message));
            }
        }

        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new AssertFailedException(string.Format("IsTrue Failed. message:{0}", message));
            }
        }

        public static void IsNull(object value, string message)
        {
            if (value != null)
            {
                throw new AssertFailedException(string.Format("IsNull Failed. value:{0} message:{1}", value, message));
            }
        }

        public static void IsNotNull(object value, string message)
        {
            if (value == null)
            {
                throw new AssertFailedException(string.Format("IsNotNull Failed. message:{0}", message));
            }
        }

        public static void Fail(string msg)
        {
            throw new AssertFailedException(string.Format("Failed. message:{0}", msg));
        }

        public static void AreSame(object expected, object actual, string message)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                throw new AssertFailedException(string.Format("AreSame Failed. expected:{0} actual:{1} message:{2}", expected, actual, message));
            }
        }

        public static void AreNotSame(object notExpected, object actual, string message)
        {
            if (object.ReferenceEquals(notExpected, actual))
            {
                throw new AssertFailedException(string.Format("AreNotSame Failed. notExpected:{0} actual:{1} message:{2}", notExpected, actual, message));
            }
        }

        public static void IsInstanceOfType(object value, Type expectedType, string message)
        {
            if (value == null || value.GetType() != expectedType)
            {
                throw new AssertFailedException(string.Format("IsInstanceOfType Failed. valueType:{0} expectedType:{1} message:{2}", (value == null) ? null : value.GetType(), expectedType, message));
            }
        }

        public static void IsNotInstanceOfType(object value, Type expectedType, string message)
        {
            if (value != null || value.GetType() == expectedType)
            {
                throw new AssertFailedException(string.Format("IsNotInstanceOfType Failed. valueType:{0} expectedType:{1} message:{2}", (value == null) ? null : value.GetType(), expectedType, message));
            }
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
        static Exception ExecuteCode(Action testCode)
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
    }

    public static class CollectionAssert
    {
        public static void AreEqual(ICollection expected, ICollection actual, string message)
        {
            var index = 0;
            var e1 = expected.GetEnumerator();
            using (e1 as IDisposable)
            {
                var e2 = actual.GetEnumerator();
                using (e2 as IDisposable)
                {
                    while (true)
                    {
                        var m1 = e1.MoveNext();
                        var m2 = e2.MoveNext();
                        if (m1 != m2)
                        {
                            throw new AssertFailedException("collection length is not equal. message:" + message);
                        }
                        if (m1 == false && m2 == false) return;

                        var c1 = e1.Current;
                        var c2 = e2.Current;

                        if (!object.Equals(c1, c2)) throw new AssertFailedException(string.Format("not equal index:{0} expected:{1} actual:{2} message:{3}", index, c1, c2, message));
                        index++;
                    }
                }
            }
        }

        public static void AreEqual(ICollection expected, ICollection actual, IComparer comparer, string message)
        {
            var index = 0;
            var e1 = expected.GetEnumerator();
            using (e1 as IDisposable)
            {
                var e2 = actual.GetEnumerator();
                using (e2 as IDisposable)
                {
                    while (true)
                    {
                        var m1 = e1.MoveNext();
                        var m2 = e2.MoveNext();
                        if (m1 != m2)
                        {
                            throw new AssertFailedException("collection length is not equal. message:" + message);
                        }
                        if (m1 == false && m2 == false) return;

                        var c1 = e1.Current;
                        var c2 = e2.Current;

                        if (comparer.Compare(c1, c2) != 0) throw new AssertFailedException(string.Format("not equal index:{0} expected:{1} actual:{2} message:{3}", index, c1, c2, message));
                        index++;
                    }
                }
            }
        }

        public static void AreNotEqual(ICollection notExpected, ICollection actual, string message)
        {
            var index = 0;
            var e1 = notExpected.GetEnumerator();
            using (e1 as IDisposable)
            {
                var e2 = actual.GetEnumerator();
                using (e2 as IDisposable)
                {
                    while (true)
                    {
                        var m1 = e1.MoveNext();
                        var m2 = e2.MoveNext();
                        if (m1 != m2)
                        {
                            throw new AssertFailedException("collection length is not equal. message:" + message);
                        }
                        if (m1 == false && m2 == false) return;

                        var c1 = e1.Current;
                        var c2 = e2.Current;

                        if (object.Equals(c1, c2)) throw new AssertFailedException(string.Format("equal index:{0} expected:{1} actual:{2} message:{3}", index, c1, c2, message));
                        index++;
                    }
                }
            }
        }

        public static void AreNotEqual(ICollection notExpected, ICollection actual, IComparer comparer, string message)
        {
            var index = 0;
            var e1 = notExpected.GetEnumerator();
            using (e1 as IDisposable)
            {
                var e2 = actual.GetEnumerator();
                using (e2 as IDisposable)
                {
                    while (true)
                    {
                        var m1 = e1.MoveNext();
                        var m2 = e2.MoveNext();
                        if (m1 != m2)
                        {
                            throw new AssertFailedException("collection length is not equal. message:" + message);
                        }
                        if (m1 == false && m2 == false) return;

                        var c1 = e1.Current;
                        var c2 = e2.Current;

                        if (comparer.Compare(c1, c2) == 0) throw new AssertFailedException(string.Format("equal index:{0} expected:{1} actual:{2} message:{3}", index, c1, c2, message));
                        index++;
                    }
                }
            }
        }
    }
}
