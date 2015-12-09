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

            range.Scan((x, y) => x + y).ToArrayWait().IsCollection(1, 3, 6, 10, 15);
            range.Scan(100, (x, y) => x + y).ToArrayWait().IsCollection(101, 103, 106, 110, 115);

            Observable.Empty<int>().Scan((x, y) => x + y).ToArrayWait().IsCollection();
            Observable.Empty<int>().Scan(100, (x, y) => x + y).ToArrayWait().IsCollection();
        }

        [TestMethod]
        public void Aggregate()
        {
            AssertEx.Throws<InvalidOperationException>(() => Observable.Empty<int>().Aggregate((x, y) => x + y).Wait());
            Observable.Range(1, 5).Aggregate((x, y) => x + y).Wait().Is(15);

            Observable.Empty<int>().Aggregate(100, (x, y) => x + y).Wait().Is(100);
            Observable.Range(1, 5).Aggregate(100, (x, y) => x + y).Wait().Is(115);

            Observable.Empty<int>().Aggregate(100, (x, y) => x + y, x => x + x).Wait().Is(200);
            Observable.Range(1, 5).Aggregate(100, (x, y) => x + y, x => x + x).Wait().Is(230);
        }
    }
}
