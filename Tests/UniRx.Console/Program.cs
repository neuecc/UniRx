using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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