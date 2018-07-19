#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UniRx.Async
{
    public static partial class UnityAsyncExtensions
    {
        public static AsyncUnityEventHandler GetAsyncEventHandler(this UnityEvent unityEvent)
        {
            return new AsyncUnityEventHandler(unityEvent);
        }

        public static async UniTask OnInvokeAsync(this UnityEvent unityEvent)
        {
            using (var handler = unityEvent.GetAsyncEventHandler())
            {
                await handler.OnInvokeAsync();
            }
        }

        public static IAsyncClickEventHandler GetAsyncEventHandler(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick);
        }

        public static async UniTask OnInvokeAsync(this Button button)
        {
            using (var handler = button.GetAsyncEventHandler())
            {
                await handler.OnClickAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<bool> GetAsyncEventHandler(this Toggle toggle)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged);
        }

        public static async UniTask<bool> OnValueChangedAsync(this Toggle toggle)
        {
            using (var handler = toggle.GetAsyncEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncEventHandler(this Scrollbar scrollbar)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged);
        }

        public static async UniTask<float> OnValueChangedAsync(this Scrollbar scrollbar)
        {
            using (var handler = scrollbar.GetAsyncEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<Vector2> GetAsyncEventHandler(this ScrollRect scrollRect)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged);
        }

        public static async UniTask<Vector2> OnValueChangedAsync(this ScrollRect scrollRect)
        {
            using (var handler = scrollRect.GetAsyncEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncEventHandler(this Slider slider)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged);
        }

        public static async UniTask<float> OnValueChangedAsync(this Slider slider)
        {
            using (var handler = slider.GetAsyncEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncEndEditEventHandler<string> GetAsyncEventHandler(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit);
        }

        public static async UniTask<string> OnEndEditAsync(this InputField inputField)
        {
            using (var handler = inputField.GetAsyncEventHandler())
            {
                return await handler.OnEndEditAsync();
            }
        }
    }

    public interface IAsyncClickEventHandler : IDisposable
    {
        UniTask OnClickAsync();
    }

    public interface IAsyncValueChangedEventHandler<T> : IDisposable
    {
        UniTask<T> OnValueChangedAsync();
    }

    public interface IAsyncEndEditEventHandler<T> : IDisposable
    {
        UniTask<T> OnEndEditAsync();
    }

    public class AsyncUnityEventHandler : IAwaiter<AsyncUnit>, IDisposable, IAsyncClickEventHandler
    {
        readonly UnityAction action;
        readonly UnityEvent unityEvent;
        Action continuation;

        public AsyncUnityEventHandler(UnityEvent unityEvent)
        {
            action = Invoke;
            unityEvent.AddListener(action);
        }

        public UniTask OnInvokeAsync()
        {
            // zero allocation wait handler.
            return new UniTask(this);
        }

        void Invoke()
        {
            var c = continuation;
            continuation = null;
            if (c != null)
            {
                c.Invoke();
            }
        }

        public void Dispose()
        {
            if (unityEvent != null)
            {
                unityEvent.RemoveListener(action);
            }
        }

        bool IAwaiter.IsCompleted => false;

        void IAwaiter.GetResult()
        {
        }

        public AsyncUnit GetResult()
        {
            return AsyncUnit.Default;
        }

        void INotifyCompletion.OnCompleted(Action action)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(action);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action action)
        {
            if (continuation != null) throw new InvalidOperationException("can't add multiple continuation(await)");
            this.continuation = action;
        }

        // Interface events.

        UniTask IAsyncClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }
    }

    public class AsyncUnityEventHandler<T> : IAwaiter<T>, IDisposable, IAsyncValueChangedEventHandler<T>, IAsyncEndEditEventHandler<T>
    {
        readonly UnityAction<T> action;
        readonly UnityEvent<T> unityEvent;
        Action continuation;
        T eventValue;

        public AsyncUnityEventHandler(UnityEvent<T> unityEvent)
        {
            action = Invoke;
            unityEvent.AddListener(action);
        }

        public UniTask<T> OnInvokeAsync()
        {
            // zero allocation wait handler.
            return new UniTask<T>(this);
        }

        void Invoke(T value)
        {
            this.eventValue = value;

            var c = continuation;
            continuation = null;
            if (c != null)
            {
                c.Invoke();
            }
        }

        public void Dispose()
        {
            if (unityEvent != null)
            {
                unityEvent.RemoveListener(action);
            }
        }

        bool IAwaiter.IsCompleted => false;

        T IAwaiter<T>.GetResult()
        {
            return eventValue;
        }

        void IAwaiter.GetResult()
        {
        }

        void INotifyCompletion.OnCompleted(Action action)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(action);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action action)
        {
            if (continuation != null) throw new InvalidOperationException("can't add multiple continuation(await)");
            continuation = action;
        }

        // Interface events.

        UniTask<T> IAsyncValueChangedEventHandler<T>.OnValueChangedAsync()
        {
            return OnInvokeAsync();
        }

        UniTask<T> IAsyncEndEditEventHandler<T>.OnEndEditAsync()
        {
            return OnInvokeAsync();
        }
    }
}

#endif