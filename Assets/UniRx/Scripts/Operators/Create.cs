using System;

namespace UniRx.Operators
{
    internal class Create<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public Create(Func<IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.subscribe = subscribe;
        }

        public Create(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new CreateObserver(observer, cancel);
            return subscribe(observer) ?? Disposable.Empty;
        }

        class CreateObserver : AutoDetachOperatorObserverBase<T>
        {
            public CreateObserver(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
        }
    }
}