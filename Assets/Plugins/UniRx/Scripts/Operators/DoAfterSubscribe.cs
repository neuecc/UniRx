using System;
using UniRx.Operators;
namespace UniRx.Operators
{
    internal class DoAfterSubscribeObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action afterSubscribe;

        public DoAfterSubscribeObservable(IObservable<T> source, Action afterSubscribe)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.afterSubscribe = afterSubscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DoAfterSubscribe(this, observer, cancel).Run();
        }

        class DoAfterSubscribe : OperatorObserverBase<T, T>
        {
            readonly DoAfterSubscribeObservable<T> parent;

            public DoAfterSubscribe(DoAfterSubscribeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var p = parent.source.Subscribe(this);
                try
                {
                    parent.afterSubscribe();
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return Disposable.Empty;
                }

                return p;
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }

}
namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<T> DoAfterSubscribe<T>(this IObservable<T> source, Action afterSubscribe)
        {
            return new DoAfterSubscribeObservable<T>(source, afterSubscribe);
        }
    }
}