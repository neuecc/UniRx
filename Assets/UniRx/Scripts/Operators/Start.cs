using System;

namespace UniRx.Operators
{
    internal class StartObservable<T> : OperatorObservableBase<T>
    {
        readonly Action action;
        readonly Func<T> function;
        readonly IScheduler scheduler;
        readonly TimeSpan? startAfter;

        public StartObservable(Func<T> function, TimeSpan? startAfter, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.function = function;
            this.startAfter = startAfter;
            this.scheduler = scheduler;
        }

        public StartObservable(Action action, TimeSpan? startAfter, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.action = action;
            this.startAfter = startAfter;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (startAfter != null)
            {
                return scheduler.Schedule(startAfter.Value, new StartObserver(this, observer, cancel).Run);
            }
            else
            {
                return scheduler.Schedule(new StartObserver(this, observer, cancel).Run);
            }
        }

        class StartObserver : AutoDetachOperatorObserverBase<T>
        {
            readonly StartObservable<T> parent;

            public StartObserver(StartObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public void Run()
            {
                var result = default(T);
                try
                {
                    if (parent.function != null)
                    {
                        result = parent.function();
                    }
                    else
                    {
                        parent.action();
                        // TODO:hack for Unit.Default as class, after changing unit as struct, remove this
                        if (typeof(T) == typeof(Unit))
                        {
                            result = (T)(object)Unit.Default;
                        }
                    }
                }
                catch (Exception exception)
                {
                    OnError(exception);
                    return;
                }

                OnNext(result);
                OnCompleted();
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