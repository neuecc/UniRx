using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservableConcatTest
    {
        [TestMethod]
        public void Concat()
        {
            var a = Observable.Range(1, 5, Scheduler.ThreadPool);
            var b = Observable.Range(10, 3, Scheduler.ThreadPool);
            var c = Observable.Return(300, Scheduler.ThreadPool);

            Observable.Concat(a, b, c).ToArray().Wait().Is(1, 2, 3, 4, 5, 10, 11, 12, 300);
        }

        [TestMethod]
        public void Zip()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.Zip(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            a.OnNext(1001);
            l.Count.Is(1);

            b.OnNext(5);
            l.Count.Is(2); // Completed!

            l[1].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void Zip2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.Zip(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            b.OnCompleted(); // Completed!

            l.Count.Is(2); // Completed!
            l[1].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ZipMulti()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.Zip(a, b).Select(xs => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));


            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            a.OnNext(1001);
            l.Count.Is(1);

            b.OnNext(5);
            l.Count.Is(2); // Completed!

            l[1].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ZipMulti2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.Zip(a, b).Select(xs => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            b.OnCompleted(); // Completed!

            l.Count.Is(2); // Completed!
            l[1].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void WhenAll()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.WhenAll(a, b).Select(xs => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            a.OnNext(1500);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(0);

            a.OnNext(1001);
            l.Count.Is(0);

            b.OnNext(5);
            b.OnCompleted();

            l.Count.Is(2); // Completed!

            l[0].Value.x.Is(1500);
            l[0].Value.y.Is(5);
            l[1].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatest()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });

            b.OnNext(3000);
            l[1].Value.Is(new { x = 1000, y = 3000 });

            a.OnNext(5000);
            l[2].Value.Is(new { x = 5000, y = 3000 });

            a.OnCompleted();
            l.Count.Is(3);

            a.OnNext(1001);
            l.Count.Is(3);

            b.OnNext(5);
            l[3].Value.Is(new { x = 5000, y = 5 });
            b.OnNext(500);
            l[4].Value.Is(new { x = 5000, y = 500 });

            b.OnCompleted();
            l[5].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatest2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            b.OnNext(2000);
            a.OnCompleted();

            l.Count.Is(0);

            b.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatest3()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(2000);
            b.OnCompleted();

            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatest4()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            b.OnCompleted();
            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatestMulti()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });

            b.OnNext(3000);
            l[1].Value.Is(new { x = 1000, y = 3000 });

            a.OnNext(5000);
            l[2].Value.Is(new { x = 5000, y = 3000 });

            a.OnCompleted();
            l.Count.Is(3);

            a.OnNext(1001);
            l.Count.Is(3);

            b.OnNext(5);
            l[3].Value.Is(new { x = 5000, y = 5 });
            b.OnNext(500);
            l[4].Value.Is(new { x = 5000, y = 500 });

            b.OnCompleted();
            l[5].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatestMulti2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            b.OnNext(2000);
            a.OnCompleted();

            l.Count.Is(0);

            b.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatestMulti3()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(2000);
            b.OnCompleted();

            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void CombineLatestMulti4()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            b.OnCompleted();
            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }
    }
}
