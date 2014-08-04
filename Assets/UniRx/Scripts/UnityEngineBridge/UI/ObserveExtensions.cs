using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace UniRx.UI
{
    public static class ObserveExtensions
    {
        public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertySelector)
        {
            var memberInfo = ((MemberExpression)propertySelector.Body).Member;

            TProperty currentValue;
            IObservable<TProperty> everyValueChanged;

            if (memberInfo is PropertyInfo)
            {
                currentValue = (TProperty)((PropertyInfo)memberInfo).GetGetMethod().Invoke(source, null);
                everyValueChanged = Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishValueChangedProperty<TProperty>(source, (PropertyInfo)memberInfo, currentValue, observer, cancellationToken));
            }
            else if (memberInfo is FieldInfo)
            {
                currentValue = (TProperty)((FieldInfo)memberInfo).GetValue(source);
                everyValueChanged = Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishValueChangedField<TProperty>(source, (FieldInfo)memberInfo, currentValue, observer, cancellationToken));
            }
            else
            {
                throw new ArgumentException("Invalid Expression");
            }

            // publish currentValue before run valuechanged
            return Observable.Return(currentValue)
                .Concat(everyValueChanged);
        }

        static IEnumerator PublishValueChangedProperty<T>(object source, PropertyInfo propertyInfo, T firstValue, IObserver<T> observer, CancellationToken cancellationToken)
        {
            T prevValue = firstValue;
            T currentValue = default(T);
            var methodInfo = propertyInfo.GetGetMethod();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    currentValue = (T)methodInfo.Invoke(source, null);
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

        static IEnumerator PublishValueChangedField<T>(object source, FieldInfo fieldInfo, T firstValue, IObserver<T> observer, CancellationToken cancellationToken)
        {
            T prevValue = firstValue;
            T currentValue = default(T);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    currentValue = (T)fieldInfo.GetValue(source);
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