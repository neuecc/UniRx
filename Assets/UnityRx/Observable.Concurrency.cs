using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public static partial class Observable
    {

        public static IObservable<T> Synchronize<T>(this IObservable<T> source)
        {
            return source.Synchronize(new object());
        }

        public static IObservable<T> Synchronize<T>(this IObservable<T> source, object gate)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(
                    x => { lock (gate) observer.OnNext(x); },
                    x => { lock (gate) observer.OnError(x); },
                    () => { lock (gate) observer.OnCompleted(); });
            });
        }

        public static IObservable<T> ObserveOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var group = new CompositeDisposable();

                var first = source.Subscribe(x =>
                {
                    var d = scheduler.Schedule(() => observer.OnNext(x));
                    group.Add(d);
                }, observer.OnError, observer.OnCompleted);

                group.Add(first);

                return group;
            });
        }
    }
}
