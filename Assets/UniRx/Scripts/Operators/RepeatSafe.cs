using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class RepeatSafe<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<IObservable<T>> sources;

        public RepeatSafe(IEnumerable<IObservable<T>> sources, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.sources = sources;
        }

        // need to make RepeatSafeObserver but difficult...
        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var isDisposed = false;
            var isRunNext = false;
            var e = sources.AsSafeEnumerable().GetEnumerator();
            var subscription = new SerialDisposable();
            var gate = new object();

            var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(self =>
            {
                lock (gate)
                {
                    if (isDisposed) return;

                    var current = default(IObservable<T>);
                    var hasNext = false;
                    var ex = default(Exception);

                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext)
                        {
                            current = e.Current;
                            if (current == null) throw new InvalidOperationException("sequence is null.");
                        }
                        else
                        {
                            e.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                        e.Dispose();
                    }

                    if (ex != null)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    if (!hasNext)
                    {
                        observer.OnCompleted();
                        return;
                    }

                    var source = e.Current;
                    var d = new SingleAssignmentDisposable();
                    subscription.Disposable = d;

                    d.Disposable = source.Subscribe(x =>
                    {
                        isRunNext = true;
                        observer.OnNext(x);
                    }, observer.OnError, () =>
                    {
                        if (isRunNext && !isDisposed)
                        {
                            isRunNext = false;
                            self();
                        }
                        else
                        {
                            e.Dispose();
                            if (!isDisposed)
                            {
                                observer.OnCompleted();
                            }
                        }
                    });
                }
            });

            return StableCompositeDisposable.Create(schedule, subscription, Disposable.Create(() =>
            {
                lock (gate)
                {
                    isDisposed = true;
                    e.Dispose();
                }
            }));
        }
    }
}