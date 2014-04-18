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

        // needs timebase Take. Take(TimeSpan)

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count <= 0");

            return Observable.Create<IList<T>>(observer =>
            {
                var list = new List<T>();

                return source.Subscribe(x =>
                {
                    list.Add(x);
                    if (list.Count == count)
                    {
                        observer.OnNext(list);
                        list = new List<T>();
                    }
                }, observer.OnError, () =>
                {
                    observer.OnNext(list);
                    observer.OnCompleted();
                });
            });
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan)
        {
            return Buffer(source, timeSpan, Scheduler.ThreadPool);
        }

        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Observable.Create<IList<T>>(observer =>
            {
                var list = new List<T>();
                var gate = new object();

                var d = new CompositeDisposable(2);

                // timer
                d.Add(scheduler.Schedule(timeSpan, self =>
                {
                    List<T> currentList;
                    lock (gate)
                    {
                        currentList = list;
                        if (currentList.Count != 0)
                        {
                            list = new List<T>();
                        }
                    }
                    if (currentList.Count != 0)
                    {
                        observer.OnNext(currentList);
                    }
                    self(timeSpan);
                }));

                // subscription
                d.Add(source.Subscribe(x =>
                {
                    lock (gate)
                    {
                        list.Add(x);
                    }
                }, observer.OnError, () =>
                {
                    var currentList = list;
                    observer.OnNext(currentList);
                    observer.OnCompleted();
                }));

                return d;
            });
        }

        // TimeSpan + count


        // first, last, single

        public static IObservable<T> Last<T>(this IObservable<T> source)
        {
            return LastCore<T>(source, false);
        }
        public static IObservable<T> Last<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return LastCore<T>(source.Where(predicate), false);
        }

        public static IObservable<T> LastOrDefault<T>(this IObservable<T> source)
        {
            return LastCore<T>(source, true);
        }

        public static IObservable<T> LastOrDefault<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return LastCore<T>(source.Where(predicate), true);
        }

        static IObservable<T> LastCore<T>(this IObservable<T> source, bool useDefault)
        {
            return Observable.Create<T>(observer =>
            {
                var value = default(T);
                var hasValue = true;
                return source.Subscribe(x => { value = x; hasValue = true; }, observer.OnError, () =>
                {
                    if (hasValue)
                    {
                        observer.OnNext(value);
                        observer.OnCompleted();
                    }
                    else
                    {
                        if (useDefault)
                        {
                            observer.OnNext(default(T));
                            observer.OnCompleted();
                        }
                        else
                        {
                            observer.OnError(new InvalidOperationException("sequence is empty"));
                        }
                    }
                });
            });
        }

        public static IObservable<T> First<T>(this IObservable<T> source)
        {
            return FirstCore<T>(source, false);
        }
        public static IObservable<T> First<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return FirstCore<T>(source.Where(predicate), false);
        }

        public static IObservable<T> FirstOrDefault<T>(this IObservable<T> source)
        {
            return FirstCore<T>(source, true);
        }

        public static IObservable<T> FirstOrDefault<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return FirstCore<T>(source.Where(predicate), true);
        }

        static IObservable<T> FirstCore<T>(this IObservable<T> source, bool useDefault)
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(x =>
                {
                    observer.OnNext(x);
                    observer.OnCompleted();
                }, observer.OnError,
                () =>
                {
                    if (useDefault)
                    {
                        observer.OnNext(default(T));
                        observer.OnCompleted();
                    }
                    else
                    {
                        observer.OnError(new InvalidOperationException("sequence is empty"));
                    }
                });
            });
        }

        public static IObservable<T> Single<T>(this IObservable<T> source)
        {
            return SingleCore<T>(source, false);
        }
        public static IObservable<T> Single<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return SingleCore<T>(source.Where(predicate), false);
        }

        public static IObservable<T> SingleOrDefault<T>(this IObservable<T> source)
        {
            return SingleCore<T>(source, true);
        }

        public static IObservable<T> SingleOrDefault<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return SingleCore<T>(source.Where(predicate), true);
        }

        static IObservable<T> SingleCore<T>(this IObservable<T> source, bool useDefault)
        {
            return Observable.Create<T>(observer =>
            {
                var value = default(T);
                var seenValue = false;
                return source.Subscribe(x =>
                {
                    if (seenValue)
                    {
                        observer.OnError(new InvalidOperationException("sequence is not single"));
                    }
                    value = x;
                    seenValue = true;
                }, observer.OnError, () =>
                {
                    if (seenValue)
                    {
                        observer.OnNext(value);
                        observer.OnCompleted();
                    }
                    else
                    {
                        if (useDefault)
                        {
                            observer.OnNext(default(T));
                            observer.OnCompleted();
                        }
                        else
                        {
                            observer.OnError(new InvalidOperationException("sequence is empty"));
                        }
                    }
                });
            });
        }
    }
}