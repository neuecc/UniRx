using System;

namespace UniRx.Operators
{
    internal class Timestamp<T> : OperatorObservableBase<Timestamped<T>>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public Timestamp(IObservable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<Timestamped<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new TimestampObserver(this, observer, cancel));
        }

        class TimestampObserver : OperatorObserverBase<T, Timestamped<T>>
        {
            readonly Timestamp<T> parent;

            public TimestampObserver(Timestamp<T> parent, IObserver<Timestamped<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(new Timestamped<T>(value, parent.scheduler.Now));
            }
        }
    }
}