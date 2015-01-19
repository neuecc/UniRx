using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
    public static class AotSafeExtensions
    {
        public static IEnumerable<T> AsSafeEnumerable<T>(this IEnumerable<T> source)
        {
            var e = ((IEnumerable)source).GetEnumerator();
            using (e as IDisposable)
            {
                while (e.MoveNext())
                {
                    yield return (T)e.Current;
                }
            }
        }

        public static IObservable<Tuple<T>> WrapValueToClass<T>(this IObservable<T> source)
            where T : struct
        {
            var dummy = 0;
            return Observable.Create<Tuple<T>>(observer =>
            {
                return source.Subscribe(Observer.Create<T>(x =>
                {
                    dummy.GetHashCode(); // capture outer value
                    var v = new Tuple<T>(x);
                    observer.OnNext(v);
                }, observer.OnError, observer.OnCompleted));
            });
        }

        public static IEnumerable<Tuple<T>> WrapValueToClass<T>(this IEnumerable<T> source)
            where T : struct
        {
            var e = ((IEnumerable)source).GetEnumerator();
            using (e as IDisposable)
            {
                while (e.MoveNext())
                {
                    yield return new Tuple<T>((T)e.Current);
                }
            }
        }
    }
}