using System;

#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif

namespace UniRx.Operators
{
    internal class SampleObservableObservable<T, T2> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<T2> intervalSource;

        public SampleObservableObservable(IObservable<T> source, IObservable<T2> intervalSource)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.intervalSource = intervalSource;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new SampleObservable(this, observer, cancel).Run();
        }

        class SampleObservable : OperatorObserverBase<T, T>
        {
            readonly SampleObservableObservable<T, T2> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool isUpdated = false;
            bool isCompleted = false;
            SingleAssignmentDisposable sourceSubscription;

            public SampleObservable(
                SampleObservableObservable<T, T2> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                var scheduling = this.parent.intervalSource
                    .Subscribe(new SampleObservableTick(this));

                return StableCompositeDisposable.Create(sourceSubscription, scheduling);
            }

            void OnNextTick(T2 _)
            {
                lock (gate)
                {
                    if (isUpdated)
                    {
                        var value = latestValue;
                        isUpdated = false;
                        observer.OnNext(value);
                    }
                    if (isCompleted)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); }
                    }
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    latestValue = value;
                    isUpdated = true;
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { base.observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isCompleted = true;
                    sourceSubscription.Dispose();
                }
            }
            class SampleObservableTick : IObserver<T2>
            {
                readonly SampleObservable parent;

                public SampleObservableTick(SampleObservable parent)
                {
                    this.parent = parent;
                }

                public void OnCompleted()
                {
                }

                public void OnError(Exception error)
                {
                }

                public void OnNext(T2 _)
                {
                    lock (parent.gate)
                    {
                        if (parent.isUpdated)
                        {
                            var value = parent.latestValue;
                            parent.isUpdated = false;
                            parent.observer.OnNext(value);
                        }
                        if (parent.isCompleted)
                        {
                            try { parent.observer.OnCompleted(); } finally { parent.Dispose(); }
                        }
                    }
                }
            }
        }
    }
}

