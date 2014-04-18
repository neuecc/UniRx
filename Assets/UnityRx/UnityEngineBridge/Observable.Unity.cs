using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public static partial class Observable
    {
        static IEnumerator EveryFrameCore(Action<Unit> onNext, ICancelable cancel)
        {
            while (!cancel.IsDisposed)
            {
                onNext(Unit.Default);
                yield return null;
            }
        }

        public static IObservable<Unit> EveryFrame()
        {
            return Observable.Create<Unit>(observer =>
            {
                var cancel = new BooleanDisposable();

                MainThreadDispatcher.StartCoroutine(EveryFrameCore(observer.OnNext, cancel));

                return cancel;
            });
        }

        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source)
        {
            return source.ObserveOn(Scheduler.MainThread);
        }
    }
}