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
                .Buffer(TimeSpan.FromSeconds(3))
                .ToArray()
                .Wait();

            xs[0].Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            xs[1].Is(1000);
        }

        [TestMethod]
        public void BufferTimeEmptyBuffer()
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
        public void BufferTimeComplete()
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
        public void BufferTimeEmptyComplete()
        {
            var xs = Observable.Empty<int>()
                .Buffer(TimeSpan.FromSeconds(1000))
                .ToArray()
                .Wait();

            xs.Length.Is(1);
        }

        [TestMethod]
        public void BufferEmpty()
        {
            var xs = Observable.Empty<int>()
                .Buffer(10)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }


        [TestMethod]
        public void BufferComplete2()
        {
            var xs = Observable.Range(1, 2)
                .Buffer(2)
                .ToArray()
                .Wait();

            xs.Length.Is(1);
            xs[0].Is(1, 2);
        }

        [TestMethod]
        public void Buffer3()
        {
            var xs = Observable.Empty<int>()
                .Buffer(1, 2)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
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
        public void BufferTimeAndCount()
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

            s = new Subject<int>();
            l.Clear();
            {
                s.First(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnError(new Exception());
                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);
                s.OnError(new Exception());
                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
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

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
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
