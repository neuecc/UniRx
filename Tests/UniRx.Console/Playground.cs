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
            var a = Observable.Interval(TimeSpan.FromMilliseconds(300))
                   .DoOnCancel(() => Console.WriteLine("end1"))
                   .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(2))
                   .SelectMany(_ => Observable.Throw<long>(new Exception()))
                   .DoOnCancel(() => Console.WriteLine("end2")))
                   .Subscribe(x => Console.WriteLine(x), ex => Console.WriteLine(ex), () => Console.WriteLine("end3"));

            Console.ReadLine();
        }

        static IObservable<int> Hoge(Subject<int> subject)
        {
            return Observable.CreateDurable<int>(observer =>
            {
                observer.OnNext(1000);
                return subject.Subscribe(observer);
            });
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

    class MyEvent
    {
        public Action<int> action;

        public void Fire(int x)
        {
            action.Invoke(x);
        }
    }
}
