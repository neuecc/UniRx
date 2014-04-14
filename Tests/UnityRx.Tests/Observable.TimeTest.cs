using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnityRx.Tests
{
    [TestClass]
    public class ObservableTimeTest
    {
        [TestMethod]
        public void TimerTest()
        {
            // 
            var now = Scheduler.ThreadPool.Now;
            var xs = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                .Take(3)
                .Timestamp()
                .ToArray()
                .Wait();

            xs[0].Value.Is(0L);
            (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[1].Value.Is(1L);
            (now.AddMilliseconds(1800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(2200)).IsTrue();

            xs[2].Value.Is(2L);
            (now.AddMilliseconds(2800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(3200)).IsTrue();
        }

        [TestMethod]
        public void DelayTest()
        {
            var now = Scheduler.ThreadPool.Now;

            var xs = Observable.Range(1, 3)
                .Delay(TimeSpan.FromSeconds(1))
                .Timestamp()
                .ToArray()
                .Wait();

            xs[0].Value.Is(1);
            (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[1].Value.Is(2);
            (now.AddMilliseconds(800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(1200)).IsTrue();
         
            xs[2].Value.Is(3);
            (now.AddMilliseconds(800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(1200)).IsTrue();
        }
    }
}