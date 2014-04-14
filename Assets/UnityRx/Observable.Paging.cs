using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    // Take, Skip, etc..
    public static partial class Observable
    {

        public static IObservable<T> Take<T>(this IObservable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            if (count == 0) return Empty<T>();

            return Observable.Create<T>(observer =>
            {
                var rest = count;

                return source.Subscribe(x =>
                {
                    if (rest > 0)
                    {
                        rest -= 1;
                        observer.OnNext(x);
                        if (rest == 0) observer.OnCompleted();
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }

        // TODO:timebase Take. Take(TimeSpan)
    }
}
