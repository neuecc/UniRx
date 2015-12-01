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
            var rp1 = new ReactiveProperty<int>();
            var rp2 = new ReactiveProperty<int>();
            var rp3 = new ReactiveProperty<int>();


            var xs = new[] {
                rp1, rp2, rp3
            }.Merge()
            .Subscribe(x => ShowStackTrace());


            rp1.Value = 1;


        }

        static void ShowStackTrace()
        {
            Console.WriteLine("----------------");
            Console.WriteLine(new StackTrace().ToString());
        }
    }
}
