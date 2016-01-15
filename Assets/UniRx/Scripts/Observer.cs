using System;
using System.Threading;

namespace UniRx
{
    public static class Observer
    {
        internal static IObserver<T> CreateSubscribeObserver<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            // need compare for avoid iOS AOT
            if (onNext == Stubs.Ignore<T>)
            {
                return new Subscribe_<T>(onError, onCompleted);
            }
            else
            {
                return new Subscribe<T>(onNext, onError, onCompleted);
            }
        }

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

        // same as AnonymousObserver...
        class Subscribe<T> : IObserver<T>
        {
            readonly Action<T> onNext;
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
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

        // same as EmptyOnNextAnonymousObserver...
        class Subscribe_<T> : IObserver<T>
        {
            readonly Action<Exception> onError;
            readonly Action onCompleted;

            int isStopped = 0;

            public Subscribe_(Action<Exception> onError, Action onCompleted)
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

        class AutoDetachObserver<T> : UniRx.Operators.OperatorObserverBase<T, T>
        {
            public AutoDetachObserver(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {

            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }

    public static partial class ObserverExtensions
    {
        public static IObserver<T> Synchronize<T>(this IObserver<T> observer)
        {
            return new UniRx.Operators.SynchronizedObserver<T>(observer, new object());
        }

        public static IObserver<T> Synchronize<T>(this IObserver<T> observer, object gate)
        {
            return new UniRx.Operators.SynchronizedObserver<T>(observer, gate);
        }
    }

    public static partial class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            return source.Subscribe(UniRx.InternalUtil.ThrowObserver<T>.Instance);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, Stubs.Throw, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, onError, Stubs.Nop));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, Stubs.Throw, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(Observer.CreateSubscribeObserver(onNext, onError, onCompleted));
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