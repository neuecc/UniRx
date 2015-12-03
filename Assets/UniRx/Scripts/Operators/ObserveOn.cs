using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ObserveOnObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public ObserveOnObservable(IObservable<T> source, IScheduler scheduler)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ObserveOn(this, observer, cancel).Run();
        }

        class ObserveOn : OperatorObserverBase<T, T>
        {
            readonly ObserveOnObservable<T> parent;
            readonly LinkedList<IDisposable> scheduleDisposables = new LinkedList<IDisposable>();
            bool isDisposed;

            public ObserveOn(ObserveOnObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;

                var sourceDisposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(sourceDisposable, Disposable.Create(() =>
                {
                    lock (scheduleDisposables)
                    {
                        isDisposed = true;

                        foreach (var item in scheduleDisposables)
                        {
                            item.Dispose();
                        }
                        scheduleDisposables.Clear();
                    }
                }));
            }

            public override void OnNext(T value)
            {
                var self = new SingleAssignmentDisposable();
                LinkedListNode<IDisposable> node;
                lock (scheduleDisposables)
                {
                    if (isDisposed) return;

                    node = scheduleDisposables.AddLast(self);
                }
                self.Disposable = parent.scheduler.Schedule(() =>
                {
                    self.Dispose();
                    lock (scheduleDisposables)
                    {
                        if (node.List != null)
                        {
                            node.List.Remove(node);
                        }
                    }
                    base.observer.OnNext(value);
                });
            }

            public override void OnError(Exception error)
            {
                var self = new SingleAssignmentDisposable();
                LinkedListNode<IDisposable> node;
                lock (scheduleDisposables)
                {
                    if (isDisposed) return;

                    node = scheduleDisposables.AddLast(self);
                }
                self.Disposable = parent.scheduler.Schedule(() =>
                {
                    self.Dispose();
                    lock (scheduleDisposables)
                    {
                        if (node.List != null)
                        {
                            node.List.Remove(node);
                        }
                    }
                    base.OnError(error);
                });
            }

            public override void OnCompleted()
            {
                var self = new SingleAssignmentDisposable();
                LinkedListNode<IDisposable> node;
                lock (scheduleDisposables)
                {
                    if (isDisposed) return;

                    node = scheduleDisposables.AddLast(self);
                }
                self.Disposable = parent.scheduler.Schedule(() =>
                {
                    self.Dispose();
                    lock (scheduleDisposables)
                    {
                        if (node.List != null)
                        {
                            node.List.Remove(node);
                        }
                    }
                    base.OnCompleted();
                });
            }
        }
    }
}