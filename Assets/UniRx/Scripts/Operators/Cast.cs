using System;

namespace UniRx.Operators
{
    internal class Cast<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;

        public Cast(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return source.Subscribe(new CastObserver(this, observer, cancel));
        }

        class CastObserver : OperatorObserverBase<TSource, TResult>
        {
            readonly Cast<TSource, TResult> parent;

            public CastObserver(Cast<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(TSource value)
            {
                var castValue = default(TResult);
                try
                {
                    castValue = (TResult)(object)value;
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                observer.OnNext(castValue);
            }
        }
    }
}