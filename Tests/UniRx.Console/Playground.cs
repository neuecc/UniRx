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

            //var s1 = new Subject<int>();
            //var s2 = new Subject<int>();

            //s1.Amb(s2).Subscribe(x => Console.WriteLine(x));


            //s1.OnNext(1000);
            //s1.OnNext(1001);
            //s1.OnNext(1002);
            //s1.OnNext(1003);
            //s1.OnNext(1004);
            //s2.OnNext(2001);
            //s2.OnNext(2002);
            //s2.OnNext(2003);
            //s2.OnNext(2004);

            var xs = Observable.Return(10).Delay(TimeSpan.FromSeconds(1)).Concat(Observable.Range(1, 3));

            var xss = Observable.Return(10).Concat(Observable.Range(1, 3));
            var a = xss.ToArray().Wait();
            var b = xss.ToArray().Wait();
            var c = xss.ToArray().Wait();


            var ys = Observable.Return(30).Delay(TimeSpan.FromSeconds(2)).Concat(Observable.Range(5, 3));

            // win left
            var result = xs.Amb(ys).ToArray().Wait();

            result[0].Is(10);
            result[1].Is(1);
            result[2].Is(2);
            result[3].Is(3);

            // win right
            result = ys.Amb(xs).ToArray().Wait();

            result[0].Is(10);
            result[1].Is(1);
            result[2].Is(2);
            result[3].Is(3);
        }

        static void ShowStackTrace()
        {
            Console.WriteLine("----------------");
            Console.WriteLine(new StackTrace().ToString());
        }
    }

    public static class OEx
    {
        public static void Is<T>(this T t, T t2)
        {
            Console.WriteLine("T:" + t + " T2:" + t2);
        }
    }
}
