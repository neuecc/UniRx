#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
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

        public static IAsyncButtonClickEventHandler GetAsyncEventHandler(this Button button)
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
    }

    public interface IAsyncButtonClickEventHandler : IDisposable
    {
        UniTask OnClickAsync();
    }


    public class AsyncUnityEventHandler : IPromise<AsyncUnit>, IDisposable, IAsyncButtonClickEventHandler
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

        // IPromise(like IValueTaskSource on .NET Core 2.1's ValueTask/System.IO.Pipeline optimization)

        bool IPromise<AsyncUnit>.IsCompleted => false;

        AsyncUnit IPromise<AsyncUnit>.GetResult()
        {
            return AsyncUnit.Default;
        }

        void IPromise<AsyncUnit>.RegisterContinuation(Action action)
        {
            if (continuation != null) throw new InvalidOperationException("can't add multiple continuation(await)");

            continuation = action;
        }

        // Interface events.

        UniTask IAsyncButtonClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }
    }
}

#endif