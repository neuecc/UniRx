using System;
using System.Collections.Generic;
using System.Text;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<TSource> Scan<TSource>(this IObservable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            return Observable.Create<TSource>(observer =>
            {
                bool isFirst = true;
                TSource prev = default(TSource);
                return source.Subscribe(x =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        prev = x;
                        observer.OnNext(x);
                    }
                    else
                    {
                        try
                        {
                            prev = func(prev, x); // prev as current
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return;
                        }

                        observer.OnNext(prev);
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }

        public static IObservable<TAccumulate> Scan<TSource, TAccumulate>(this IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return Observable.Create<TAccumulate>(observer =>
            {
                var prev = seed;
                observer.OnNext(seed);

                return source.Subscribe(x =>
                {
                    try
                    {
                        prev = func(prev, x); // prev as next
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    observer.OnNext(prev);
                }, observer.OnError, observer.OnCompleted);
            });
        }
    }
}
