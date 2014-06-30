using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    // Check AOT Compile for mono 2.6.7 compile with [mono --full-aot]

    public struct MyStruct
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
        {
            Test("ReturnEmpty", () => Observable.Return(1).Subscribe());

            Test("Throw", () => Observable.Return(1).Do(_ => { throw new Exception(); }).Do(_ => { })
                .CatchIgnore().Subscribe());

            Test("Range", () => Observable.Range(1, 3).Subscribe(x => Console.WriteLine(x)));

            Test("Complex", () => Observable.Return(1)
                .SelectMany(_ => new[] { new MyStruct(), new MyStruct() })
                .Where(x => true)
                .Subscribe(x =>
                {
                    Console.WriteLine("done");
                }));
        }
    }
}