using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class AsObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;

        public AsObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new AsObservableObserver(observer, cancel));
        }

        class AsObservableObserver : OperatorObserverBase<T, T>
        {
            public AsObservableObserver(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }
        }
    }
}