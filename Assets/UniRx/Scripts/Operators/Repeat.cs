using System;

namespace UniRx.Operators
{
    internal class RepeatObservable<T> : OperatorObservableBase<T>
    {
        readonly T value;
        readonly int? repeatCount;
        readonly IScheduler scheduler;

        public RepeatObservable(T value, int? repeatCount, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.value = value;
            this.repeatCount = repeatCount;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Repeat(observer, cancel);

            if (repeatCount == null)
            {
                return scheduler.Schedule((Action self) =>
                {
                    observer.OnNext(value);
                    self();
                });
            }
            else
            {
                var currentCount = this.repeatCount.Value;
                return scheduler.Schedule((Action self) =>
                {
                    if (currentCount > 0)
                    {
                        observer.OnNext(value);
                        currentCount--;
                    }

                    if (currentCount == 0)
                    {
                        observer.OnCompleted();
                        return;
                    }

                    self();
                });
            }
        }

        class Repeat : AutoDetachOperatorObserverBase<T>
        {
            public Repeat(IObserver<T> observer, IDisposable cancel)
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