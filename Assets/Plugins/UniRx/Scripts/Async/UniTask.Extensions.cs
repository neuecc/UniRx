#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx.Async.Triggers;

namespace UniRx.Async
{
    public static class UniTaskExtensions
    {
        /// <summary>
        /// Convert UniTask -> UniTask[AsyncUnit].
        /// </summary>
        public static UniTask<AsyncUnit> AsAsyncUnitUniTask(this UniTask task)
        {
            // use implicit conversion
            return task;
        }

        /// <summary>
        /// Convert Task[T] -> UniTask[T].
        /// </summary>
        public static UniTask<T> AsUniTask<T>(this Task<T> task)
        {
            var promise = new UniTaskCompletionSource<T>();

            task.ContinueWith((x, state) =>
            {
                var p = (UniTaskCompletionSource<T>)state;

                switch (x.Status)
                {
                    case TaskStatus.Canceled:
                        p.TrySetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        p.TrySetException(x.Exception);
                        break;
                    case TaskStatus.RanToCompletion:
                        p.TrySetResult(x.Result);
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
            var promise = new UniTaskCompletionSource<AsyncUnit>();

            task.ContinueWith((x, state) =>
            {
                var p = (UniTaskCompletionSource<AsyncUnit>)state;

                switch (x.Status)
                {
                    case TaskStatus.Canceled:
                        p.TrySetCanceled();
                        break;
                    case TaskStatus.Faulted:
                        p.TrySetException(x.Exception);
                        break;
                    case TaskStatus.RanToCompletion:
                        p.TrySetResult(default(AsyncUnit));
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

        public static UniTask Timeout(this UniTask task, TimeSpan timeout, bool ignoreTimeScale = true, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            return Timeout(task.AsAsyncUnitUniTask(), timeout, ignoreTimeScale, timeoutCheckTiming, taskCancellationTokenSource);
        }

        public static async UniTask<T> Timeout<T>(this UniTask<T> task, TimeSpan timeout, bool ignoreTimeScale = true, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            // left, right both suppress operation canceled exception.

            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = (UniTask)UniTask.Delay(timeout, ignoreTimeScale, timeoutCheckTiming).WithIsCanceled(delayCancellationTokenSource.Token);

            var (hasValue, value) = await UniTask.WhenAny(task.SupressOperationCanceledException(), timeoutTask);

            if (!hasValue)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                }

                throw new TimeoutException("Exceed Timeout:" + timeout);
            }
            else
            {
                delayCancellationTokenSource.Cancel();
            }

            if (value.isCanceled)
            {
                throw new OperationCanceledException();
            }

            return value.value;
        }

        public static async UniTask<bool> TimeoutWithoutException(this UniTask task, TimeSpan timeout, bool ignoreTimeScale = true, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var v = await TimeoutWithoutException(task.AsAsyncUnitUniTask(), timeout, ignoreTimeScale, timeoutCheckTiming, taskCancellationTokenSource);
            return v.isTimeout;
        }

        public static async UniTask<(bool isTimeout, T value)> TimeoutWithoutException<T>(this UniTask<T> task, TimeSpan timeout, bool ignoreTimeScale = true, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
        {
            // left, right both suppress operation canceled exception.

            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = (UniTask)UniTask.Delay(timeout, ignoreTimeScale, timeoutCheckTiming).WithIsCanceled(delayCancellationTokenSource.Token);

            var (hasValue, value) = await UniTask.WhenAny(task.SupressOperationCanceledException(), timeoutTask);
            if (!hasValue)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                }

                return (true, default(T));
            }
            else
            {
                delayCancellationTokenSource.Cancel();
            }

            if (value.isCanceled)
            {
                throw new OperationCanceledException();
            }

            return (false, value.value);
        }

        public static void Forget(this UniTask task, Action<Exception> exceptionHandler = null)
        {
            ForgetCore(task, exceptionHandler).Forget();
        }

        static async UniTaskVoid ForgetCore(UniTask task, Action<Exception> exceptionHandler)
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
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        public static void Forget<T>(this UniTask<T> task, Action<Exception> exceptionHandler = null)
        {
            ForgetCore(task, exceptionHandler).Forget();
        }

        static async UniTaskVoid ForgetCore<T>(UniTask<T> task, Action<Exception> exceptionHandler)
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
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async UniTask ContinueWith<T>(this UniTask<T> task, Action<T> continuationFunction)
        {
            continuationFunction(await task);
        }

        public static async UniTask ContinueWith<T>(this UniTask<T> task, Func<T, UniTask> continuationFunction)
        {
            await continuationFunction(await task);
        }

        public static async UniTask<TR> ContinueWith<T, TR>(this UniTask<T> task, Func<T, TR> continuationFunction)
        {
            return continuationFunction(await task);
        }

        public static async UniTask<TR> ContinueWith<T, TR>(this UniTask<T> task, Func<T, UniTask<TR>> continuationFunction)
        {
            return await continuationFunction(await task);
        }

        public static async UniTask ContinueWith(this UniTask task, Action continuationFunction)
        {
            await task;
            continuationFunction();
        }

        public static async UniTask ContinueWith(this UniTask task, Func<UniTask> continuationFunction)
        {
            await task;
            await continuationFunction();
        }

        public static async UniTask<T> ContinueWith<T>(this UniTask task, Func<T> continuationFunction)
        {
            await task;
            return continuationFunction();
        }

        public static async UniTask<T> ContinueWith<T>(this UniTask task, Func<UniTask<T>> continuationFunction)
        {
            await task;
            return await continuationFunction();
        }

        public static async UniTask ConfigureAwait(this Task task, PlayerLoopTiming timing)
        {
            await task.ConfigureAwait(false);
            await UniTask.Yield(timing);
        }

        public static async UniTask<T> ConfigureAwait<T>(this Task<T> task, PlayerLoopTiming timing)
        {
            var v = await task.ConfigureAwait(false);
            await UniTask.Yield(timing);
            return v;
        }

        public static async UniTask ConfigureAwait(this UniTask task, PlayerLoopTiming timing)
        {
            await task;
            await UniTask.Yield(timing);
        }

        public static async UniTask<T> ConfigureAwait<T>(this UniTask<T> task, PlayerLoopTiming timing)
        {
            var v = await task;
            await UniTask.Yield(timing);
            return v;
        }

        public static async UniTask<T> Unwrap<T>(this UniTask<UniTask<T>> task)
        {
            return await await task;
        }

        public static async UniTask Unwrap<T>(this UniTask<UniTask> task)
        {
            await await task;
        }

        public static UniTask<bool> SupressOperationCanceledException(this UniTask task)
        {
            return task.WithIsCanceled(CancellationToken.None);
        }

        public static UniTask<(bool isCanceled, T value)> SupressOperationCanceledException<T>(this UniTask<T> task)
        {
            return task.WithIsCanceled(CancellationToken.None);
        }

        public static UniTask<bool> WithIsCanceled(this UniTask task, CancellationTokenSource cts)
        {
            return task.WithIsCanceled(cts.Token);
        }

        public static UniTask<(bool isCanceled, T value)> WithIsCanceled<T>(this UniTask<T> task, CancellationTokenSource cts)
        {
            return task.WithIsCanceled(cts.Token);
        }

        public static UniTask<bool> WithIsCanceled(this UniTask task, Component component)
        {
            return WithIsCanceled(task, component);
        }

        public static UniTask<bool> WithIsCanceled(this UniTask task, GameObject gameObject)
        {
            var token = gameObject.GetCancellationTokenOnDestroy();
            return task.WithIsCanceled(token);
        }

        // shortcut of WhenAll

        public static UniTask.Awaiter GetAwaiter<T>(this IEnumerable<UniTask> tasks)
        {
            return UniTask.WhenAll(tasks).GetAwaiter();
        }

        public static UniTask<T[]>.Awaiter GetAwaiter<T>(this IEnumerable<UniTask<T>> tasks)
        {
            return UniTask.WhenAll(tasks).GetAwaiter();
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