﻿using System;

#if SystemReactive
using System.Reactive.Concurrency;

namespace System.Reactive.Linq
#else
namespace UniRx.Operators
#endif
{
    internal class Range : OperatorObservableBase<int>
    {
        readonly int start;
        readonly int count;
        readonly IScheduler scheduler;

        int i = 0;

        public Range(int start, int count, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.start = start;
            this.count = count;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<int> observer, IDisposable cancel)
        {
            observer = new RangeObserver(observer, cancel);

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
        }
    }
}