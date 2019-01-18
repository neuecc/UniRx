using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace UniRx.Tests.Operators
{

    public class ContinueWithTest
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
        public void ContinueWith_PublishesLastValueAndCompletes()
        {
            var subject = new Subject<int>();

            var record = subject.ContinueWith(x => Observable.Return(x)).Record();

            subject.OnNext(10);
            record.Values.Count.Is(0);

            subject.OnNext(100);
            record.Values.Count.Is(0);

            subject.OnCompleted();
            record.Values[0].Is(100);
            record.Notifications.Last().Kind.Is(NotificationKind.OnCompleted);
        }
        
        [Test]
        public void ContinueWith_PublishesLastValueAndCompletes_WithDelay()
        {
            var subject = new Subject<int>();

            var record = subject.ContinueWith(x => Observable.Return(x).Delay(TimeSpan.FromMilliseconds(100))).Record();

            subject.OnNext(10);
            record.Values.Count.Is(0);

            subject.OnNext(100);
            record.Values.Count.Is(0);

            subject.OnCompleted();
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            record.Values[0].Is(100);
            record.Notifications.Last().Kind.Is(NotificationKind.OnCompleted);
        }

        [Test]
        public void ContinueWith_Throws_WhenErrorOriginatesFromOnCompleted()
        {
            Assert.Throws<InvalidOperationException>(() =>
                PublishErrorInOnCompleted().Subscribe());
        }

        [Test]
        public void ContinueWith_PublishesError_WhenErrorOriginatesFromOnCompleted()
        {
            Exception ex = null;
            PublishErrorInOnCompleted().Subscribe(_ => { }, e => ex = e);
            Assert.IsNotNull(ex);
        }

        [Test]
        public void ContinueWith_PublishesError_WhenTrowingFromSelector()
        {
            Exception ex = null;
            Observable
                .ReturnUnit()
                .ContinueWith<Unit, Unit>(
                    _ =>
                    {
                        throw new NotImplementedException();
                    })
                .Subscribe(_ => { }, e => ex = e);
            Assert.IsNotNull(ex);
        }

        private IObservable<Unit> PublishErrorInOnCompleted()
        {
            // This is a special case where calling OnCompleted will publish an error.
            // Using Empty and Single here was the simplest solution, but something else could
            // be used also.
            return Observable
                .ReturnUnit()
                .ContinueWith(_ => Observable.Empty<Unit>())
                .Single();
        }
    }
}
