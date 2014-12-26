using System;
using System.Collections;
using System.Threading;

namespace UniRx
{
    public static class Observer
    {
        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return Create<T>(onNext, onError, onCompleted, Disposable.Empty);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted, IDisposable disposable)
        {
            if (onNext == null) throw new ArgumentNullException("onNext");
            if (onError == null) throw new ArgumentNullException("onError");
            if (onCompleted == null) throw new ArgumentNullException("onCompleted");
            if (disposable == null) throw new ArgumentNullException("disposable");

            // need compare for avoid iOS AOT
            if (onNext == Stubs.Ignore<T>)
            {
                return new EmptyOnNextAnonymousObserver<T>(onError, onCompleted, disposable);
            }
            else
            {
                return new AnonymousObserver<T>(onNext, onError, onCompleted, disposable);
            }
        }

        class AnonymousObserver<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;
            readonly IDisposable disposable;

            int isStopped = 0;

            public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted, IDisposable disposable)
            {
                this.onNext = onNext;
                this.onError = onError;
                this.onCompleted = onCompleted;
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
                        onError(error);
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
                        onCompleted();
                    }
                    finally
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        class EmptyOnNextAnonymousObserver<T> : IObserver<T>
        {
            readonly Action<Exception> onError;
            readonly Action onCompleted;
            readonly IDisposable disposable;

            int isStopped = 0;

            public EmptyOnNextAnonymousObserver(Action<Exception> onError, Action onCompleted, IDisposable disposable)
            {
                this.onError = onError;
                this.onCompleted = onCompleted;
                this.disposable = disposable;
            }

            public void OnNext(T value)
            {
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref isStopped) == 1)
                {
                    try
                    {
                        onError(error);
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
                        onCompleted();
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