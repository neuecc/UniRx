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
        public void ContinueWith()
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
        public void ContinueWith2()
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
    }
}
