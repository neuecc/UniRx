// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx
{
    public static partial class UnityUIComponentExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, Text text)
        {
            return source.Subscribe(x => text.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text)
        {
            return source.Subscribe(x => text.text = x.ToString());
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text, Func<T, string> selector)
        {
            return source.Subscribe(x => text.text = selector(x));
        }

        public static IDisposable SubscribeToInteractable(this IObservable<bool> source, Selectable selectable)
        {
            return source.Subscribe(x => selectable.interactable = x);
        }

        /// <summary>Observe onClick event.</summary>
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            return button.onClick.AsObservable();
        }

        /// <summary>Observe onValueChanged with current `isOn` value on subscribe.</summary>
        public static IObservable<bool> OnValueChangedAsObservable(this Toggle toggle)
        {
            // Optimized Defer + StartWith
            return Observable.Create<bool>(observer =>
            {
                observer.OnNext(toggle.isOn);
                return toggle.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Scrollbar scrollbar)
        {
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(scrollbar.value);
                return scrollbar.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `normalizedPosition` value on subscribe.</summary>
        public static IObservable<Vector2> OnValueChangedAsObservable(this ScrollRect scrollRect)
        {
            return Observable.Create<Vector2>(observer =>
            {
                observer.OnNext(scrollRect.normalizedPosition);
                return scrollRect.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Slider slider)
        {
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(slider.value);
                return slider.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnEndEditAsObservable(this InputField inputField)
        {
            return inputField.onEndEdit.AsObservable();
        }

        /// <summary>Observe onValueChange with current `text` value on subscribe.</summary>
#if !(UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        [Obsolete("onValueChange has been renamed to onValueChanged")]
#endif
        public static IObservable<string> OnValueChangeAsObservable(this InputField inputField)
        {
            return Observable.Create<string>(observer =>
            {
                observer.OnNext(inputField.text);
#if (UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                return inputField.onValueChange.AsObservable().Subscribe(observer);
#else
                return inputField.onValueChanged.AsObservable().Subscribe(observer);
#endif
            });
        }

#if !(UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        /// <summary>Observe onValueChanged with current `text` value on subscribe.</summary>
        public static IObservable<string> OnValueChangedAsObservable(this InputField inputField)
        {
            return Observable.Create<string>(observer =>
            {
                observer.OnNext(inputField.text);
                return inputField.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
#endif
    }
}

#endif