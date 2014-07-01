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
            return source.Select(x => new Tuple<T>(x));
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