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

        public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick);
        }

        public static async UniTask OnInvokeAsync(this Button button)
        {
            using (var handler = button.GetAsyncClickEventHandler())
            {
                await handler.OnClickAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<bool> GetAsyncValueChangedEventHandler(this Toggle toggle)
        {
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged);
        }

        public static async UniTask<bool> OnValueChangedAsync(this Toggle toggle)
        {
            using (var handler = toggle.GetAsyncValueChangedEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Scrollbar scrollbar)
        {
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged);
        }

        public static async UniTask<float> OnValueChangedAsync(this Scrollbar scrollbar)
        {
            using (var handler = scrollbar.GetAsyncValueChangedEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<Vector2> GetAsyncValueChangedEventHandler(this ScrollRect scrollRect)
        {
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged);
        }

        public static async UniTask<Vector2> OnValueChangedAsync(this ScrollRect scrollRect)
        {
            using (var handler = scrollRect.GetAsyncValueChangedEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<float> GetAsyncValueChangedEventHandler(this Slider slider)
        {
            return new AsyncUnityEventHandler<float>(slider.onValueChanged);
        }

        public static async UniTask<float> OnValueChangedAsync(this Slider slider)
        {
            using (var handler = slider.GetAsyncValueChangedEventHandler())
            {
                return await handler.OnValueChangedAsync();
            }
        }

        public static IAsyncEndEditEventHandler<string> GetAsyncEndEditEventHandler(this InputField inputField)
        {
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit);
        }

        public static async UniTask<string> OnEndEditAsync(this InputField inputField)
        {
            using (var handler = inputField.GetAsyncEndEditEventHandler())
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
        AwaiterStatus status;

        public AsyncUnityEventHandler(UnityEvent unityEvent)
        {
            action = Invoke;
            status = AwaiterStatus.Pending;
            unityEvent.AddListener(action);
            this.unityEvent = unityEvent;
        }

        public UniTask OnInvokeAsync()
        {
            // zero allocation wait handler.
            return new UniTask(this);
        }

        void Invoke()
        {
            status = AwaiterStatus.Succeeded;

            var c = continuation;
            continuation = null;
            if (c != null)
            {
                c.Invoke();
            }

            status = AwaiterStatus.Pending;
        }

        public void Dispose()
        {
            if (unityEvent != null)
            {
                unityEvent.RemoveListener(action);
            }
        }

        bool IAwaiter.IsCompleted => false;

        AwaiterStatus IAwaiter.Status => status;

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
        AwaiterStatus status;

        public AsyncUnityEventHandler(UnityEvent<T> unityEvent)
        {
            action = Invoke;
            status = AwaiterStatus.Pending;
            unityEvent.AddListener(action);
            this.unityEvent = unityEvent;
        }

        public UniTask<T> OnInvokeAsync()
        {
            // zero allocation wait handler.
            return new UniTask<T>(this);
        }

        void Invoke(T value)
        {
            this.eventValue = value;
            status = AwaiterStatus.Succeeded;

            var c = continuation;
            continuation = null;
            if (c != null)
            {
                c.Invoke();
            }

            status = AwaiterStatus.Pending;
        }

        public void Dispose()
        {
            if (unityEvent != null)
            {
                unityEvent.RemoveListener(action);
            }
        }

        bool IAwaiter.IsCompleted => false;
        AwaiterStatus IAwaiter.Status => status;

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