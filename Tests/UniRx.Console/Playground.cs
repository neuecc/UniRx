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
            subject
                .Select<int, int>(x => { throw new Exception(); })
                .Where(x => x % 2 == 0)
                .Take(10)
                .Subscribe(_ => ShowStackTrace(), ex => ShowStackTrace(), () => ShowStackTrace());

            subject.OnNext(200);

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
