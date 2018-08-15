#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        static readonly UniTask CanceledUniTask = new Func<UniTask>(() =>
        {
            var promise = new UniTaskCompletionSource<AsyncUnit>();
            promise.TrySetCanceled();
            return new UniTask(promise);
        })();

        public static UniTask CompletedTask
        {
            get
            {
                return new UniTask();
            }
        }

        public static UniTask FromException(Exception ex)
        {
            var promise = new UniTaskCompletionSource<AsyncUnit>();
            promise.TrySetException(ex);
            return new UniTask(promise);
        }

        public static UniTask<T> FromException<T>(Exception ex)
        {
            var promise = new UniTaskCompletionSource<T>();
            promise.TrySetException(ex);
            return new UniTask<T>(promise);
        }

        public static UniTask<T> FromResult<T>(T value)
        {
            return new UniTask<T>(value);
        }

        public static UniTask FromCanceled()
        {
            return CanceledUniTask;
        }

        public static UniTask<T> FromCanceled<T>()
        {
            return CanceledUniTaskCache<T>.Task;
        }

        public static UniTask FromCanceled(CancellationToken token)
        {
            var promise = new UniTaskCompletionSource<AsyncUnit>();
            promise.TrySetException(new OperationCanceledException(token));
            return new UniTask(promise);
        }

        public static UniTask<T> FromCanceled<T>(CancellationToken token)
        {
            var promise = new UniTaskCompletionSource<T>();
            promise.TrySetException(new OperationCanceledException(token));
            return new UniTask<T>(promise);
        }

        static class CanceledUniTaskCache<T>
        {
            public static readonly UniTask<T> Task;

            static CanceledUniTaskCache()
            {
                var promise = new UniTaskCompletionSource<T>();
                promise.TrySetCanceled();
                Task = new UniTask<T>(promise);
            }
        }
    }

    internal static class CompletedTasks
    {
        public static readonly UniTask<bool> True = UniTask.FromResult(true);
        public static readonly UniTask<bool> False = UniTask.FromResult(false);
        public static readonly UniTask<int> Zero = UniTask.FromResult(0);
        public static readonly UniTask<int> MinusOne = UniTask.FromResult(-1);
        public static readonly UniTask<int> One = UniTask.FromResult(1);
    }
}
#endif