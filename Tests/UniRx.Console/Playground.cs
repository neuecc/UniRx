using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public class Playground
    {
        public void Run()
        {


            var xs = Observable.Return(1, Scheduler.CurrentThread)
                //.Do(x => { Console.WriteLine("DO:" + x); })
                .Repeat()
                .Take(3)
                .Subscribe(x => Console.WriteLine(x));

            Console.WriteLine("---");
                

            

        }
    }
}
