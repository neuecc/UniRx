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
        public static UniTask AsUniTask(this Task task)
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

        public static IEnumerator ToCoroutine<T>(this UniTask<T> task, Action<T> resultHandler = null, Action<Exception> exceptionHandler = null)
        {
            return new ToCoroutineEnumerator<T>(task, resultHandler, exceptionHandler);
        }

        public static IEnumerator ToCoroutine(this UniTask task, Action<Exception> exceptionHandler = null)
        {
            return new ToCoroutineEnumerator(task, exceptionHandler);
        }

        public static async UniTask<T> Timeout<T>(this UniTask<T> task, TimeSpan timeout, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = UniTask.Delay(timeout, cancellationToken: delayCancellationTokenSource.Token);

            var (hasValue, value) = await UniTask.WhenAny(task, timeoutTask);
            if (!hasValue)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                }

                throw new TimeoutException();
            }
            else
            {
                delayCancellationTokenSource.Cancel();
            }

            return value;
        }

        class ToCoroutineEnumerator : IEnumerator
        {
            bool completed;
            UniTask task;
            Action<Exception> exceptionHandler = null;
            bool isStarted = false;

            public ToCoroutineEnumerator(UniTask task, Action<Exception> exceptionHandler)
            {
                completed = false;
                this.exceptionHandler = exceptionHandler;
                this.task = task;
            }

            async UniTaskVoid RunTask(UniTask task)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    completed = true;
                }
            }

            public object Current => null;

            public bool MoveNext()
            {
                if (!isStarted)
                {
                    isStarted = true;
                    RunTask(task).Forget();
                }

                return !completed;
            }

            public void Reset()
            {
            }
        }

        class ToCoroutineEnumerator<T> : IEnumerator
        {
            bool completed;
            Action<T> resultHandler = null;
            Action<Exception> exceptionHandler = null;
            bool isStarted = false;
            UniTask<T> task;
            object current = null;

            public ToCoroutineEnumerator(UniTask<T> task, Action<T> resultHandler, Action<Exception> exceptionHandler)
            {
                completed = false;
                this.task = task;
                this.resultHandler = resultHandler;
                this.exceptionHandler = exceptionHandler;
            }

            async UniTaskVoid RunTask(UniTask<T> task)
            {
                try
                {
                    var value = await task;
                    current = value;
                    if (resultHandler != null)
                    {
                        resultHandler(value);
                    }
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    completed = true;
                }
            }

            public object Current => current;

            public bool MoveNext()
            {
                if (isStarted)
                {
                    isStarted = true;
                    RunTask(task).Forget();
                }

                return !completed;
            }

            public void Reset()
            {
            }
        }

    }
}
#endif