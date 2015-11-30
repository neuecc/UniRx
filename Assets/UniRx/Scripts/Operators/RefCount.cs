using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class RefCount<T> : OperatorObservableBase<T>
    {
        readonly IConnectableObservable<T> source;
        readonly object gate = new object();
        int refCount = 0;
        IDisposable connection;

        public RefCount(IConnectableObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new RefCountObserver(this, observer, cancel).Run();
        }

        class RefCountObserver : OperatorObserverBase<T, T>
        {
            readonly RefCount<T> parent;

            public RefCountObserver(RefCount<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var subcription = parent.source.Subscribe(this);

                lock (parent.gate)
                {
                    if (++parent.refCount == 1)
                    {
                        parent.connection = parent.source.Connect();
                    }
                }

                return Disposable.Create(() =>
                {
                    subcription.Dispose();

                    lock (parent.gate)
                    {
                        if (--parent.refCount == 0)
                        {
                            parent.connection.Dispose();
                        }
                    }
                });
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }
        }
    }
}