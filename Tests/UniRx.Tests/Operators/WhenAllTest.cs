using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class WhenAllTest
    {
        [TestMethod]
        public void WhenAllEmpty()
        {
            var xs = Observable.WhenAll(new IObservable<int>[0]).Wait();
            xs.Length.Is(0);

            var xs2 = Observable.WhenAll(Enumerable.Empty<IObservable<int>>().Select(x => x)).Wait();
            xs2.Length.Is(0);
        }

        [TestMethod]
        public void WhenAll()
        {
            var xs = Observable.WhenAll(
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4))
                .Wait();

            xs.Is(100, 5, 4);
        }

        [TestMethod]
        public void WhenAllEnumerable()
        {
            var xs = new[] {
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4)
            }.Select(x => x).WhenAll().Wait();
                
            xs.Is(100, 5, 4);
        }
    }
}
