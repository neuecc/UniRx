#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async.Internal;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask SwitchToThreadPool()
        {
            return new UniTask(new SwitchToThreadPoolPromise());
        }

        public static UniTask SwitchToTaskPool(CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UniTask(new SwitchToTaskPoolPromise(cancellationToken));
        }

        class SwitchToThreadPoolPromise : IAwaiter
        {
            static readonly WaitCallback switchToCallback = Callback;
            CancellationToken cancellationToken;

            public AwaiterStatus Status => cancellationToken.IsCancellationRequested ? AwaiterStatus.Canceled : AwaiterStatus.Pending;

            public bool IsCompleted => cancellationToken.IsCancellationRequested ? true : false;

            public void GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            public void SetCancellationToken(CancellationToken token)
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellationToken, token);
            }

            public void OnCompleted(Action continuation)
            {
                ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
            }

            static void Callback(object state)
            {
                var continuation = (Action)state;
                continuation();
            }
        }

        class SwitchToTaskPoolPromise : IAwaiter
        {
            static readonly Action<object> switchToCallback = Callback;
            CancellationToken cancellationToken;

            public SwitchToTaskPoolPromise(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public AwaiterStatus Status => cancellationToken.IsCancellationRequested ? AwaiterStatus.Canceled : AwaiterStatus.Pending;

            public bool IsCompleted => cancellationToken.IsCancellationRequested ? true : false;

            public void GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            public void SetCancellationToken(CancellationToken token)
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellationToken, token);
            }

            public void OnCompleted(Action continuation)
            {
                Task.Factory.StartNew(switchToCallback, continuation, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                Task.Factory.StartNew(switchToCallback, continuation, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }

            static void Callback(object state)
            {
                var continuation = (Action)state;
                continuation();
            }
        }
    }
}

#endif