using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class RangeTest
    {
        [TestMethod]
        public void Range()
        {
            AssertEx.Throws<ArgumentOutOfRangeException>(() => Observable.Range(1, -1).ToArray().Wait());

            Observable.Range(1, 0).ToArray().Wait().Length.Is(0);
            Observable.Range(1, 10).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }
    }
}
