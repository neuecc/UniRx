using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Operators
{
    //public class SwitchObservable<T> : OperatorObservableBase<T>
    //{
    //    readonly IObservable<IObservable<T>> sources;

    //    public Switch(IObservable<IObservable<T>> sources)
    //        : base(true)
    //    {
    //        this.sources = sources;
    //    }

    //    protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
    //    {
    //        return new SwitchObserver(this, observer, cancel).Run();
    //    }

    //    class SwitchObserver : OperatorObserverBase<IObservable<T>, T>
    //    {
    //        readonly SwitchObservable<T> parent;

    //        readonly object gate = new object();
    //        readonly SerialDisposable innerSubscription = new SerialDisposable();
    //        bool isStopped = false;
    //        ulong latest = 0UL;
    //        bool hasLatest = false;

    //        public SwitchObserver(SwitchObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
    //        {
    //            this.parent = parent;
    //        }

    //        public IDisposable Run()
    //        {
    //            var subscription = parent.sources.Subscribe(this);
    //            return StableCompositeDisposable.Create(subscription, innerSubscription);
    //        }

    //        public override void OnNext(IObservable<T> value)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public override void OnError(Exception error)
    //        {
    //            base.OnError(error);
    //        }

    //        public override void OnCompleted()
    //        {
    //            base.OnCompleted();
    //        }
    //    }
    //}
}
