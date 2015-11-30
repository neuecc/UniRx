using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ToObservable<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<T> source;
        readonly IScheduler scheduler;

        public ToObservable(IEnumerable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ToObservableObserver(this, observer, cancel).Run();
        }

        class ToObservableObserver : OperatorObserverBase<T, T>
        {
            readonly ToObservable<T> parent;

            public ToObservableObserver(ToObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var e = default(IEnumerator<T>);
                try
                {
                    e = parent.source.GetEnumerator();
                }
                catch (Exception exception)
                {
                    OnError(exception);
                    return Disposable.Empty;
                }

                if (parent.scheduler == Scheduler.Immediate)
                {
                    while (true)
                    {
                        bool hasNext;
                        var current = default(T);
                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext) current = e.Current;
                        }
                        catch (Exception ex)
                        {
                            e.Dispose();
                            base.OnError(ex);
                            break;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                        else
                        {
                            e.Dispose();
                            base.OnCompleted();
                            break;
                        }
                    }

                    return Disposable.Empty;
                }

                var flag = new SingleAssignmentDisposable();
                flag.Disposable = parent.scheduler.Schedule(self =>
                {
                    if (flag.IsDisposed)
                    {
                        e.Dispose();
                        return;
                    }

                    bool hasNext;
                    var current = default(T);
                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext) current = e.Current;
                    }
                    catch (Exception ex)
                    {
                        e.Dispose();
                        base.OnError(ex);
                        return;
                    }

                    if (hasNext)
                    {
                        observer.OnNext(current);
                        self();
                    }
                    else
                    {
                        e.Dispose();
                        base.OnCompleted();
                    }
                });

                return flag;
            }

            public override void OnNext(T value)
            {
                // do nothing
            }
        }
    }
}