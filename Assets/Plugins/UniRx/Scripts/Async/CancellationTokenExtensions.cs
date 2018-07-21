#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace UniRx.Async
{
    public static class CancellationTokenExtensions
    {
        static readonly Action<object> cancellationTokenCallback = Callback;

        public static (UniTask, CancellationTokenRegistration) ToUniTask(this CancellationToken cts)
        {
            if (cts.IsCancellationRequested)
            {
                return (UniTask.FromCanceled(cts), default(CancellationTokenRegistration));
            }

            var promise = new UniTaskCompletionSource<AsyncUnit>();
            return (promise.Task, cts.Register(cancellationTokenCallback, promise, false));
        }

        static void Callback(object state)
        {
            var promise = (UniTaskCompletionSource<AsyncUnit>)state;
            promise.TrySetResult(AsyncUnit.Default);
        }
    }
}

#endif