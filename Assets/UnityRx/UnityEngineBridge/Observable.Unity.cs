using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public static partial class Observable
    {
        // TODO:Dispose,Cancel!

        static IEnumerator EveryFrameCore(Action<Unit> onNext)
        {
            while (true)
            {
                onNext(Unit.Default);
                yield return null;
            }
        }

        public static IObservable<Unit> EveryFrame()
        {
            var subject = new Subject<Unit>();
            MainThreadDispatcher.StartCoroutine(EveryFrameCore(subject.OnNext));

            return subject;
        }
    }
}
