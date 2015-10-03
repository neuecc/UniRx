using System;

namespace UniRx.Operators
{
    internal class Never<T> : OperatorObservableBase<T>
    {
        public Never()
            : base(false)
        {
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return Disposable.Empty;
        }
    }
}