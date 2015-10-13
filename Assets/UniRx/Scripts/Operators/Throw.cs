using System;

#if SystemReactive
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace System.Reactive.Linq
#else
namespace UniRx.Operators
#endif
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
            observer = new ThrowObserver(observer, cancel);

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

        class ThrowObserver : AutoDetachOperatorObserverBase<T>
        {
            public ThrowObserver(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }
        }
    }
}
