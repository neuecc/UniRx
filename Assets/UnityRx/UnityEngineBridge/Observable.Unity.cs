using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<Unit> EveryFrame()
        {
            return Observable.Create<Unit>(observer =>
            {
                var cancel = new BooleanDisposable();

                MainThreadDispatcher.StartCoroutine(EveryFrameCore(observer.OnNext, cancel));

                return cancel;
            });
        }

        static IEnumerator EveryFrameCore(Action<Unit> onNext, ICancelable cancel)
        {
            while (!cancel.IsDisposed)
            {
                onNext(Unit.Default);
                yield return null;
            }
        }

        public static IObservable<T> DelayFrame<T>(this IObservable<T> source, int frameCount)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");

            return Observable.Create<T>(observer =>
            {
                var cancel = new BooleanDisposable();

                source.Materialize().Subscribe(x=>
                {
                    if(x.Kind == NotificationKind.OnError)
                    {
                        observer.OnError(x.Exception);
                        cancel.Dispose();
                        return;
                    }

                    MainThreadDispatcher.StartCoroutine(DelayFrameCore(() => x.Accept(observer), frameCount, cancel));
                });

                return cancel;
            });
        }

        static IEnumerator DelayFrameCore(Action onNext, int frameCount, ICancelable cancel)
        {
            while (!cancel.IsDisposed && frameCount-- != 0)
            {
                yield return null;
            }
            if (!cancel.IsDisposed)
            {
                onNext();
            }
        }

        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source)
        {
            return source.ObserveOn(Scheduler.MainThread);
        }
    }
}