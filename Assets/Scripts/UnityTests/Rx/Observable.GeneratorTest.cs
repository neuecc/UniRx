using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace UniRx.Tests
{
    
    public class ObservableGeneratorTest
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
        public void Empty()
        {
            var material = Observable.Empty<Unit>().Materialize().ToArray().Wait();
            material.Is(Notification.CreateOnCompleted<Unit>());
        }

        [Test]
        public void Never()
        {
            Assert.Catch<TimeoutException>(() =>
                Observable.Never<Unit>().Materialize().ToArray().Wait(TimeSpan.FromMilliseconds(10)));
        }

        [Test]
        public void Return()
        {
            Observable.Return(100).Materialize().ToArray().Wait().Is(Notification.CreateOnNext(100), Notification.CreateOnCompleted<int>());
        }

        [Test]
        public void Range()
        {
            Observable.Range(1, 5).ToArray().Wait().Is(1, 2, 3, 4, 5);
        }

        [Test]
        public void Repeat()
        {
            var xs = Observable.Range(1, 3, Scheduler.CurrentThread)
                .Concat(Observable.Return(100))
                .Repeat()
                .Take(10)
                .ToArray()
                .Wait();
            xs.Is(1, 2, 3, 100, 1, 2, 3, 100, 1, 2);
            Observable.Repeat(100).Take(5).ToArray().Wait().Is(100, 100, 100, 100, 100);
        }

        [Test]
        public void Repeat2()
        {
            Observable.Repeat("a", 5, Scheduler.Immediate).ToArrayWait().Is("a", "a", "a", "a", "a");
        }

        [Test]
        public void RepeatStatic()
        {
            var xss = Observable.Repeat(5, 3).ToArray().Wait();
            xss.Is(5, 5, 5);
        }

        [Test]
        public void ToObservable()
        {
            {
                var msgs = new List<string>();
                new[] { 1, 10, 100, 1000, 10000, 20000 }.ToObservable(Scheduler.CurrentThread)
                    .Do(i => msgs.Add("DO:" + i))
                    .Scan((x, y) =>
                    {
                        if (y == 100) throw new Exception("exception");
                        msgs.Add("x:" + x + " y:" + y);
                        return x + y;
                    })
                    .Subscribe(x => msgs.Add(x.ToString()), e => msgs.Add(e.Message), () => msgs.Add("comp"));

                msgs.Is("DO:1", "1", "DO:10", "x:1 y:10", "11", "DO:100", "exception");
            }

            {
                var msgs = new List<string>();
                new[] { 1, 10, 100, 1000, 10000, 20000 }.ToObservable(Scheduler.Immediate)
                    .Do(i => msgs.Add("DO:" + i))
                    .Scan((x, y) =>
                    {
                        if (y == 100) throw new Exception("exception");
                        msgs.Add("x:" + x + " y:" + y);
                        return x + y;
                    })
                    .Subscribe(x => msgs.Add(x.ToString()), e => msgs.Add(e.Message), () => msgs.Add("comp"));

                msgs.Is("DO:1", "1", "DO:10", "x:1 y:10", "11", "DO:100", "exception",
                    "DO:1000", "x:11 y:1000",
                    "DO:10000", "x:1011 y:10000",
                    "DO:20000", "x:11011 y:20000"
                    );
            }
        }

        [Test]
        public void Throw()
        {
            var ex = new Exception();
            Observable.Throw<string>(ex).Materialize().ToArray().Wait().Is(Notification.CreateOnError<string>(ex));
        }

        [Test]
        public void OptimizeReturnTest()
        {
            for (int i = -1; i <= 9; i++)
            {
                var r = Observable.Return(i);
                var xs = r.Record();
                xs.Values[0].Is(i);
                r.GetType().FullName.Contains("ImmutableReturnInt32Observable").IsTrue();
            }
            foreach (var i in new[] { -2, 10, 100 })
            {
                var r = Observable.Return(i);
                r.Record().Values[0].Is(i);
                r.GetType().FullName.Contains("ImmediateReturnObservable").IsTrue();
            }
        }
    }
}
