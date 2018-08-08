using System;
using NUnit.Framework;
using System.Collections.Generic;
using UniRx;
using System.Threading;

namespace UniRx.Tests
{

    public class ObservablePagingTest
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void BufferTimeEmptyComplete()
        {
            var xs = Observable.Empty<int>()
                .Buffer(TimeSpan.FromSeconds(1000))
                .ToArray()
                .Wait();

            xs.Length.Is(1);
        }

        [Test]
        public void BufferEmpty()
        {
            var xs = Observable.Empty<int>()
                .Buffer(10)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }


        [Test]
        public void BufferComplete2()
        {
            var xs = Observable.Range(1, 2)
                .Buffer(2)
                .ToArray()
                .Wait();

            xs.Length.Is(1);
            xs[0].Is(1, 2);
        }

        [Test]
        public void Buffer3()
        {
            var xs = Observable.Empty<int>()
                .Buffer(1, 2)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }

        [Test]
        public void BufferSkip()
        {
            // count > skip
            {
                var xs = Observable.Range(1, 10).Buffer(3, 1)
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
                var xs = Observable.Range(1, 10).Buffer(3, 3)
                    .ToArray()
                    .Wait();

                xs[0].Is(1, 2, 3);
                xs[1].Is(4, 5, 6);
                xs[2].Is(7, 8, 9);
                xs[3].Is(10);
            }

            // count < skip
            {
                var xs = Observable.Range(1, 20).Buffer(3, 5)
                    .ToArray()
                    .Wait();

                xs[0].Is(1, 2, 3);
                xs[1].Is(6, 7, 8);
                xs[2].Is(11, 12, 13);
                xs[3].Is(16, 17, 18);
            }
        }

        [Test]
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

        [Test]
        public void BufferTimeAndCountTimeSide()
        {
            var subject = new Subject<int>();
            var record = subject.Buffer(TimeSpan.FromMilliseconds(100), 100).Take(5).Record();

            Thread.Sleep(TimeSpan.FromSeconds(2));

            record.Values.Count.Is(5);
        }

        [Test]
        public void BufferWindowBoundaries()
        {
            var subject = new Subject<int>();
            var boundaries = new Subject<int>();

            var record = subject.Buffer(boundaries).Record();

            subject.OnNext(1);
            subject.OnNext(2);
            record.Values.Count.Is(0);

            boundaries.OnNext(0);
            record.Values.Count.Is(1);
            record.Values[0].Is(1, 2);

            boundaries.OnNext(0);
            record.Values.Count.Is(2);
            record.Values[1].Count.Is(0);
        }

        [Test]
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

        [Test]
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

        [Test]
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

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.Last(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(20);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(20);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(5);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }

        [Test]
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

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.LastOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(20);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(20);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }

        [Test]
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

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(10);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }

        [Test]
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

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(10);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }
        }

        [Test]
        public void TakeWhile()
        {
            Observable.Range(1, 10)
                .TakeWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .Is(1, 2, 3, 4, 5);
        }

        [Test]
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
                l.Count.Is(3);

                b.OnNext(1000);
                l.Count.Is(3);

                a.OnNext(100);
                l.Count.Is(3);
                l[0].Value.Is(1);
                l[1].Value.Is(10);
            }
        }



        [Test]
        public void Skip()
        {
            Observable.Range(1, 10)
                .Skip(3)
                .ToArray()
                .Wait()
                .Is(4, 5, 6, 7, 8, 9, 10);

            {
                var range = Observable.Range(1, 10);

                Assert.Throws<ArgumentOutOfRangeException>(() => range.Skip(-1));

                range.Skip(10).ToArray().Wait().Length.Is(0);

                range.Skip(3).ToArrayWait().Is(4, 5, 6, 7, 8, 9, 10);
                range.Skip(3).Skip(2).ToArrayWait().Is(6, 7, 8, 9, 10);
            }
        }

        [Test]
        public void SkipTime()
        {
            {
                var now = DateTime.Now;
                var timer = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(10))
                    .Timestamp();

                var v = timer.Skip(TimeSpan.FromMilliseconds(300))
                    .First()
                    .Wait();

                var x = (v.Timestamp - now);
                (TimeSpan.FromMilliseconds(250) <= x && x <= TimeSpan.FromMilliseconds(350)).IsTrue();
            }

            {
                var now = DateTime.Now;
                var timer = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(10))
                    .Timestamp();

                var v = timer
                    .Skip(TimeSpan.FromMilliseconds(100))
                    .Skip(TimeSpan.FromMilliseconds(300))
                    .First()
                    .Wait();

                var x = (v.Timestamp - now);
                (TimeSpan.FromMilliseconds(250) <= x && x <= TimeSpan.FromMilliseconds(350)).IsTrue();
            }
        }

        [Test]
        public void SkipWhile()
        {
            Observable.Range(1, 10)
                .SkipWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .Is(6, 7, 8, 9, 10);
        }

        [Test]
        public void SkipWhileIndex()
        {
            Observable.Range(1, 10)
                .SkipWhile((x, i) => i <= 5)
                .ToArray()
                .Wait()
                .Is(7, 8, 9, 10);
        }


        [Test]
        public void SkipUntil()
        {
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.SkipUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnNext(1000);

                l.Count.Is(0);

                b.OnNext(99999);
                b.HasObservers.IsFalse();

                a.OnNext(1);
                a.OnNext(10);

                l[0].Value.Is(1);
                l[1].Value.Is(10);
            }
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.SkipUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnCompleted();
                l.Count.Is(0);

                a.OnNext(100);
                l.Count.Is(0);
            }
        }

        [Test]
        public void Pairwise()
        {
            var xs = Observable.Range(1, 5).Pairwise((x, y) => x + ":" + y).ToArrayWait();
            xs.Is("1:2", "2:3", "3:4", "4:5");
        }

        [Test]
        public void Pairwise2()
        {
            var xs = Observable.Range(1, 5).Pairwise().ToArrayWait();
            xs[0].Previous.Is(1); xs[0].Current.Is(2);
            xs[1].Previous.Is(2); xs[1].Current.Is(3);
            xs[2].Previous.Is(3); xs[2].Current.Is(4);
            xs[3].Previous.Is(4); xs[3].Current.Is(5);
        }

        [Test]
        public void TakeLast()
        {
            var record = Observable.Range(1, 2).TakeLast(3).Record();
            record.Values.Is(1, 2);

            record = Observable.Range(1, 3).TakeLast(3).Record();
            record.Values.Is(1, 2, 3);

            record = Observable.Range(1, 4).TakeLast(3).Record();
            record.Values.Is(2, 3, 4);

            record = Observable.Range(1, 10).TakeLast(3).Record();
            record.Values.Is(8, 9, 10);

            record = Observable.Empty<int>().TakeLast(3).Record();
            record.Notifications[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [Test]
        public void TakeLastDuration()
        {
            var subject = new Subject<long>();

            var record = subject.Record();
            subject.OnCompleted();
            record.Notifications[0].Kind.Is(NotificationKind.OnCompleted);

            // 0, 200, 400, 600, 800
            var data = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(200))
                .Take(5)
                .TakeLast(TimeSpan.FromMilliseconds(250))
                .ToArrayWait();

            data.Is(3, 4);
        }

        [Test]
        public void GroupBy()
        {
            var subject = new Subject<int>();

            RecordObserver<int> a = null;
            RecordObserver<int> b = null;
            RecordObserver<int> c = null;
            subject.GroupBy(x => x % 3)
                .Subscribe(x =>
                {
                    if (x.Key == 0)
                    {
                        a = x.Record();
                    }
                    else if (x.Key == 1)
                    {
                        b = x.Record();
                    }
                    else if (x.Key == 2)
                    {
                        c = x.Record();
                    }
                });

            subject.OnNext(99);
            subject.OnNext(100);
            subject.OnNext(101);

            subject.OnNext(0);
            subject.OnNext(1);
            subject.OnNext(2);

            a.Values.Is(99, 0);
            b.Values.Is(100, 1);
            c.Values.Is(101, 2);
        }
    }
}
