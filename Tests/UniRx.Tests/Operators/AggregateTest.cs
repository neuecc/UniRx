using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class AggregateTest
    {
        [TestMethod]
        public void Scan()
        {
            var range = Observable.Range(1, 5);

            range.Scan((x, y) => x + y).ToArrayWait().Is(1, 3, 6, 10, 15);
            range.Scan(100, (x, y) => x + y).ToArrayWait().Is(101, 103, 106, 110, 115);

            Observable.Empty<int>().Scan((x, y) => x + y).ToArrayWait().Is();
            Observable.Empty<int>().Scan(100, (x, y) => x + y).ToArrayWait().Is();
        }
    }
}
