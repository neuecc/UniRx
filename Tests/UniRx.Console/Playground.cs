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
            var subject = new Subject<int>();

            subject.Select(x => x).Subscribe(x => { Console.WriteLine(x); throw new Exception(); }, ex =>
            {
                ShowStackTrace();
                Console.WriteLine("called ex 1");
                //throw ex;
            });

            try
            {
                subject.OnNext(2);
            }
            catch
            {
            }

            try
            {
                subject.OnNext(1);
            }
            catch
            {
            }



        }

        static void ShowStackTrace()
        {
            Console.WriteLine("----------------");
            Console.WriteLine(new StackTrace().ToString());
        }
    }
}
