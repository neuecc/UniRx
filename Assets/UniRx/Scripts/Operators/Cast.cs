using System;

namespace UniRx.Operators
{
    internal class CastObservable<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;

        public CastObservable(IObservable<TSource> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return source.Subscribe(new Cast(this, observer, cancel));
        }

        class Cast : OperatorObserverBase<TSource, TResult>
        {
            readonly CastObservable<TSource, TResult> parent;

            public Cast(CastObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
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