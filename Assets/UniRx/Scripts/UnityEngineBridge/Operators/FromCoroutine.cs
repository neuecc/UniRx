using System;
using System.Collections;

namespace UniRx.Operators
{
    internal class FromCoroutine<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;

        public FromCoroutine(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
            : base(false)
        {
            this.coroutine = coroutine;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var fromCoroutineObserver = new FromCoroutineObserver(observer, cancel);

            var moreCancel = new BooleanDisposable();

            MainThreadDispatcher.SendStartCoroutine(coroutine(fromCoroutineObserver, new CancellationToken(moreCancel)));

            return moreCancel;
        }

        class FromCoroutineObserver : OperatorObserverBase<T, T>
        {
            public FromCoroutineObserver(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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