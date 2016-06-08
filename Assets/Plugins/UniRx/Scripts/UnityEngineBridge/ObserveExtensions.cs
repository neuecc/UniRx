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

            // MicroCoroutine does not publish value immediately, so publish value on subscribe.
            if (isUnityObject)
            {
                return ObservableUnity.FromMicroCoroutine<TProperty>((observer, cancellationToken) =>
                {
                    if (unityObject != null)
                    {
                        var firstValue = default(TProperty);
                        try
                        {
                            firstValue = propertySelector((TSource)(object)unityObject);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return EmptyEnumerator();
                        }

                        observer.OnNext(firstValue);
                        return PublishUnityObjectValueChanged(unityObject, firstValue, propertySelector, observer, cancellationToken);
                    }
                    else
                    {
                        observer.OnCompleted();
                        return EmptyEnumerator();
                    }
                }, frameCountType);
            }
            else
            {
                var reference = new WeakReference(source);
                source = null;

                return ObservableUnity.FromMicroCoroutine<TProperty>((observer, cancellationToken) =>
                {
                    var target = reference.Target;
                    if (target != null)
                    {
                        var firstValue = default(TProperty);
                        try
                        {
                            firstValue = propertySelector((TSource)target);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return EmptyEnumerator();
                        }
                        finally
                        {
                            target = null;
                        }

                        observer.OnNext(firstValue);
                        return PublishPocoValueChanged(reference, firstValue, propertySelector, observer, cancellationToken);
                    }
                    else
                    {
                        observer.OnCompleted();
                        return EmptyEnumerator();
                    }
                }, frameCountType);
            }
        }

        static IEnumerator EmptyEnumerator()
        {
            yield break;
        }

        static IEnumerator PublishPocoValueChanged<TSource, TProperty>(WeakReference sourceReference, TProperty firstValue, Func<TSource, TProperty> propertySelector, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
            var comparer = UnityEqualityComparer.GetDefault<TProperty>();

            var currentValue = default(TProperty);
            var prevValue = firstValue;

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

                if (!comparer.Equals(currentValue, prevValue))
                {
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return null;
            }
        }

        static IEnumerator PublishUnityObjectValueChanged<TSource, TProperty>(UnityEngine.Object unityObject, TProperty firstValue, Func<TSource, TProperty> propertySelector, IObserver<TProperty> observer, CancellationToken cancellationToken)
        {
            var comparer = UnityEqualityComparer.GetDefault<TProperty>();

            var currentValue = default(TProperty);
            var prevValue = firstValue;

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

                if (!comparer.Equals(currentValue, prevValue))
                {
                    observer.OnNext(currentValue);
                    prevValue = currentValue;
                }

                yield return null;
            }
        }
    }
}