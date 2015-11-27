using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class Take<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int count;
        readonly TimeSpan duration;
        readonly IScheduler scheduler;

        public Take(IObservable<T> source, int count)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.count = count;
        }

        // TODO:Timespan Overload

        // optimize combiner

        public IObservable<T> Combine(int count)
        {
            throw new NotImplementedException();
        }

        public IObservable<T> Combine(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == null)
            {
                return source.Subscribe(new TakeObserver(this, observer, cancel));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        class TakeObserver : OperatorObserverBase<T, T>
        {
            readonly Take<T> parent;
            int rest;

            public TakeObserver(Take<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.rest = parent.count;
            }

            public override void OnNext(T value)
            {
                if (rest > 0)
                {
                    rest -= 1;
                    base.observer.OnNext(value);
                    if (rest == 0)
                    {
                        base.OnCompleted();
                    }
                }
            }
        }
    }
}