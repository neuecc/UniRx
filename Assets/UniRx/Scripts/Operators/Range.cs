using System;

namespace UniRx.Operators
{
    internal class Range : OperatorObservableBase<int>
    {
        readonly int start;
        readonly int count;
        readonly IScheduler scheduler;

        public Range(int start, int count, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count < 0");

            this.start = start;
            this.count = count;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<int> observer, IDisposable cancel)
        {
            observer = new RangeObserver(observer, cancel);

            var i = 0;
            return scheduler.Schedule((Action self) =>
            {
                if (i < count)
                {
                    int v = start + i;
                    observer.OnNext(v);
                    i++;
                    self();
                }
                else
                {
                    observer.OnCompleted();
                }
            });
        }

        class RangeObserver : AutoDetachOperatorObserverBase<int>
        {
            public RangeObserver(IObserver<int> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(int value)
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