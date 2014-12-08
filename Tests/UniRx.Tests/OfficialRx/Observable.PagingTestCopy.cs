using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive;

namespace OfficialRx
{
    [TestClass]
    public class ObservablePagingTestOfficialRx
    {
        [TestMethod]
        public void BufferOfficialRx()
        {
            var xs = Observable.Range(1, 10)
                .Buffer(3)
                .ToArray()
                .Wait();
            xs[0].Is(1, 2, 3);
            xs[1].Is(4, 5, 6);
            xs[2].Is(7, 8, 9);
            xs[3].Is(10);
        }

        [TestMethod]
        public void BufferTimeOfficialRx()
        {
            var hoge = Observable.Return(1000).Delay(TimeSpan.FromSeconds(4));

            var xs = Observable.Range(1, 10)
                .Concat(hoge)
                .Buffer(TimeSpan.FromSeconds(3), Scheduler.CurrentThread)
                .ToArray()
                .Wait();

            xs[0].Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            xs[1].Is(1000);
        }

        [TestMethod]
        public void BufferTimeEmptyBufferOfficialRx()
        {
            var xs = Observable.Return(10).Delay(TimeSpan.FromMilliseconds(3500))
                .Buffer(TimeSpan.FromSeconds(1))
                .ToArray()
                .Wait();

            xs.Length.Is(4);
            xs[0].Count.Is(0); // 1sec
            xs[1].Count.Is(0); // 2sec
            xs[2].Count.Is(0); // 3sec
            xs[3].Count.Is(1); // 4sec
        }

        [TestMethod]
        public void BufferTimeCompleteOfficialRx()
        {
            // when complete, return empty array.
            var xs = Observable.Return(1).Delay(TimeSpan.FromSeconds(2))
                .Concat(Observable.Return(1).Delay(TimeSpan.FromSeconds(2)).Skip(1))
                .Buffer(TimeSpan.FromSeconds(3))
                .ToArray()
                .Wait();

            xs.Length.Is(2);
            xs[0].Count.Is(1);
            xs[1].Count.Is(0);
        }

        [TestMethod]
        public void BufferTimeEmptyCompleteOfficialRx()
        {
            var xs = Observable.Empty<int>()
                .Buffer(TimeSpan.FromSeconds(1))
                .ToArray()
                .Wait();

            xs.Length.Is(1);
        }

        [TestMethod]
        public void BufferEmptyOfficialRx()
        {
            var xs = Observable.Empty<int>()
                .Buffer(10)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }

        [TestMethod]
        public void BufferComplete2OfficialRx()
        {
            var xs = Observable.Range(1, 2)
                .Buffer(2)
                .ToArray()
                .Wait();

            xs.Length.Is(1);
            xs[0].Is(1, 2);
        }

        [TestMethod]
        public void Buffer3OfficialRx()
        {
            var xs = Observable.Empty<int>()
                .Buffer(1, 2)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }


        [TestMethod]
        public void BufferSkipRxOfficial()
        {
            // count > skip
            {
                var xs = Observable.Range(1, 10).Buffer(count: 3, skip: 1)
                    .ToArray()
                    .Wait();

                xs[0].Is(1, 2, 3);
                xs[1].Is(2, 3, 4);
                xs[2].Is(3, 4, 5);
                xs[3].Is(4, 5, 6);
                xs[4].Is(5, 6, 7);
                xs[5].Is(6, 7, 8);
                xs[6].Is(7, 8, 9);
                xs[7].Is(8, 9, 10);
                xs[8].Is(9, 10);
                xs[9].Is(10);
            }

            // count == skip
            {
                var xs = Observable.Range(1, 10).Buffer(count: 3, skip: 3)
                    .ToArray()
                    .Wait();

                xs[0].Is(1, 2, 3);
                xs[1].Is(4, 5, 6);
                xs[2].Is(7, 8, 9);
                xs[3].Is(10);
            }

            // count < skip
            {
                var xs = Observable.Range(1, 20).Buffer(count: 3, skip: 5)
                    .ToArray()
                    .Wait();

                xs[0].Is(1, 2, 3);
                xs[1].Is(6, 7, 8);
                xs[2].Is(11, 12, 13);
                xs[3].Is(16, 17, 18);
            }
        }

        [TestMethod]
        public void BufferTimeAndCountOfficialRx()
        {
            // time...count...complete
            {
                var xs = Observable.Return(1000)
                    .Concat(Observable.Return(99).Delay(TimeSpan.FromMilliseconds(1500)))
                    .Concat(Observable.Range(1, 7))
                    .Buffer(TimeSpan.FromSeconds(1), 5)
                    .ToArray()
                    .Wait();

                xs.Length.Is(3);
                xs[0].Is(1000); // 1sec
                xs[1].Is(99, 1, 2, 3, 4); // 1.5sec -> buffer
                xs[2].Is(5, 6, 7); // next 1sec
            }
            // time...count...time
            {
                var xs = Observable.Return(1000)
                    .Concat(Observable.Return(99).Delay(TimeSpan.FromMilliseconds(1500)))
                    .Concat(Observable.Range(1, 7))
                    .Concat(Observable.Never<int>())
                    .Buffer(TimeSpan.FromSeconds(1), 5)
                    .Take(3)
                    .ToArray()
                    .Wait();

                xs.Length.Is(3);
                xs[0].Is(1000); // 1sec
                xs[1].Is(99, 1, 2, 3, 4); // 1.5sec -> buffer
                xs[2].Is(5, 6, 7); // next 1sec
            }
            // time(before is canceled)
            {
                var start = DateTime.Now;
                var result = Observable.Return(10).Delay(TimeSpan.FromSeconds(2))
                    .Concat(Observable.Range(1, 2))
                    .Concat(Observable.Return(1000).Delay(TimeSpan.FromSeconds(2)))
                    .Concat(Observable.Never<int>())
                    .Buffer(TimeSpan.FromSeconds(3), 3)
                    .Take(2)
                    .Select(xs =>
                    {
                        var currentSpan = DateTime.Now - start;
                        return new { currentSpan, xs };
                    })
                    .ToArray()
                    .Wait();

                // after 2 seconds, buffer is flush by count
                result[0].xs.Is(10, 1, 2);
                result[0].currentSpan.Is(x => TimeSpan.FromMilliseconds(1800) <= x && x <= TimeSpan.FromMilliseconds(2200));

                // after 3 seconds, buffer is flush by time
                result[1].xs.Is(1000);
                result[1].currentSpan.Is(x => TimeSpan.FromMilliseconds(4800) <= x && x <= TimeSpan.FromMilliseconds(5200));
            }
        }

        [TestMethod]
        public void TakeUntilRxOfficial()
        {
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.TakeUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnNext(1000);
                l.Count.Is(3);
                l[0].Value.Is(1);
                l[1].Value.Is(10);
                l[2].Kind.Is(NotificationKind.OnCompleted);
            }
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.TakeUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnCompleted();
                l.Count.Is(2);

                b.OnNext(1000);
                l.Count.Is(2);

                a.OnNext(100);
                l.Count.Is(3);
                l[0].Value.Is(1);
                l[1].Value.Is(10);
                l[2].Value.Is(100);
            }
        }

        [TestMethod]
        public void SkipRxOfficial()
        {
            Observable.Range(1, 10)
                .Skip(3)
                .ToArray()
                .Wait()
                .Is(4, 5, 6, 7, 8, 9, 10);
        }
    }
}