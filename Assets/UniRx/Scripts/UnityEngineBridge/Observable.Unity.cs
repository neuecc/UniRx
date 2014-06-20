using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx
{
    public static partial class Observable
    {
        /// <summary>From has no callback coroutine to IObservable. If publishEveryYield = true then publish OnNext every yield return else return once on enumeration completed.</summary>
        public static IObservable<Unit> FromCoroutine(Func<IEnumerator> coroutine, bool publishEveryYield = false)
        {
            return FromCoroutine<Unit>((observer, cancellationToken) => WrapEnumerator(coroutine(), observer, cancellationToken, publishEveryYield));
        }

        static IEnumerator WrapEnumerator(IEnumerator enumerator, IObserver<Unit> observer, CancellationToken cancellationToken, bool publishEveryYield)
        {
            var hasNext = default(bool);
            var raisedError = false;
            do
            {
                try
                {
                    hasNext = enumerator.MoveNext();
                }
                catch (Exception ex)
                {
                    try
                    {
                        raisedError = true;
                        observer.OnError(ex);
                    }
                    finally
                    {
                        var d = enumerator as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                        }
                    }
                    yield break;
                }
                if (hasNext && publishEveryYield)
                {
                    try
                    {
                        observer.OnNext(Unit.Default);
                    }
                    catch
                    {
                        var d = enumerator as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                        }
                        throw;
                    }
                }
                if (hasNext)
                {
                    yield return enumerator.Current; // yield inner YieldInstruction
                }
            } while (hasNext && !cancellationToken.IsCancellationRequested);

            try
            {
                if (!raisedError && !cancellationToken.IsCancellationRequested)
                {
                    observer.OnNext(Unit.Default); // last one
                    observer.OnCompleted();
                }
            }
            finally
            {
                var d = enumerator as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
            }
        }

        public static IObservable<T> FromCoroutine<T>(Func<IObserver<T>, IEnumerator> coroutine)
        {
            return FromCoroutine<T>((observer, _) => coroutine(observer));
        }

        public static IObservable<T> FromCoroutine<T>(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
        {
            return Observable.Create<T>(observer =>
            {
                var cancel = new BooleanDisposable();

                MainThreadDispatcher.StartCoroutine(coroutine(observer, new CancellationToken(cancel)));

                return cancel;
            });
        }

        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, IEnumerator coroutine, bool publishEveryYield = false)
        {
            return source.SelectMany(Observable.FromCoroutine(() => coroutine, publishEveryYield));
        }

        public static IObservable<Unit> SelectMany<T>(this IObservable<T> source, Func<T, IEnumerator> selector, bool publishEveryYield = false)
        {
            return source.SelectMany(x => Observable.FromCoroutine(() => selector(x), publishEveryYield));
        }

        public static IObservable<Unit> ToObservable(this IEnumerator coroutine, bool publishEveryYield = false)
        {
            return FromCoroutine<Unit>((observer, cancellationToken) => WrapEnumerator(coroutine, observer, cancellationToken, publishEveryYield));
        }

        // variation of FromCoroutine

        public static IObservable<long> EveryUpdate()
        {
            return FromCoroutine<long>((observer, cancellationToken) => EveryUpdateCore(observer, cancellationToken));
        }

        static IEnumerator EveryUpdateCore(IObserver<long> observer, CancellationToken cancellationToken)
        {
            var count = 0L;
            while (!cancellationToken.IsCancellationRequested)
            {
                observer.OnNext(count++);
                yield return null;
            }
        }

        public static IObservable<long> EveryFixedUpdate()
        {
            return FromCoroutine<long>((observer, cancellationToken) => EveryFixedUpdateCore(observer, cancellationToken));
        }

        static IEnumerator EveryFixedUpdateCore(IObserver<long> observer, CancellationToken cancellationToken)
        {
            var count = 0L;
            while (!cancellationToken.IsCancellationRequested)
            {
                yield return new UnityEngine.WaitForFixedUpdate();
                observer.OnNext(count++);
            }
        }

        public static IObservable<T> DelayFrame<T>(this IObservable<T> source, int frameCount)
        {
            if (frameCount < 0) throw new ArgumentOutOfRangeException("frameCount");

            return Observable.Create<T>(observer =>
            {
                var cancel = new BooleanDisposable();

                source.Materialize().Subscribe(x =>
                {
                    if (x.Kind == NotificationKind.OnError)
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

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source)
        {
            return ToCoroutine<T>(source, _ => { }, _ => { }, () => { });
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source, IObserver<T> observer)
        {
            return ToCoroutine<T>(source, observer.OnNext, observer.OnError, observer.OnCompleted);
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source, Action<T> onNext)
        {
            return ToCoroutine<T>(source, onNext, _ => { }, () => { });
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return ToCoroutine<T>(source, onNext, onError, () => { });
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return ToCoroutine<T>(source, onNext, _ => { }, onCompleted);
        }

        /// <summary>Convert to awaitable IEnumerator. It's run on MainThread.</summary>
        public static IEnumerator ToCoroutine<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            var running = true;

            source
                .Do(onNext, onError, onCompleted)
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(
                    ex => { running = false; },
                    () => { running = false; });

            while (running)
            {
                yield return null;
            }
        }

        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source)
        {
            return source.ObserveOn(Scheduler.MainThread);
        }

        public static IObservable<T> SubscribeOnMainThread<T>(this IObservable<T> source)
        {
            return source.SubscribeOn(Scheduler.MainThread);
        }
    }
}