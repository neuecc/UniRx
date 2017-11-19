using System;
using System.Collections;


namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class TestClassAttribute : Attribute
    {
    }

    public class TestMethodAttribute : Attribute
    {
    }
}

namespace RuntimeUnitTestToolkit
{
    public static partial class AssertEx
    {
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
}