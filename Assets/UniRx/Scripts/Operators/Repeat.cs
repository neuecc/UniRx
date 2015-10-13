﻿using System;

#if SystemReactive
using System.Reactive.Concurrency;

namespace System.Reactive.Linq
#else
namespace UniRx.Operators
#endif
{
    internal class Repeat<T> : OperatorObservableBase<T>
    {
        readonly T value;
        readonly int? repeatCount;
        readonly IScheduler scheduler;

        int currentCount;

        public Repeat(T value, int? repeatCount, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.value = value;
            this.repeatCount = repeatCount;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new RepeatObserver(observer, cancel);

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

        class RepeatObserver : AutoDetachOperatorObserverBase<T>
        {
            public RepeatObserver(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }
        }
    }
}