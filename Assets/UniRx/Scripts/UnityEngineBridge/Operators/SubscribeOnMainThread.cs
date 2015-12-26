using System;

namespace UniRx.Operators
{
    internal class SubscribeOnMainThreadObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<long> subscribeTrigger;

        public SubscribeOnMainThreadObservable(IObservable<T> source, IObservable<long> subscribeTrigger)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.subscribeTrigger = subscribeTrigger;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var m = new SingleAssignmentDisposable();
            var d = new SerialDisposable();
            d.Disposable = m;

            m.Disposable = subscribeTrigger.Subscribe(_ =>
            {
                d.Disposable = source.Subscribe(observer);
            });

            return d;
        }
    }
}