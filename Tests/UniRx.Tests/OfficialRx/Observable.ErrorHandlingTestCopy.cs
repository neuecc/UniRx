using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficialRx
{
    [TestClass]
    public class ErrorHandlingTestRxOfficial
    {
        [TestMethod]
        public void Finally()
        {
            var called = false;
            try
            {
                Observable.Range(1, 10, CurrentThreadScheduler.Instance)
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
