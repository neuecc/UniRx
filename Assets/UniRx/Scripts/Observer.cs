using System;
using System.Threading;

#if SystemReactive
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace System
#else
namespace UniRx
#endif
{
    public static class Observer
    {
        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            // need compare for avoid iOS AOT
            if (onNext == Stubs.Ignore<T>)
            {
                return new EmptyOnNextAnonymousObserver<T>(onError, onCompleted);
            }
            else
            {
                return new AnonymousObserver<T>(onNext, onError, onCompleted);
            }
        }

        /// <summary>
        /// Create new onNext + rootObserver.OnError, rootObserver.OnCompleted observer.
        /// </summary>
        public static IObserver<T> Create<T, TRoot>(Action<T> onNext, IObserver<TRoot> rootObserver)
        {
            return new DelegatedOnNextObserver<T, TRoot>(onNext, rootObserver, Disposable.Empty);
        }

        public static IObserver<T> CreateAutoDetachObserver<T>(IObserver<T> observer, IDisposable disposable)
        {
            return new AutoDetachObserver<T>(observer, disposable);
        }

        class AnonymousObserver<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    onNext(value);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted();
                }
            }
        }

        class EmptyOnNextAnonymousObserver<T> : IObserver<T>
        {
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public EmptyOnNextAnonymousObserver(Action<Exception> onError, Action onCompleted)
            {
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onError(error);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    onCompleted();
                }
            }
        }

        class DelegatedOnNextObserver<T, TRoot> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly IObserver<TRoot> observer;
            readonly IDisposable disposable;

            int isStopped = 0;

            public DelegatedOnNextObserver(Action<T> onNext, IObserver<TRoot> observer, IDisposable disposable)
            {
                this.onNext = onNext;
                this.observer = observer;
                this.disposable = disposable;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    try
                    {
                        onNext(value);
                    }
                    catch
                    {
                        disposable.Dispose();
                        throw;
                    }
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        observer.OnError(error);
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        observer.OnCompleted();
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        class AutoDetachObserver<T> : IObserver<T>
        {
            readonly IObserver<T> observer;
            readonly IDisposable disposable;

            int isStopped = 0;

            public AutoDetachObserver(IObserver<T> observer, IDisposable disposable)
            {
                this.observer = observer;
                this.disposable = disposable;
            }

            public void OnNext(T value)
            {
                if (isStopped == 0)
                {
                    try
                    {
                        this.observer.OnNext(value);
                    }
                    catch
                    {
                        disposable.Dispose();
                        throw;
                    }
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        this.observer.OnError(error);
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        this.observer.OnCompleted();
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }

    public static partial class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            return source.Subscribe(Observer.Create<T>(Stubs.Ignore<T>, Stubs.Throw, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(Observer.Create(onNext, Stubs.Throw, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(Observer.Create(onNext, onError, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(Observer.Create(onNext, Stubs.Throw, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(Observer.Create(onNext, onError, onCompleted));
        }
    }

    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => { throw ex; };

        // Stubs<T>.Ignore can't avoid iOS AOT problem.
        public static void Ignore<T>(T t)
        {
        }

        // marker for CatchIgnore and Catch avoid iOS AOT problem.
        public static IObservable<TSource> CatchIgnore<TSource>(Exception ex)
        {
            return Observable.Empty<TSource>();
        }
    }
}