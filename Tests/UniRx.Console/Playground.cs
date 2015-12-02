using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace UniRx
{
    public class Playground
    {
        public void Run()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();
            var c = new Subject<int>();

            Observable.CombineLatest(a, b, c, (x, y, z) => new { x, y, z })
                //.Materialize().Select(x => x.ToString())
                .Subscribe(Console.WriteLine);


            a.OnNext(100);
            b.OnNext(2000);
            c.OnNext(50);


            a.OnCompleted();
            b.OnCompleted();


            c.OnNext(90);



        }

        static void ShowStackTrace()
        {
            Console.WriteLine("----------------");
            Console.WriteLine(new StackTrace().ToString());
        }
    }
}
