using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnityRx.Tests
{
    [TestClass]
    public class ObservableConcatTest
    {
        [TestMethod]
        public void Concat()
        {
            var a = Observable.Range(1, 5, Scheduler.ThreadPool);
            var b = Observable.Range(10, 3, Scheduler.ThreadPool);
            var c = Observable.Return(300, Scheduler.ThreadPool);

            Observable.Concat(a, b, c).ToArray().Wait().Is(1, 2, 3, 4, 5, 10, 11, 12, 300);
        }
        
    }
}
