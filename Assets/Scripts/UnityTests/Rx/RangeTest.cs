using System;
using NUnit.Framework;

namespace UniRx.Tests.Operators
{
    public class RangeTest
    {
        [Test]
        public void Range()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Observable.Range(1, -1).ToArray().Wait());

            Observable.Range(1, 0).ToArray().Wait().Length.Is(0);
            Observable.Range(1, 10).ToArray().Wait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

            Observable.Range(1, 0, Scheduler.Immediate).ToArray().Wait().Length.Is(0);
            Observable.Range(1, 10, Scheduler.Immediate).ToArray().Wait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }
    }
}
