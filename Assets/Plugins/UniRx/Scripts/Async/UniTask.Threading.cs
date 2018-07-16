#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        static readonly WaitCallback switchToThreadPoolCallback = CallbackPromiseSetResult;
        static readonly Action<object> switchToTaskPoolCallback = CallbackPromiseSetResult;

        public static UniTask SwitchToThreadPool()
        {
            var promise = new Promise<AsyncUnit>();
            ThreadPool.UnsafeQueueUserWorkItem(switchToThreadPoolCallback, promise);
            return new UniTask(promise);
        }

        public static UniTask SwitchToTaskPool(CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new Promise<AsyncUnit>();
            Task.Factory.StartNew(switchToTaskPoolCallback, promise, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            return new UniTask(promise);
        }

        static void CallbackPromiseSetResult(object state)
        {
            var promise = (Promise<AsyncUnit>)state;
            promise.SetResult(AsyncUnit.Default);
        }
    }
}

#endif