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
            var s = new Subject<int>();

            Hoge(s).Subscribe(x =>
            {
                Console.WriteLine("B:" + x);
                if (x == 1000) throw new Exception();
                Console.WriteLine("F:" + x);
            });

            try { s.OnNext(100); }catch{ }
            try { s.OnNext(200); } catch { }
            try { s.OnNext(300); } catch { }


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
