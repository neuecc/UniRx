using System;
using System.Collections.Generic;
using System.Text;
using UniRx.Operators;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<TSource> Scan<TSource>(this IObservable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            return new Scan<TSource>(source, func);
        }

        public static IObservable<TAccumulate> Scan<TSource, TAccumulate>(this IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return new Scan<TSource, TAccumulate>(source, seed, func);
        }
    }
}
