using System;
using System.Collections;

namespace UniRx.Operators
{
    internal class FromCoroutineObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;

        public FromCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
            : base(false)
        {
            this.coroutine = coroutine;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var fromCoroutineObserver = new FromCoroutine(observer, cancel);

            var moreCancel = new BooleanDisposable();

            MainThreadDispatcher.SendStartCoroutine(coroutine(fromCoroutineObserver, new CancellationToken(moreCancel)));

            return moreCancel;
        }

        class FromCoroutine : OperatorObserverBase<T, T>
        {
            public FromCoroutine(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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

    internal class FromMicroCoroutineObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;

        public FromMicroCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
            : base(false)
        {
            this.coroutine = coroutine;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var fromCoroutineObserver = new FromMicroCoroutine(observer, cancel);

            var moreCancel = new BooleanDisposable();

            MainThreadDispatcher.StartMicroCoroutine(coroutine(fromCoroutineObserver, new CancellationToken(moreCancel)));

            return moreCancel;
        }

        class FromMicroCoroutine : OperatorObserverBase<T, T>
        {
            public FromMicroCoroutine(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
}