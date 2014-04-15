using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace OfficialRx
{
    [TestClass]
    public class ObservablePagingTestOfficialRx
    {
        [TestMethod]
        public void BufferOfficialRx()
        {
            var xs = Observable.Range(1, 10)
                .Buffer(3)
                .ToArray()
                .Wait();
            xs[0].Is(1, 2, 3);
            xs[1].Is(4, 5, 6);
            xs[2].Is(7, 8, 9);
            xs[3].Is(10);
        }

        [TestMethod]
        public void Buffer2OfficialRx()
        {
            var xs = Observable.Range(1, 10)
                .Buffer(TimeSpan.FromSeconds(3), Scheduler.CurrentThread)
                .ToArray()
                .Wait();

            xs[0].Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

    }
}