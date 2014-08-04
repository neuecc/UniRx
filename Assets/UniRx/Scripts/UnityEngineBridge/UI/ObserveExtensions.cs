using System;
using System.Collections;
using System.Linq.Expressions;
using UniRx.InternalUtil;

namespace UniRx.UI
{
    public static partial class ObserveExtensions
    {
        public static IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertySelector)
        {
            var accessor = ReflectionAccessor.Create(propertySelector.Body as MemberExpression);
            var currentValue = (TProperty)accessor.GetValue(source);

            var everyValueChanged = Observable.FromCoroutine<TProperty>((observer, cancellationToken) => PublishValueChanged(source, accessor, currentValue, observer, cancellationToken));

            // publish currentValue before run valuechanged
            return Observable.Return(currentValue)
                .Concat(everyValueChanged);
        }

        static IEnumerator PublishValueChanged<T>(object source, IReflectionAccessor accessor, T firstValue, IObserver<T> observer, CancellationToken cancellationToken)
        {
            T prevValue = firstValue;
            T currentValue = default(T);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    currentValue = (T)accessor.GetValue(source);
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