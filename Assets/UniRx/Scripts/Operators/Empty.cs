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
            observer = new EmptyObserver(observer, cancel);

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

        class EmptyObserver : AutoDetachOperatorObserverBase<T>
        {
            public EmptyObserver(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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