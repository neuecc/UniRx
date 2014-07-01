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
        public void Buffer2OfficialRx()
        {
            var xs = Observable.Range(1, 10)
                .Buffer(TimeSpan.FromSeconds(3), Scheduler.CurrentThread)
                .ToArray()
                .Wait();

            xs[0].Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
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