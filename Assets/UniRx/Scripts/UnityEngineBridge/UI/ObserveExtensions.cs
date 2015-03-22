using System;
using System.Collections;

namespace UniRx.UI
{
    public static partial class ObserveExtensions
    {
        /// <summary>
        /// Publish target property when value is changed. If source is UnityEngine.Object and when source was destroyed, publish OnCompleted.
        /// </summary>
        public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector)
            where TSource : class
        {
            if (source == null) return Observable.Empty<TProperty>();

            var unityObject = source as UnityEngine.Object;
            var isUnityObject = unityObject != null;
            if (isUnityObject && unityObject == null) return Observable.Empty<TProperty>();

            var everyValueChanged = Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishValueChanged(source, unityObject, isUnityObject, propertySelector, observer, cancellationToken));
            return everyValueChanged;
        }

        static IEnumerator PublishValueChanged<TSource, TProperty>(TSource source, UnityEngine.Object unityObject, bool isUnityObject, Func<TSource, TProperty> propertySelector, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
            var isFirst = true;
            var currentValue = default(TProperty);
            var prevValue = default(TProperty);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!isUnityObject)
                    {
                        if (source != null)
                        {
                            currentValue = propertySelector(source);
                        }
                        else
                        {
                            observer.OnCompleted();
                            yield break;
                        }
                    }
                    else
                    {
                        if (unityObject != null)
                        {
                            currentValue = propertySelector(source);
                        }
                        else
                        {
                            observer.OnCompleted();
                            yield break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }

                if (isFirst || !object.Equals(currentValue, prevValue))
                {
                    isFirst = false;
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return null;
            }
        }
    }
}