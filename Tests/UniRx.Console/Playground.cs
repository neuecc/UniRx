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
            var rp = new Subject<int>();

            var rrp = rp.ToReadOnlyReactiveProperty();

            rrp.Subscribe(x => ShowStackTrace());

            rp.OnNext(5000);


            rp.OnNext(9900);

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
