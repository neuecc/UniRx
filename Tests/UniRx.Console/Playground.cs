using System;
using System.Collections.Generic;
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

            var list = new List<int>();
            a.SelectMany(_ => b).Subscribe(x => list.Add(x));

            a.OnNext(10);
            a.OnCompleted();
            b.OnNext(100);

            //list.Count.Is(1);
            //list[0].Is(100);


            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }
    }
}
