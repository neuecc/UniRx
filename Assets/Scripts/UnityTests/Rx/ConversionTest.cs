using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace UniRx.Tests.Operators
{
    
    public class ConversionTest
    {
        [Test]
        public void AsObservable()
        {
            Observable.Range(1, 10).AsObservable().ToArrayWait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [Test]
        public void AsUnitObservable()
        {
            Observable.Range(1, 3).AsUnitObservable().ToArrayWait().Is(Unit.Default, Unit.Default, Unit.Default);
        }

        [Test]
        public void ToObservable()
        {
            Enumerable.Range(1, 3).ToObservable(Scheduler.CurrentThread).ToArrayWait().Is(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.ThreadPool).ToArrayWait().Is(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.Immediate).ToArrayWait().Is(1, 2, 3);
        }

        [Test]
        public void Cast()
        {
            Observable.Range(1, 3).Cast<int, object>().ToArrayWait().Is(1, 2, 3);
        }

        [Test]
        public void OfType()
        {
            var subject = new Subject<object>();

            var list = new List<int>();
            subject.OfType(default(int)).Subscribe(x => list.Add(x));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("hogehoge");
            subject.OnNext(3);

            list.Is(1, 2, 3);
        }

        [Test]
        public void AsSingleUnitObservable()
        {
            var subject = new Subject<int>();

            var done = false;
            subject.AsSingleUnitObservable().Subscribe(_ => done = true);

            subject.OnNext(1);
            done.IsFalse();
            subject.OnNext(100);
            done.IsFalse();
            subject.OnCompleted();
            done.IsTrue();
        }
    }
}
