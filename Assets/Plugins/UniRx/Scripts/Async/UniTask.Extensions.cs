#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace UniRx.Async
{
    public static class UniTaskExtensions
    {
        /// <summary>
        /// Convert UniTask -> UniTask[AsyncUnit].
        /// </summary>
        public static async UniTask<AsyncUnit> AsAsyncUnitUniTask(this UniTask task)
        {
            await task;
            return AsyncUnit.Default;
        }

        /// <summary>
        /// Convert UniTask[T] -> UniTask.
        /// </summary>
        public static async UniTask AsUniTask<T>(this UniTask<T> task)
        {
            await task;
        }

        /// <summary>
        /// Convert Task[T] -> UniTask[T].
        /// </summary>
        public static UniTask<T> AsUniTask<T>(this Task<T> task)
        {
            var promise = new Promise<T>();

            task.ContinueWith((x, state) =>
            {
                var p = (Promise<T>)state;

                switch (x.Status)
                {
                    case TaskStatus.Canceled:
                        p.SetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        p.SetException(x.Exception);
                        break;
                    case TaskStatus.RanToCompletion:
                        p.SetResult(x.Result);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }, promise);

            return new UniTask<T>(promise);
        }

        /// <summary>
        /// Convert Task -> UniTask.
        /// </summary>
        public static UniTask AsUniTask<T>(this Task task)
        {
            var promise = new Promise<AsyncUnit>();

            task.ContinueWith((x, state) =>
            {
                var p = (Promise<AsyncUnit>)state;

                switch (x.Status)
                {
                    case TaskStatus.Canceled:
                        p.SetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        p.SetException(x.Exception);
                        break;
                    case TaskStatus.RanToCompletion:
                        p.SetResult(default(AsyncUnit));
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }, promise);

            return new UniTask(promise);
        }

        public static IEnumerator ToCoroutine(this UniTask task)
        {
            return new ToCoroutineEnumerator(task);
        }

        public static async UniTask<T> Timeout<T>(this UniTask<T> task, TimeSpan timeout, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            var timeoutTask = UniTask.Delay(timeout, cancellationToken: cancellationTokenSource.Token);

            var (hasValue, value) = await UniTask.WhenAny(task, timeoutTask);
            if (!hasValue)
            {
                throw new TimeoutException();
            }

            cancellationTokenSource.Cancel();
            return value;
        }

        class ToCoroutineEnumerator : IEnumerator
        {
            bool completed;

            public ToCoroutineEnumerator(UniTask task)
            {
                completed = false;
                RunTask(task).Forget();
            }

            async UniTaskVoid RunTask(UniTask task)
            {
                try
                {
                    await task;
                }
                finally
                {
                    completed = true;
                }
            }

            public object Current => null;

            public bool MoveNext()
            {
                return !completed;
            }

            public void Reset()
            {
            }
        }
    }
}
#endif