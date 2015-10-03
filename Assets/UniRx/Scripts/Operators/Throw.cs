using System;

namespace UniRx.Operators
{
    internal class Throw<T> : OperatorObservableBase<T>
    {
        readonly Exception error;
        readonly IScheduler scheduler;

        public Throw(Exception error, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.error = error;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == Scheduler.Immediate)
            {
                observer.OnError(error);
                return Disposable.Empty;
            }
            else
            {
                return scheduler.Schedule(() =>
                {
                    observer.OnError(error);
                    observer.OnCompleted();
                });
            }
        }
    }
}
