using System;
using System.Collections;

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
                return Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishUnityObjectValueChanged(unityObject, propertySelector, frameCountType, observer, cancellationToken));
            }
            else
            {
                var reference = new WeakReference(source);
                source = null;
                return Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishPocoValueChanged(reference, propertySelector, frameCountType, observer, cancellationToken));
            }
        }

        static IEnumerator PublishPocoValueChanged<TSource, TProperty>(WeakReference sourceReference, Func<TSource, TProperty> propertySelector, FrameCountType frameCountType, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
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


                if (isFirst || !object.Equals(currentValue, prevValue))
                {
                    isFirst = false;
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return frameCountType.GetYieldInstruction();
            }
        }

        static IEnumerator PublishUnityObjectValueChanged<TSource, TProperty>(UnityEngine.Object unityObject, Func<TSource, TProperty> propertySelector, FrameCountType frameCountType, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
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

                if (isFirst || !object.Equals(currentValue, prevValue))
                {
                    isFirst = false;
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return frameCountType.GetYieldInstruction();
            }
        }
    }
}