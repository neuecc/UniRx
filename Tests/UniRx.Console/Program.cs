#pragma warning disable CS0162

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace UniRx
{
    class Program
    {
        static void Main(string[] args)
        {
            // my path:)
            var path = @"C:\Users\neuecc\Documents\Git\neuecc\UniRx\Assets\Plugins\UniRx\Scripts\UnityEngineBridge\Triggers";
            var outpath = @"";
            TriggerFileGenerator.GenerateAsyncTrigger(path, outpath);

            //            Console.WriteLine("-- -");
            //          Console.WriteLine(code);
            //        Console.WriteLine("---");

        }
    }
}

#pragma warning restore CS0162