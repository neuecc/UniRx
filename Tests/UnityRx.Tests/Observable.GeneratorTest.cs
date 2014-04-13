using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnityRx.Tests
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
        public void ToObservable()
        {
            {
                var msgs = new List<string>();
                new[] { 1, 10, 100, 1000, 10000, 20000 }.ToObservable(Scheduler.CurrentThread)
                    .Do(i => msgs.Add("DO:" + i))
                    .Scan((x, y) =>
                    {
                        if (y == 100) throw new Exception("execption");
                        msgs.Add("x:" + x + " y:" + y);
                        return x + y;
                    })
                    .Subscribe(x => msgs.Add(x.ToString()), e => msgs.Add(e.Message), () => msgs.Add("comp"));

                Console.WriteLine(string.Join(",", msgs));
            }

            //{
            //    var msgs = new List<string>();
            //    new[] { 1, 10, 100, 1000, 10000, 20000 }.ToObservable(Scheduler.Immediate)
            //        .Do(i => msgs.Add("DO:" + i))
            //        .Scan((x, y) =>
            //        {
            //            if (y == 100) throw new Exception("execption");
            //            msgs.Add("x:" + x + " y:" + y);
            //            return x + y;
            //        })
            //        .Subscribe(x => msgs.Add(x.ToString()), e => msgs.Add(e.Message), () => msgs.Add("comp"));

            //    Console.WriteLine(string.Join(",", msgs));
            //}
        }
    }
}
