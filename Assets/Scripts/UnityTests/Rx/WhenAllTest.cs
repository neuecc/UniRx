using System;
using System.Linq;
using NUnit.Framework;

namespace UniRx.Tests.Operators
{
    public class WhenAllTest
    {
        [SetUp]
        public void Init()
        {
            TestUtil.SetScehdulerForImport();
        }

        [TearDown]
        public void Dispose()
        {
            UniRx.Scheduler.SetDefaultForUnity();
        }

        [Test]
        public void WhenAllEmpty()
        {
            var xs = Observable.WhenAll(new IObservable<int>[0]).Wait();
            xs.Length.Is(0);

            var xs2 = Observable.WhenAll(Enumerable.Empty<IObservable<int>>().Select(x => x)).Wait();
            xs2.Length.Is(0);
        }

        [Test]
        public void WhenAll()
        {
            var xs = Observable.WhenAll(
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4))
                .Wait();

            xs.Is(100, 5, 4);
        }

        [Test]
        public void WhenAllEnumerable()
        {
            var xs = new[] {
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4)
            }.Select(x => x).WhenAll().Wait();

            xs.Is(100, 5, 4);
        }

        [Test]
        public void WhenAllUnitEmpty()
        {
            var xs = Observable.WhenAll(new IObservable<Unit>[0]).Wait();
            xs.Is(Unit.Default);

            var xs2 = Observable.WhenAll(Enumerable.Empty<IObservable<Unit>>().Select(x => x)).Wait();
            xs2.Is(Unit.Default);
        }

        [Test]
        public void WhenAllUnit()
        {
            var xs = Observable.WhenAll(
                    Observable.Return(100).AsUnitObservable(),
                    Observable.Timer(TimeSpan.FromSeconds(1)).AsUnitObservable(),
                    Observable.Range(1, 4).AsUnitObservable())
                .Wait();

            xs.Is(Unit.Default);
        }

        [Test]
        public void WhenAllUnitEnumerable()
        {
            var xs = new[] {
                    Observable.Return(100).AsUnitObservable(),
                    Observable.Timer(TimeSpan.FromSeconds(1)).AsUnitObservable(),
                    Observable.Range(1, 4).AsUnitObservable()
            }.Select(x => x).WhenAll().Wait();

            xs.Is(Unit.Default);
        }
    }
}
