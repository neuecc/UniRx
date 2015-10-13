using System;

#if SystemReactive
using System.Reactive.Disposables;

namespace System.Reactive.Linq
#else
namespace UniRx.Operators
#endif
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