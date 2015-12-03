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

            var xs = Observable.Range(1, 100)
                .ObserveOn(Scheduler.ThreadPool)
                .Subscribe(x => Console.WriteLine(x));


            //xs.Dispose();

            Console.ReadLine();


        }

        static void ShowStackTrace()
        {
            Console.WriteLine("----------------");
            Console.WriteLine(new StackTrace().ToString());
        }
    }
}
