using System;

namespace UniRx.Operators
{
    internal class TimeInterval<T> : OperatorObservableBase<UniRx.TimeInterval<T>>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public TimeInterval(IObservable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<UniRx.TimeInterval<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new TimestampObserver(this, observer, cancel));
        }

        class TimestampObserver : OperatorObserverBase<T, UniRx.TimeInterval<T>>
        {
            readonly TimeInterval<T> parent;
            DateTimeOffset lastTime;

            public TimestampObserver(TimeInterval<T> parent, IObserver<UniRx.TimeInterval<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.lastTime = parent.scheduler.Now;
            }

            public override void OnNext(T value)
            {
                var now = parent.scheduler.Now;
                var span = now.Subtract(lastTime);
                lastTime = now;

                base.observer.OnNext(new UniRx.TimeInterval<T>(value, span));
            }
        }
    }
}