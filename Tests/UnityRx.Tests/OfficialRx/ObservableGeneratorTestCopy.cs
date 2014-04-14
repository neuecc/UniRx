using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive;

namespace OfficialRx
{
    [TestClass]
    public class ObservableGeneratorTestCopy
    {
        [TestMethod]
        public void EmptyRxOfficial()
        {
            var material = Observable.Empty<Unit>().Materialize().ToArray().Wait();
            material.Is(Notification.CreateOnCompleted<Unit>());
        }

        [TestMethod]
        public void ReturnRxOfficia()
        {
            Observable.Return(100).Materialize().ToArray().Wait().Is(Notification.CreateOnNext(100), Notification.CreateOnCompleted<int>());
        }

        [TestMethod]
        public void ToObservableTestRxOfficial()
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
        public void RepeatRxOfficial()
        {
            Observable.Range(3, 2, Scheduler.CurrentThread).Repeat().Take(10).ToArray().Wait().Is(3, 4, 3, 4, 3, 4, 3, 4, 3, 4);
        }
    }
}
