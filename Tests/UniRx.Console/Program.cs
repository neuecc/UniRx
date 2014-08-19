using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using UniRx.InternalUtil;

namespace UniRx
{
    // Check AOT Compile for mono 2.6.7 compile with [mono --full-aot]

    public struct MyStruct
    {
        public int MyProperty { get; set; }
        public string MyProperty2 { get; set; }
    }

    public class MyClass
    {
        public int MyProperty { get; set; }
        public string MyProperty2 { get; set; }
    }

    public class MyClass2
    {
        public int MyProperty { get; set; }
        public Depth2 Depth2;
    }

    public struct Depth2
    {
        public int MyProperty { get; set; }
        public Depth3 Depth3 { get; set; }
    }

    public class Depth3
    {
        public int MyProperty { get; set; }
    }

    class Program
    {
        static void Test(string label, Action action)
        {
            Console.WriteLine(label + " ---------------");

            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine();
        }

        static void Hoge<T>(Action<T> a)
        {
            Console.WriteLine(a.GetHashCode());
        }

        static void Main(string[] args)
        // static void Main2(string[] args)
        {
            Test("ReturnEmpty", () => Observable.Return(1).Subscribe());

            Test("Throw", () => Observable.Return(1).Do(_ => { throw new Exception(); }).Do(_ => { })
                .CatchIgnore().Subscribe());

            Test("Range", () => Observable.Range(1, 3).Subscribe(x => Console.WriteLine(x)));

            Test("IgnoreElements", () => Observable.Range(1, 3).IgnoreElements().Subscribe(x => Console.WriteLine(x)));

            Test("DefaultIfEmpty", () => Observable.Empty<int>().DefaultIfEmpty(100).Subscribe(x => Console.WriteLine(x)));

            Test("ToArray", () => Observable.Range(1, 3).ToArray().Subscribe(xs => Console.WriteLine(xs.Length)));

            Test("Complex", () => Observable.Return(1)
                .SelectMany(_ => new[] { new MyStruct(), new MyStruct() })
                .Where(x => true)
                .Subscribe(x =>
                {
                    Console.WriteLine("done");
                }));

            Test("Where", () => Observable.Range(1, 5).Where(x => x % 2 == 0).Subscribe());

            Test("SkipWhile", () => Observable.Range(1, 5).SkipWhile(x => x <= 3).Subscribe(Console.WriteLine));

            Test("FromEventPattern", () => EventAotCheck());

            Test("FromEvent", () => EventAotCheck2());

            Test("Subject", () =>
            {
                var s = new Subject<int>();
                s.Subscribe(x => Console.WriteLine(x), ex => { });
                s.Subscribe(x => Console.WriteLine(x), ex => { });
                var d = s.Subscribe(x => Console.WriteLine(x), ex => { });
                s.OnNext(10);
                s.OnNext(100);
                d.Dispose();
                s.OnNext(1000);
                s.OnError(new Exception());
                s.OnCompleted();
                s.Dispose();
            });

            Test("Distinct", () =>
            {
                var s = new Subject<int>();
                s.Distinct().Subscribe(x => Console.WriteLine(x));
                s.OnNext(10);
                s.OnNext(10);
                s.OnNext(100);
                s.OnNext(10);
                s.OnNext(100);
                s.OnNext(100);
                s.OnNext(200);
            });

            Test("DistinctUntilChanged", () =>
            {
                var s = new Subject<int>();
                s.DistinctUntilChanged().Subscribe(x => Console.WriteLine(x));
                s.OnNext(10);
                s.OnNext(10);
                s.OnNext(100);
                s.OnNext(10);
                s.OnNext(100);
                s.OnNext(100);
                s.OnNext(200);
            });

            //Test("ReflectionAccessor", () =>
            //{
            //    var mc = new MyClass2
            //    {
            //        MyProperty = 100,
            //        Depth2 = new Depth2
            //        {
            //            MyProperty = 1000,
            //            Depth3 = new Depth3
            //            {
            //                MyProperty = 10000
            //            }
            //        }
            //    };

            //    {
            //        Expression<Func<MyClass2, int>> selector = x => x.MyProperty;
            //        var accessor = ReflectionAccessor.Create(selector.Body as MemberExpression);
            //        Console.WriteLine(accessor.GetValue(mc));
            //    }

            //    {
            //        Expression<Func<MyClass2, int>> selector = x => x.Depth2.MyProperty;
            //        var accessor = ReflectionAccessor.Create(selector.Body as MemberExpression);
            //        Console.WriteLine(accessor.GetValue(mc));
            //    }

            //    {
            //        Expression<Func<MyClass2, int>> selector = x => x.Depth2.Depth3.MyProperty;
            //        var accessor = ReflectionAccessor.Create(selector.Body as MemberExpression);
            //        Console.WriteLine(accessor.GetValue(mc));
            //    }
            //});
        }

        static void EventAotCheck()
        {
            var test = new EventTestesr();

            {
                var isRaised = false;
                var d = Observable.FromEventPattern<EventHandler, EventArgs>(
                    h => h.Invoke,
                    h => test.Event1 += h, h => test.Event1 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(1);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(1);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEventPattern<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => h.Invoke,
                    h => test.Event2 += h, h => test.Event2 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(2);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(2);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEventPattern<MyEventHandler, MyEventArgs>(
                    h => h.Invoke,
                    h => test.Event3 += h, h => test.Event3 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(3);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(3);
                Console.WriteLine(isRaised == false);
            }
        }


        static void EventAotCheck2()
        {
            var test = new EventTestesr();

            {
                var isRaised = false;
                var d = Observable.FromEvent<EventHandler, EventArgs>(
                    h => (sender, e) => h.Invoke(e),
                    h => test.Event1 += h, h => test.Event1 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(1);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(1);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => (sender, e) => h.Invoke(e),
                    h => test.Event2 += h, h => test.Event2 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(2);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(2);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<MyEventHandler, MyEventArgs>(
                    h => (sender, e) => h.Invoke(e),
                    h => test.Event3 += h, h => test.Event3 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(3);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(3);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<Action, Unit>(
                    h => () => h(Unit.Default),
                    h => test.Event4 += h, h => test.Event4 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(4);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(4);
                Console.WriteLine(isRaised == false);
            }

            // shortcut
            {
                var isRaised = false;
                var d = Observable.FromEvent(
                    h => test.Event4 += h, h => test.Event4 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(4);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(4);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<Action<int>, int>(
                    h => h,
                    h => test.Event5 += h, h => test.Event5 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(5);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(5);
                Console.WriteLine(isRaised == false);
            }

            // shortcut
            {
                var isRaised = false;
                var d = Observable.FromEvent<int>(
                    h => test.Event5 += h, h => test.Event5 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(5);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(5);
                Console.WriteLine(isRaised == false);
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<Action<int, string>, Tuple<int, string>>(
                    h => (x, y) => h(Tuple.Create(x, y)),
                    h => test.Event6 += h, h => test.Event6 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(6);
                Console.WriteLine(isRaised);
                isRaised = false;
                d.Dispose();
                test.Fire(6);
                Console.WriteLine(isRaised == false);
            }
        }


        class EventTestesr
        {
            public event EventHandler Event1;
            public event EventHandler<MyEventArgs> Event2;
            public event MyEventHandler Event3;
            public event Action Event4;
            public event Action<int> Event5;
            public event Action<int, string> Event6;

            public void Fire(int num)
            {
                switch (num)
                {
                    case 1:
                        if (Event1 == null) return;
                        Event1(this, new EventArgs());
                        break;
                    case 2:
                        if (Event2 == null) return;
                        Event2(this, new MyEventArgs());
                        break;
                    case 3:
                        if (Event3 == null) return;
                        Event3(this, new MyEventArgs());
                        break;
                    case 4:
                        if (Event4 == null) return;
                        Event4();
                        break;
                    case 5:
                        if (Event5 == null) return;
                        Event5(100);
                        break;
                    case 6:
                        if (Event6 == null) return;
                        Event6(100, "hogehoge");
                        break;
                    default:
                        break;
                }
            }
        }

        delegate void MyEventHandler(object sender, MyEventArgs eventArgs);

        class MyEventArgs : EventArgs
        {

        }

        // static void Main(string[] args)
        static void Main2(string[] args) // Check AOT Exception Pattern
        {
            int x = 100;
            Interlocked.CompareExchange(ref x, 10, 20); // safe

            object x2 = new object();
            Interlocked.CompareExchange(ref x2, new object(), new object()); // safe

            MyClass mc = new MyClass();
            var hoge = Interlocked.CompareExchange<MyClass>(ref mc, new MyClass(), new MyClass()); // death
            Console.WriteLine(hoge.MyProperty);
        }
    }
}