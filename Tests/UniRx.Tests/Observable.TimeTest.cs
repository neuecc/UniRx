using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservableTimeTest
    {
        [TestMethod]
        public void TimerTest()
        {
            // periodic(Observable.Interval)
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Timestamp()
                    .ToArray()
                    .Wait();

                xs[0].Value.Is(0L);
                (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

                xs[1].Value.Is(1L);
                (now.AddMilliseconds(1800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(2200)).IsTrue();

                xs[2].Value.Is(2L);
                (now.AddMilliseconds(2800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(3200)).IsTrue();
            }

            // dueTime + periodic
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
                xs[1].Is(3);
                xs[2].Is(4);
            }

            // dueTime(DateTimeOffset)
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(now.AddSeconds(2), TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
                xs[1].Is(3);
                xs[2].Is(4);
            }

            // onetime
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(2))
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
            }

            // non periodic scheduler
            {
                var now = Scheduler.CurrentThread.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), Scheduler.CurrentThread)
                    .Take(3)
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
                xs[1].Is(3);
                xs[2].Is(4);
            }
        }

        [TestMethod]
        public void DelayTest()
        {
            var now = Scheduler.ThreadPool.Now;

            var xs = Observable.Range(1, 3)
                .Delay(TimeSpan.FromSeconds(1))
                .Timestamp()
                .ToArray()
                .Wait();

            xs[0].Value.Is(1);
            (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[1].Value.Is(2);
            (now.AddMilliseconds(800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[2].Value.Is(3);
            (now.AddMilliseconds(800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(1200)).IsTrue();
        }

        [TestMethod]
        public void SampleTest()
        {
            // 2400, 4800, 7200, 9600
            var xs = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                .Take(10)
                .Sample(TimeSpan.FromMilliseconds(2400), Scheduler.CurrentThread)
                .Timestamp()
                .ToArray()
                .Wait();

            xs[0].Value.Is(2);
            xs[1].Value.Is(4);
            xs[2].Value.Is(7);
            xs[3].Value.Is(9);
        }

        [TestMethod]
        public void TimeoutTest()
        {
            var xs = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(9).Delay(TimeSpan.FromSeconds(3))
                )
                .Timestamp()
                .Timeout(TimeSpan.FromMilliseconds(1500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(2);
            xs[0].Value.Value.Is(1);
            xs[1].Exception.IsInstanceOf<TimeoutException>();
        }

        [TestMethod]
        public void TimeoutTestOffset()
        {
            var now = Scheduler.ThreadPool.Now;
            var xs = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(9).Delay(TimeSpan.FromSeconds(3))
                )
                .Timestamp()
                .Timeout(now.AddMilliseconds(3500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(3);
            xs[0].Value.Value.Is(1);
            xs[1].Value.Value.Is(5);
            xs[2].Exception.IsInstanceOf<TimeoutException>();
        }

        [TestMethod]
        public void ThrottleTest()
        {
            var xs = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(4).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(6).Delay(TimeSpan.FromSeconds(3)), // over 2500
                    Observable.Return(7).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(8).Delay(TimeSpan.FromSeconds(1)) // with onCompleted
                )
                .Timestamp()
                .Throttle(TimeSpan.FromMilliseconds(2500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(3);
            xs[0].Value.Value.Is(5);
            xs[1].Value.Value.Is(8);
            xs[2].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ThrottleFirstTest()
        {
            var xs = Observable.Concat(
                    Observable.Return(1),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(4).Delay(TimeSpan.FromSeconds(0.4)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(0.2)), // over 2500
                    Observable.Return(6).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(7).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(8).Delay(TimeSpan.FromSeconds(1)), // over 2500
                    Observable.Return(9) // withCompleted
                )
                .Timestamp()
                .ThrottleFirst(TimeSpan.FromMilliseconds(2500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(4);
            xs[0].Value.Value.Is(1);
            xs[1].Value.Value.Is(5);
            xs[2].Value.Value.Is(8);
            xs[3].Kind.Is(NotificationKind.OnCompleted);
        }
    }
}