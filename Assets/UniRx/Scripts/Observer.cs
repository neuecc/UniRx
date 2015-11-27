using System;
using System.Threading;

namespace UniRx
{
    internal interface ISafeObserver
    {

    }

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

        class AnonymousObserver<T> : IObserver<T>, ISafeObserver
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

        class EmptyOnNextAnonymousObserver<T> : IObserver<T>, ISafeObserver
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

        class DelegatedOnNextObserver<T, TRoot> : UniRx.Operators.OperatorObserverBase<T, TRoot>
        {
            readonly Action<T> onNext;

            public DelegatedOnNextObserver(Action<T> onNext, IObserver<TRoot> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.onNext = onNext;
            }

            public override void OnNext(T value)
            {
                try
                {
                    this.onNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }
        }

        class AutoDetachObserver<T> : UniRx.Operators.AutoDetachOperatorObserverBase<T>
        {
            public AutoDetachObserver(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {

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