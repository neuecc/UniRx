using System;

namespace UniRx.Operators
{
    internal class Empty<T> : OperatorObservableBase<T>
    {
        readonly IScheduler scheduler;

        public Empty(IScheduler scheduler)
            : base(false)
        {
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == Scheduler.Immediate)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
            else
            {
                return scheduler.Schedule(observer.OnCompleted);
            }
        }
    }
}