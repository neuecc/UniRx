using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UniRx.Tests
{
    // TODO:Needs test scheduler, Test Utilities...

    [TestClass]
    public class ObservableTest
    {
        [TestMethod]
        public void ToArray()
        {
            var subject = new Subject<int>();

            int[] array = null;
            var disp = subject.ToArray().Subscribe(xs => array = xs);

            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);
            subject.OnCompleted();

            array.Is(1, 10, 100);
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
            var subject = new Subject<int>();

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                subject.OnNext(100);
                subject.OnCompleted();
            });

            subject.Wait().Is(100);
        }


        [TestMethod]
        public void DistinctUntilChanged()
        {
            {
                var subject = new Subject<int>();

                int[] array = null;
                var disp = subject.DistinctUntilChanged().ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5 }, subject.OnNext);
                subject.OnCompleted();

                array.Is(1, 10, 1, 100, 5);
            }
            {

                string[] array = null;
                new[] { "hoge", "huga", null, null, "huga", "huga", "hoge" }
                    .ToObservable()
                    .DistinctUntilChanged()
                    .ToArray()
                    .Subscribe(xs => array = xs);

                array.Is("hoge", "huga", null, "huga", "hoge");
            }
            {
                var subject = new Subject<int>();

                int[] array = null;
                var disp = subject.DistinctUntilChanged(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5 }, subject.OnNext);
                subject.OnCompleted();

                array.Is(1, 10, 1, 100, 5);
            }
        }

        [TestMethod]
        public void Distinct()
        {
            {
                var subject = new Subject<int>();

                int[] array = null;
                var disp = subject.Distinct().ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }, subject.OnNext);
                subject.OnCompleted();

                array.Is(1, 10, 100, 5, 70, 7);
            }
            {
                var subject = new Subject<int>();

                int[] array = null;
                var disp = subject.Distinct(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }, subject.OnNext);
                subject.OnCompleted();

                array.Is(1, 10, 100, 5, 70, 7);
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
                list.Is(10000, 40000, 90000);
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
                list.Is(0, 200, 600);
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
                list.Is(16, 256, 4096);
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
                list.Is(0, 2000, 6000);
            }
        }
    }
}
