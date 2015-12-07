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
            var myE = new MyEvent();

            var ev = Observable.FromEvent<int>(h => myE.action += h, h =>
            {
                Console.WriteLine("DISPOSED");
                myE.action -= h;
            });

            ev.Subscribe(xx =>
            {
                ShowStackTrace();
                Console.WriteLine("COME:" + xx);
                // Subscribe in Subscribe
                Observable.Return(xx)
                    .Do(x => { if (x == 1) throw new Exception(); })
                    .Subscribe(x => ShowStackTrace());
            });

            try { myE.Fire(1); }
            catch (Exception ex)
            {
                //Console.WriteLine("Here:" + ex);
            }

            try { myE.Fire(10); }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
            }
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
