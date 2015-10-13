using System;

#if SystemReactive
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
#else
namespace UniRx.Operators
#endif
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
            observer = new ReturnObserver(observer, cancel);

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

        class ReturnObserver : AutoDetachOperatorObserverBase<T>
        {
            public ReturnObserver(IObserver<T> observer, IDisposable cancel) 
                : base(observer, cancel)
            {
            }
        }
    }
}
