using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservableGeneratorTest
    {
        [TestMethod]
        public void Empty()
        {
            var material = Observable.Empty<Unit>().Materialize().ToArray().Wait();
            material.Is(Notification.CreateOnCompleted<Unit>());
        }

        [TestMethod]
        public void Never()
        {
            AssertEx.Catch<TimeoutException>(() =>
                Observable.Never<Unit>().Materialize().ToArray().Wait(TimeSpan.FromMilliseconds(10)));
        }

        [TestMethod]
        public void Return()
        {
            Observable.Return(100).Materialize().ToArray().Wait().Is(Notification.CreateOnNext(100), Notification.CreateOnCompleted<int>());
        }

        [TestMethod]
        public void Range()
        {
            Observable.Range(1, 5).ToArray().Wait().Is(1, 2, 3, 4, 5);
        }

        [TestMethod]
        public void Repeat()
        {
            Observable.Range(1, 3, Scheduler.CurrentThread)
                .Concat(Observable.Return(100))
                .Repeat()
                .Take(10)
                .ToArray()
                .Wait()
                .Is(1, 2, 3, 100, 1, 2, 3, 100, 1, 2);
            Observable.Repeat(100).Take(5).ToArray().Wait().Is(100, 100, 100, 100, 100);

            Observable.Repeat(5, 3).ToArray().Wait().Is(5, 5, 5);
        }

        [TestMethod]
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

        [TestMethod]
        public void Throw()
        {
            var ex = new Exception();
            Observable.Throw<string>(ex).Materialize().ToArray().Wait().Is(Notification.CreateOnError<string>(ex));
        }
    }
}
