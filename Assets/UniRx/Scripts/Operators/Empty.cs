using System;

#if SystemReactive
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
#else
namespace UniRx.Operators
#endif
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
        }
    }
}