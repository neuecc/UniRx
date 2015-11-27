using System;

namespace UniRx.Operators
{
    internal class Defer<T> : OperatorObservableBase<T>
    {
        readonly Func<IObservable<T>> observableFactory;

        public Defer(Func<IObservable<T>> observableFactory)
            : base(false)
        {
            this.observableFactory = observableFactory;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new DeferObserver(observer, cancel);

            IObservable<T> source;
            try
            {
                source = observableFactory();
            }
            catch (Exception ex)
            {
                source = Observable.Throw<T>(ex);
            }

            return source.Subscribe(observer);
        }

        class DeferObserver : AutoDetachOperatorObserverBase<T>
        {
            public DeferObserver(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
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