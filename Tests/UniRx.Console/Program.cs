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
        static void Main(string[] args)
        {
            Observable.Range(1, 10).Subscribe(x => Console.WriteLine(x));
        }
    }
}