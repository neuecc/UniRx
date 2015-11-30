using System;

namespace UniRx.Operators
{
    internal class OfType<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;

        public OfType(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return source.Subscribe(new OfTypeObserver(this, observer, cancel));
        }

        class OfTypeObserver : OperatorObserverBase<TSource, TResult>
        {
            readonly OfType<TSource, TResult> parent;

            public OfTypeObserver(OfType<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
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