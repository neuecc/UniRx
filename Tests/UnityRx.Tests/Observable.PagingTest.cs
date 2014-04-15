using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnityRx;

namespace UnityRx.Tests
{
    [TestClass]
    public class ObservablePagingTest
    {
        [TestMethod]
        public void Buffer()
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
        public void BufferTime()
        {
            var hoge = Observable.Return(1000).Delay(TimeSpan.FromSeconds(4));

            var xs = Observable.Range(1, 10)
                .Concat(hoge)
                .Buffer(TimeSpan.FromSeconds(3), Scheduler.CurrentThread)
                .ToArray()
                .Wait();

            xs[0].Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            xs[1].Is(1000);
        }
    }
}
