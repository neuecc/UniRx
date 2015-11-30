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

            var list = Enumerable.Range(1, 3).ToObservable().ToArray().Wait();

            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }
    }
}
