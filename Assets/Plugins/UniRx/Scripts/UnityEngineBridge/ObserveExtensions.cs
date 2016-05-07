using System;
using System.Collections;
using System.Collections.Generic;

#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

namespace UniRx
{
    public static partial class ObserveExtensions
    {
        /// <summary>
        /// Publish target property when value is changed. If source is destroyed/destructed, publish OnCompleted.
        /// </summary>
        public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector, FrameCountType frameCountType = FrameCountType.Update)
            where TSource : class
        {
            if (source == null) return Observable.Empty<TProperty>();

            var unityObject = source as UnityEngine.Object;
            var isUnityObject = source is UnityEngine.Object;
            if (isUnityObject && unityObject == null) return Observable.Empty<TProperty>();

            if (isUnityObject)
            {
                return ObservableUnity.FromMicroCoroutine<TProperty>((observer, cancellationToken) => PublishUnityObjectValueChanged(unityObject, propertySelector, observer, cancellationToken), frameCountType);
            }
            else
            {
                var reference = new WeakReference(source);
                source = null;
                return ObservableUnity.FromMicroCoroutine<TProperty>((observer, cancellationToken) => PublishPocoValueChanged(reference, propertySelector, observer, cancellationToken), frameCountType);
            }
        }

        static IEnumerator PublishPocoValueChanged<TSource, TProperty>(WeakReference sourceReference, Func<TSource, TProperty> propertySelector, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
            var comparer = UnityEqualityComparer.GetDefault<TProperty>();

            var isFirst = true;
            var currentValue = default(TProperty);
            var prevValue = default(TProperty);

            while (!cancellationToken.IsCancellationRequested)
            {
                var target = sourceReference.Target;
                if (target != null)
                {
                    try
                    {
                        currentValue = propertySelector((TSource)target);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    finally
                    {
                        target = null; // remove reference(must need!)
                    }
                }
                else
                {
                    observer.OnCompleted();
                    yield break;
                }


                if (isFirst || !comparer.Equals(currentValue, prevValue))
                {
                    isFirst = false;
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return null;
            }
        }

        static IEnumerator PublishUnityObjectValueChanged<TSource, TProperty>(UnityEngine.Object unityObject, Func<TSource, TProperty> propertySelector, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
            var comparer = UnityEqualityComparer.GetDefault<TProperty>();

            var isFirst = true;
            var currentValue = default(TProperty);
            var prevValue = default(TProperty);

            var source = (TSource)(object)unityObject;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (unityObject != null)
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
                }
                else
                {
                    observer.OnCompleted();
                    yield break;
                }

                if (isFirst || !comparer.Equals(currentValue, prevValue))
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