#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx.Async.Triggers;

namespace UniRx.Async
{
    public static partial class UnityAsyncExtensions
    {
        public static AsyncUnityEventHandler GetAsyncEventHandler(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            return new AsyncUnityEventHandler(unityEvent, cancellationToken);
        }

        public static async UniTask OnInvokeAsync(this UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            using (var handler = unityEvent.GetAsyncEventHandler(cancellationToken))
            {
                await handler.OnInvokeAsync();
            }
        }

        public static IAsyncClickEventHandler GetAsyncClickEventHandler(this Button button)
        {
            return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy());
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
            return new AsyncUnityEventHandler<bool>(toggle.onValueChanged, toggle.GetCancellationTokenOnDestroy());
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
            return new AsyncUnityEventHandler<float>(scrollbar.onValueChanged, scrollbar.GetCancellationTokenOnDestroy());
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
            return new AsyncUnityEventHandler<Vector2>(scrollRect.onValueChanged, scrollRect.GetCancellationTokenOnDestroy());
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
            return new AsyncUnityEventHandler<float>(slider.onValueChanged, slider.GetCancellationTokenOnDestroy());
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
            return new AsyncUnityEventHandler<string>(inputField.onEndEdit, inputField.GetCancellationTokenOnDestroy());
        }

        public static async UniTask<string> OnEndEditAsync(this InputField inputField)
        {
            using (var handler = inputField.GetAsyncEndEditEventHandler())
            {
                return await handler.OnEndEditAsync();
            }
        }

        public static IAsyncValueChangedEventHandler<int> GetAsyncValueChangedEventHandler(this Dropdown dropdown)
        {
            return new AsyncUnityEventHandler<int>(dropdown.onValueChanged, dropdown.GetCancellationTokenOnDestroy());
        }

        public static async UniTask<int> OnValueChanged(this Dropdown dropdown)
        {
            using (var handler = dropdown.GetAsyncValueChangedEventHandler())
            {
                return await handler.OnValueChangedAsync();
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

    // event handler is reusable.
    public class AsyncUnityEventHandler : IAwaiter, IDisposable, IAsyncClickEventHandler
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly UnityAction action;
        readonly UnityEvent unityEvent;
        Action continuation;
        CancellationTokenRegistration registration;
        bool isDisposed;

        public AsyncUnityEventHandler(UnityEvent unityEvent, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            action = Invoke;
            unityEvent.AddListener(action);
            this.unityEvent = unityEvent;

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.Register(cancellationCallback, this, false);
            }

            TaskTracker.TrackActiveTask(this, 3);
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

        static void CancellationCallback(object state)
        {
            var self = (AsyncUnityEventHandler)state;
            self.Dispose();
            self.Invoke(); // call continuation if exists yet(GetResult -> throw OperationCanceledException).
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                if (unityEvent != null)
                {
                    unityEvent.RemoveListener(action);
                }
            }
        }

        bool IAwaiter.IsCompleted => isDisposed ? true : false;
        AwaiterStatus IAwaiter.Status => isDisposed ? AwaiterStatus.Canceled : AwaiterStatus.Pending;
        void IAwaiter.GetResult()
        {
            if (isDisposed) throw new OperationCanceledException();
        }

        void INotifyCompletion.OnCompleted(Action action)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(action);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action action)
        {
            Error.ThrowWhenContinuationIsAlreadyRegistered(this.continuation);
            this.continuation = action;
        }

        // Interface events.

        UniTask IAsyncClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }
    }

    // event handler is reusable.
    public class AsyncUnityEventHandler<T> : IAwaiter<T>, IDisposable, IAsyncValueChangedEventHandler<T>, IAsyncEndEditEventHandler<T>
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly UnityAction<T> action;
        readonly UnityEvent<T> unityEvent;
        Action continuation;
        CancellationTokenRegistration registration;
        bool isDisposed;
        T eventValue;

        public AsyncUnityEventHandler(UnityEvent<T> unityEvent, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            action = Invoke;
            unityEvent.AddListener(action);
            this.unityEvent = unityEvent;

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.Register(cancellationCallback, this, false);
            }

            TaskTracker.TrackActiveTask(this, 3);
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

        static void CancellationCallback(object state)
        {
            var self = (AsyncUnityEventHandler<T>)state;
            self.Dispose();
            self.Invoke(default(T)); // call continuation if exists yet(GetResult -> throw OperationCanceledException).
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                if (unityEvent != null)
                {
                    unityEvent.RemoveListener(action);
                }
            }
        }

        bool IAwaiter.IsCompleted => isDisposed ? true : false;
        AwaiterStatus IAwaiter.Status => isDisposed ? AwaiterStatus.Canceled : AwaiterStatus.Pending;

        T IAwaiter<T>.GetResult()
        {
            if (isDisposed) throw new OperationCanceledException();
            return eventValue;
        }

        void IAwaiter.GetResult()
        {
            if (isDisposed) throw new OperationCanceledException();
        }

        void INotifyCompletion.OnCompleted(Action action)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(action);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action action)
        {
            Error.ThrowWhenContinuationIsAlreadyRegistered(this.continuation);
            this.continuation = action;
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