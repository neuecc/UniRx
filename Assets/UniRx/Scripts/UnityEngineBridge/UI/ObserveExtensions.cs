using System;
using System.Collections;

namespace UniRx.UI
{
    public static partial class ObserveExtensions
    {
        public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector)
        {
            // same as : Observable.EveryUpdate().Select(_ => propertySelector(source)).DistinctUntilChanged();
            var currentValue = propertySelector(source);
            var everyValueChanged = Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishValueChanged(source, propertySelector, currentValue, observer, cancellationToken));

            // publish currentValue before run valuechanged
            return everyValueChanged.StartWith(currentValue);
        }

        static IEnumerator PublishValueChanged<TSource, TProperty>(TSource source, Func<TSource, TProperty> propertySelector, TProperty firstValue, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
            TProperty prevValue = firstValue;
            TProperty currentValue = default(TProperty);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    currentValue = propertySelector(source);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }

                if (!object.Equals(currentValue, prevValue))
                {
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return null;
            }
        }
    }
}