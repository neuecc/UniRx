using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservableTest
    {
        [TestMethod]
        public void ToArray()
        {
            var subject = new Subject<int>();

            int[] array = null;
            subject.ToArray().Subscribe(xs => array = xs);

            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);
            subject.OnCompleted();

            array.IsCollection(1, 10, 100);
        }

        [TestMethod]
        public void ToArray_Dispose()
        {
            var subject = new Subject<int>();

            int[] array = null;
            var disp = subject.ToArray().Subscribe(xs => array = xs);

            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);

            disp.Dispose();

            subject.OnCompleted();

            array.IsNull();
        }

        [TestMethod]
        public void Wait()
        {
#if !UNITY_METRO

            var subject = new Subject<int>();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                subject.OnNext(100);
                subject.OnCompleted();
            });

            subject.Wait().Is(100);

#endif
        }

        [TestMethod]
        public void DistinctUntilChanged()
        {
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.DistinctUntilChanged().ToArray().Subscribe(xs => array = xs);

                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5 }) { subject.OnNext(item); };
                subject.OnCompleted();

                array.IsCollection(1, 10, 1, 100, 5);
            }
            {

                string[] array = null;
                new[] { "hoge", "huga", null, null, "huga", "huga", "hoge" }
                    .ToObservable()
                    .DistinctUntilChanged()
                    .ToArray()
                    .Subscribe(xs => array = xs);

                array.IsCollection("hoge", "huga", null, "huga", "hoge");
            }
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.DistinctUntilChanged(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);

                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5 }) { subject.OnNext(item); };
                subject.OnCompleted();

                array.IsCollection(1, 10, 1, 100, 5);
            }
        }

        [TestMethod]
        public void Distinct()
        {
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.Distinct().ToArray().Subscribe(xs => array = xs);

                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }) { subject.OnNext(item); };
                subject.OnCompleted();

                array.IsCollection(1, 10, 100, 5, 70, 7);
            }
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.Distinct(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);
                       
                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }) { subject.OnNext(item); };
                subject.OnCompleted();

                array.IsCollection(1, 10, 100, 5, 70, 7);
            }
        }

        [TestMethod]
        public void SelectMany()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            var list = new List<int>();
            a.SelectMany(_ => b).Subscribe(x => list.Add(x));

            a.OnNext(10);
            a.OnCompleted();
            b.OnNext(100);

            list.Count.Is(1);
            list[0].Is(100);
        }

        [TestMethod]
        public void Select()
        {
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select(x => x * x).Subscribe(x => list.Add(x));

                a.OnNext(100);
                a.OnNext(200);
                a.OnNext(300);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(10000, 40000, 90000);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select((x, i) => x * i).Subscribe(x => list.Add(x));

                a.OnNext(100);
                a.OnNext(200);
                a.OnNext(300);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(0, 200, 600);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select(x => x * x).Select(x => x * x).Subscribe(x => list.Add(x));

                a.OnNext(2);
                a.OnNext(4);
                a.OnNext(8);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(16, 256, 4096);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select((x, i) => x * i).Select(x => x * 10).Subscribe(x => list.Add(x));

                a.OnNext(100);
                a.OnNext(200);
                a.OnNext(300);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(0, 2000, 6000);
            }
        }

        [TestMethod]
        public void Where()
        {
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where(x => x % 3 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3);
                a.OnNext(5);
                a.OnNext(7);
                a.OnNext(9);
                a.OnNext(300);
                a.OnCompleted();

                list.IsCollection(3, 9, 300);
            }

            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where((x, i) => (x + i) % 3 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3); // 3 + 0
                a.OnNext(5); // 5 + 1
                a.OnNext(7); // 7 + 2
                a.OnNext(9); // 9 + 3
                a.OnNext(300); // 300 + 4
                a.OnCompleted();

                list.IsCollection(3, 5, 7, 9);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where(x => x % 3 == 0).Where(x => x % 5 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3);
                a.OnNext(5);
                a.OnNext(7);
                a.OnNext(9);
                a.OnNext(300);
                a.OnCompleted();

                list.IsCollection(300);
            }

            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where((x, i) => (x + i) % 3 == 0).Where(x => x % 5 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3); // 3 + 0
                a.OnNext(5); // 5 + 1
                a.OnNext(7); // 7 + 2
                a.OnNext(9); // 9 + 3
                a.OnNext(300); // 300 + 4
                a.OnCompleted();

                list.IsCollection(5);
            }
        }

        [TestMethod]
        public void Materialize()
        {
            var m = Observable.Empty<int>().Materialize().ToArrayWait();
            m[0].Kind.Is(NotificationKind.OnCompleted);

            m = Observable.Range(1, 3).Materialize().ToArrayWait();
            m[0].Value.Is(1);
            m[1].Value.Is(2);
            m[2].Value.Is(3);
            m[3].Kind.Is(NotificationKind.OnCompleted);

            m = Observable.Range(1, 3).Concat(Observable.Throw<int>(new Exception())).Materialize().ToArrayWait();
            m[0].Value.Is(1);
            m[1].Value.Is(2);
            m[2].Value.Is(3);
            m[3].Kind.Is(NotificationKind.OnError);
        }

        [TestMethod]
        public void Dematerialize()
        {
            var m = Observable.Empty<int>().Materialize().Dematerialize().ToArrayWait();
            m.IsCollection();

            m = Observable.Range(1, 3).Materialize().Dematerialize().ToArrayWait();
            m.IsCollection(1, 2, 3);

            var l = new List<int>();
            Observable.Range(1, 3).Concat(Observable.Throw<int>(new Exception())).Materialize()
                .Dematerialize()
                .Subscribe(x => l.Add(x), ex => l.Add(1000), () => l.Add(10000));
            l.IsCollection(1, 2, 3, 1000);
        }

        [TestMethod]
        public void DefaultIfEmpty()
        {
            Observable.Range(1, 3).DefaultIfEmpty(-1).ToArrayWait().IsCollection(1, 2, 3);
            Observable.Empty<int>().DefaultIfEmpty(-1).ToArrayWait().IsCollection(-1);
        }

        [TestMethod]
        public void IgnoreElements()
        {
            var xs = Observable.Range(1, 10).IgnoreElements().Materialize().ToArrayWait();
            xs[0].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ForEachAsync()
        {
            {
                var list = new List<int>();
                var xs = Observable.Range(1, 10).ForEachAsync(x => list.Add(x)).ToArray().Wait();
                list.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
                xs.Length.Is(1);
                xs[0].Is(Unit.Default);
            }

            {
                var list = new List<int>();
                var listI = new List<int>();
                var xs = Observable.Range(1, 10).ForEachAsync((x, i) =>
                {
                    list.Add(x);
                    listI.Add(i);
                }).ToArray().Wait();
                list.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
                listI.IsCollection(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                xs.Length.Is(1);
                xs[0].Is(Unit.Default);
            }
        }
    }
}
