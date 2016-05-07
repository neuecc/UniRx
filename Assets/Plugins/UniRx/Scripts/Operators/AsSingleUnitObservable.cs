using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class AsSingleUnitObservableObservable<T> : OperatorObservableBase<Unit>
    {
        readonly IObservable<T> source;

        public AsSingleUnitObservableObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            return source.Subscribe(new AsSingleUnitObservable(this, observer, cancel));
        }

        class AsSingleUnitObservable : OperatorObserverBase<T, Unit>
        {
            readonly AsSingleUnitObservableObservable<T> parent;

            public AsSingleUnitObservable(AsSingleUnitObservableObservable<T> parent, IObserver<Unit> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                observer.OnNext(Unit.Default);

                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}