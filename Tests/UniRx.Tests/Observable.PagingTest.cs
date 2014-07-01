using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UniRx;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservablePagingTest
    {
        [TestMethod]
        public void Buffer()
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
        public void BufferTime()
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
        public void BufferSkip()
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
        public void First()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.First().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First().Materialize().Subscribe(l.Add);

                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }

        [TestMethod]
        public void FirstOrDefault()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.FirstOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault().Materialize().Subscribe(l.Add);

                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }
        }

        [TestMethod]
        public void Last()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.Last().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnCompleted();

                l[0].Value.Is(30);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }

        [TestMethod]
        public void LastOrDefault()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.LastOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnCompleted();

                l[0].Value.Is(30);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }
        }

        [TestMethod]
        public void Single()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }

        [TestMethod]
        public void SingleOrDefault()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }
        }

        [TestMethod]
        public void TakeWhile()
        {
            Observable.Range(1, 10)
                .TakeWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .Is(1, 2, 3, 4, 5);
        }

        [TestMethod]
        public void TakeUntil()
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
        public void Skip()
        {
            Observable.Range(1, 10)
                .Skip(3)
                .ToArray()
                .Wait()
                .Is(4, 5, 6, 7, 8, 9, 10);
        }

        [TestMethod]
        public void SkipWhile()
        {
            Observable.Range(1, 10)
                .SkipWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .Is(6, 7, 8, 9, 10);
        }
    }
}
