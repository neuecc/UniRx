using System;

namespace UniRx.Operators
{
    internal class AsUnitObservable<T> : OperatorObservableBase<Unit>
    {
        readonly IObservable<T> source;

        public AsUnitObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            return source.Subscribe(new AsUnitObservableObserver(observer, cancel));
        }

        class AsUnitObservableObserver : OperatorObserverBase<T, Unit>
        {
            public AsUnitObservableObserver(IObserver<Unit> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(Unit.Default);
            }
        }
    }
}