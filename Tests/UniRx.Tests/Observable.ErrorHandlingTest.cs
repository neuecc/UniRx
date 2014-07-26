using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniRx.Tests
{
    [TestClass]
    public class ErrorHandlingTest
    {
        [TestMethod]
        public void Finally()
        {
            var called = false;
            try
            {
                Observable.Range(1, 10, Scheduler.Immediate)
                    .Do(x => { throw new Exception(); })
                    .Finally(() => called = true)
                    .Subscribe();
            }
            catch
            {
            }
            finally
            {
                called.IsTrue();
            }
        }
    }
}
