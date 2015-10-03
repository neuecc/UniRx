using System;

namespace UniRx.Operators
{
    internal class Create<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public Create(Func<IObserver<T>, IDisposable> subscribe)
            : base(false)
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
            return subscribe(observer);
        }
    }
}