using System;

namespace UniRx.Operators
{
    internal class ResetAfterObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly TimeSpan dueTime;
        readonly IScheduler scheduler;
        readonly T defaultValue;

        public ResetAfterObservable(IObservable<T> source, T defaultValue, TimeSpan dueTime, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
            this.defaultValue = defaultValue;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ResetAfter(this, observer, cancel).Run();
        }

        class ResetAfter : OperatorObserverBase<T, T>
        {
            private readonly ResetAfterObservable<T> parent;
            private readonly object gate = new object();
            SerialDisposable cancelable;

            public ResetAfter(ResetAfterObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            private void OnNext()
            {
                lock (gate)
                {
                    observer.OnNext(parent.defaultValue);
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    observer.OnNext(value);

                    var d = new SingleAssignmentDisposable();
                    cancelable.Disposable = d;
                    d.Disposable = parent.scheduler.Schedule(parent.dueTime, OnNext);
                }
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }
        }
    }
}
