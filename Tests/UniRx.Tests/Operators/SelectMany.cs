using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class SelectMany
    {
        [TestMethod]
        public void Selector()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext(200)", "OnNext(300)", "OnNext(400)", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext(200)", "OnNext(300)", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext(200)", "OnNext(300)", "OnError(System.Exception)");
            }
        }

        [TestMethod]
        public void SelectorWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((200, 0))", "OnNext((300, 1))", "OnNext((400, 1))", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((200, 0))", "OnNext((300, 1))", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((200, 0))", "OnNext((300, 1))", "OnError(System.Exception)");
            }
        }

        [TestMethod]
        public void SelectorEnumerable()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.Is("OnNext(1)", "OnNext(2)", "OnNext(3)", "OnNext(10)", "OnNext(10)", "OnNext(10)", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.Is("OnNext(1)", "OnNext(2)", "OnNext(3)", "OnNext(10)", "OnNext(10)", "OnNext(10)", "OnError(System.Exception)");
            }
        }

        [TestMethod]
        public void SelectorEnumerableWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.Is("OnNext(0)", "OnNext(1)", "OnNext(2)", "OnNext(1)", "OnNext(1)", "OnNext(1)", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.Is("OnNext(0)", "OnNext(1)", "OnNext(2)", "OnNext(1)", "OnNext(1)", "OnNext(1)", "OnError(System.Exception)");
            }
        }



        [TestMethod]
        public void ResultSelector()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c, (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((10, 200))", "OnNext((100, 300))", "OnNext((100, 400))", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c, (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((10, 200))", "OnNext((100, 300))", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c, (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((10, 200))", "OnNext((100, 300))", "OnError(System.Exception)");
            }
        }

        [TestMethod]
        public void ResultSelectorWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)), (x, i1, y, i2) => Tuple.Create(x, i1, y.Item1, y.Item2, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((10, 0, 200, 0, 0))", "OnNext((100, 1, 300, 1, 0))", "OnNext((100, 1, 400, 1, 1))", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)), (x, i1, y, i2) => Tuple.Create(x, i1, y.Item1, y.Item2, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((10, 0, 200, 0, 0))", "OnNext((100, 1, 300, 1, 0))", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)), (x, i1, y, i2) => Tuple.Create(x, i1, y.Item1, y.Item2, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.Is("OnNext((10, 0, 200, 0, 0))", "OnNext((100, 1, 300, 1, 0))", "OnError(System.Exception)");
            }
        }

        [TestMethod]
        public void ResultSelectorEnumerable()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3), (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.Is("OnNext((10, 1))", "OnNext((10, 2))", "OnNext((10, 3))", "OnNext((100, 10))", "OnNext((100, 10))", "OnNext((100, 10))", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3), (x, y) => Tuple.Create(x, y))
                 .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.Is("OnNext((10, 1))", "OnNext((10, 2))", "OnNext((10, 3))", "OnNext((100, 10))", "OnNext((100, 10))", "OnNext((100, 10))", "OnError(System.Exception)");
            }
        }

        [TestMethod]
        public void ResultSelectorEnumerableWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3), (x, i1, y, i2) => Tuple.Create(x, i1, y, i2))
                 .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.Is("OnNext((10, 0, 0, 0))", "OnNext((10, 0, 1, 1))", "OnNext((10, 0, 2, 2))", "OnNext((100, 1, 1, 0))", "OnNext((100, 1, 1, 1))", "OnNext((100, 1, 1, 2))", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3), (x, i1, y, i2) => Tuple.Create(x, i1, y, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.Is("OnNext((10, 0, 0, 0))", "OnNext((10, 0, 1, 1))", "OnNext((10, 0, 2, 2))", "OnNext((100, 1, 1, 0))", "OnNext((100, 1, 1, 1))", "OnNext((100, 1, 1, 2))", "OnError(System.Exception)");
            }
        }
    }
}
