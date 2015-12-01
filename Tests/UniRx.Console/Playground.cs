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


            var a = Observable.Range(1, 5, Scheduler.ThreadPool);
            var b = Observable.Range(10, 3, Scheduler.ThreadPool);
            var c = Observable.Return(300, Scheduler.ThreadPool);


            Observable.Concat(a).Concat(b).Concat(c).Subscribe
                (
                x => {
                    var s = new StackTrace().ToString();
                    Console.WriteLine(s);

                    Console.WriteLine("---------");
                });


            Console.ReadLine();
        }
    }
}
