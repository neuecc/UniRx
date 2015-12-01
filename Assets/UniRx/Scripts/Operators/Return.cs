using System;

namespace UniRx.Operators
{
    internal class ReturnObservable<T> : OperatorObservableBase<T>
    {
        readonly T value;
        readonly IScheduler scheduler;

        public ReturnObservable(T value, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.value = value;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Return(observer, cancel);

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

        class Return : AutoDetachOperatorObserverBase<T>
        {
            public Return(IObserver<T> observer, IDisposable cancel) 
                : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }
        }
    }
}
