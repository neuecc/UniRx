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

            Observable.Range(1, 3, Scheduler.CurrentThread)
                .Select(x => x)
                .StartWith(200)
                .Subscribe(x =>
                {
                    Console.WriteLine("-----------------------");
                    Console.WriteLine(new StackTrace().ToString());
                    Console.WriteLine("-----------------------");
                });


        }
    }
}
