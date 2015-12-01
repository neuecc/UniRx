using System;

namespace UniRx.Operators
{
    internal class OfTypeObservable<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;

        public OfTypeObservable(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return source.Subscribe(new OfType(this, observer, cancel));
        }

        class OfType : OperatorObserverBase<TSource, TResult>
        {
            readonly OfTypeObservable<TSource, TResult> parent;

            public OfType(OfTypeObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(TSource value)
            {
                if (value is TResult)
                {
                    var castValue = (TResult)(object)value;
                    observer.OnNext(castValue);
                }
            }
        }
    }
}