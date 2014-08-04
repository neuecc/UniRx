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
            Func<object, object, object> getValue = null;

            var memberInfo = ((MemberExpression)propertySelector.Body).Member;
            var memberName = memberInfo.Name;

            if (memberInfo is PropertyInfo)
            {
                getValue = (obj, mi) => ((PropertyInfo)mi).GetValue(obj, null);
            }
            else if (memberInfo is FieldInfo)
            {
                getValue = (obj, mi) => ((FieldInfo)mi).GetValue(obj);
            }
            else
            {
                throw new ArgumentException("Invalid Expression");
            }

            var currentValue = (TProperty)getValue(source, memberInfo);

            var everyValueChanged = Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishValueChanged<TProperty>(source, memberInfo, getValue, currentValue, observer, cancellationToken));

            // publish currentValue before run valuechanged
            return Observable.Return(currentValue)
                .Concat(everyValueChanged);
        }

        static IEnumerator PublishValueChanged<T>(object source, MemberInfo memberInfo, Func<object, object, object> getValue, T firstValue, IObserver<T> observer, CancellationToken cancellationToken)
        {
            T prevValue = firstValue;
            T currentValue = default(T);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    currentValue = (T)getValue(source, memberInfo);
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