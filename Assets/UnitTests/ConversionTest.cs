using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [TestClass]
    public class ConversionTest
    {
        [TestMethod]
        public void AsObservable()
        {
            Observable.Range(1, 10).AsObservable().ToArrayWait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [TestMethod]
        public void AsUnitObservable()
        {
            Observable.Range(1, 3).AsUnitObservable().ToArrayWait().IsCollection(Unit.Default, Unit.Default, Unit.Default);
        }

        [TestMethod]
        public void ToObservable()
        {
            Enumerable.Range(1, 3).ToObservable(Scheduler.CurrentThread).ToArrayWait().IsCollection(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.ThreadPool).ToArrayWait().IsCollection(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.Immediate).ToArrayWait().IsCollection(1, 2, 3);
        }

        [TestMethod]
        public void Cast()
        {
            Observable.Range(1, 3).Cast<int, object>().ToArrayWait().IsCollection(1, 2, 3);
        }

        [TestMethod]
        public void OfType()
        {
            var subject = new Subject<object>();

            var list = new List<int>();
            subject.OfType(default(int)).Subscribe(x => list.Add(x));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("hogehoge");
            subject.OnNext(3);

            list.IsCollection(1, 2, 3);
        }
    }
}
