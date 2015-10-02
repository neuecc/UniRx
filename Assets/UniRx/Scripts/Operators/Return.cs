using System;

namespace UniRx.Operators
{
    internal class Return<T> : OperatorObservableBase<T>
    {
        readonly T value;
        readonly IScheduler scheduler;

        public Return(T value, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.value = value;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == Scheduler.Immediate)
            {
                observer.OnNext(value);
                observer.OnCompleted();
                return Disposable.Empty;
            }
            else
            {
                return scheduler.Schedule(() =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });
            }
        }
    }
}
