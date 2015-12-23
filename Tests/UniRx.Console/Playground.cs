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
            GCCheck();

        }

        void GCCheck()
        {
            var t = Tuple.Create(1, 1);
            var dict = new Dictionary<Tuple<int, int>, int>();
            //var d = EqualityComparer<Tuple<int, int>>.Default;
            //Console.WriteLine(d.GetType().Name);
            dict[Tuple.Create(1, 1)] = 1;
            for (int i = 0; i < 10000000; i++)
            {
                dict.ContainsKey(Tuple.Create(1, 1));
                //d.Equals(t, Tuple.Create(1, 1));
            }

            Console.WriteLine("0:" + GC.CollectionCount(0));
            Console.WriteLine("1:" + GC.CollectionCount(1));
            Console.WriteLine("2:" + GC.CollectionCount(2));
        }

        static IObservable<int> Hoge(Subject<int> subject)
        {
            return Observable.Create<int>(observer =>
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
