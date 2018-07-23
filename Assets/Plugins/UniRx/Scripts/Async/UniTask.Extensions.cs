#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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

        public static UniTask Timeout(this UniTask task, TimeSpan timeout, CancellationTokenSource taskCancellationTokenSource = null)
        {
            return Timeout(task.AsAsyncUnitUniTask(), timeout, taskCancellationTokenSource);
        }

        public static async UniTask<T> Timeout<T>(this UniTask<T> task, TimeSpan timeout, CancellationTokenSource taskCancellationTokenSource = null)
        {
            // left, right both suppress operation canceled exception.

            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = (UniTask)UniTask.Delay(timeout, cancellationToken: delayCancellationTokenSource.Token).WithIsCanceled();

            var (hasValue, value) = await UniTask.WhenAny(task.WithIsCanceled(), timeoutTask);
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

        public static async UniTask<bool> TimeoutWithoutException(this UniTask task, TimeSpan timeout, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var v = await TimeoutWithoutException(task.AsAsyncUnitUniTask(), timeout, taskCancellationTokenSource);
            return v.isTimeout;
        }

        public static async UniTask<(bool isTimeout, T value)> TimeoutWithoutException<T>(this UniTask<T> task, TimeSpan timeout, CancellationTokenSource taskCancellationTokenSource = null)
        {
            // left, right both suppress operation canceled exception.

            var delayCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = (UniTask)UniTask.Delay(timeout, cancellationToken: delayCancellationTokenSource.Token).WithIsCanceled();

            var (hasValue, value) = await UniTask.WhenAny(task.WithIsCanceled(), timeoutTask);
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

        public static UniTask WithCancellation(this UniTask task, CancellationToken cancellationToken, CancellationTokenSource taskCancellationTokenSource = null)
        {
            return WithCancellation(task.AsAsyncUnitUniTask(), cancellationToken, taskCancellationTokenSource);
        }

        public static async UniTask<T> WithCancellation<T>(this UniTask<T> task, CancellationToken cancellationToken, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var (cancellationTask, registration) = cancellationToken.ToUniTask();

            var (hasValue, value) = await UniTask.WhenAny(task.WithIsCanceled(), cancellationTask);
            if (!hasValue)
            {
                if (taskCancellationTokenSource != null)
                {
                    taskCancellationTokenSource.Cancel();
                }

                throw new OperationCanceledException(cancellationToken);
            }
            else
            {
                registration.Dispose();
            }

            if (value.isCanceled)
            {
                throw new OperationCanceledException();
            }

            return value.value;
        }

        public static async UniTask<bool> WithCancellationWithoutException(this UniTask task, CancellationToken cancellationToken, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var v = await WithCancellationWithoutException(task.AsAsyncUnitUniTask(), cancellationToken, taskCancellationTokenSource);
            return v.isCanceled;
        }

        public static async UniTask<(bool isCanceled, T value)> WithCancellationWithoutException<T>(this UniTask<T> task, CancellationToken cancellationToken, CancellationTokenSource taskCancellationTokenSource = null)
        {
            var (cancellationTask, registration) = cancellationToken.ToUniTask();

            var (hasValue, value) = await UniTask.WhenAny(task.WithIsCanceled(), cancellationTask);
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
                registration.Dispose();
            }

            if (value.isCanceled)
            {
                return (true, default(T));
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

        /// <summary>
        /// Suppress throw OperationCanceledException when Task is canceled for optimize performance.
        /// </summary>
        public static async UniTask<bool> WithIsCanceled(this UniTask task)
        {
            var exceptionlessTask = new ExceptionlessUniTask(task);
            await exceptionlessTask;
            if (exceptionlessTask.Status == AwaiterStatus.Canceled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Suppress throw OperationCanceledException when Task is canceled for optimize performance.
        /// </summary>
        public static async UniTask<(bool isCanceled, T value)> WithIsCanceled<T>(this UniTask<T> task)
        {
            var exceptionlessTask = new ExceptionlessUniTask<T>(task);
            var result = await exceptionlessTask;
            if (exceptionlessTask.Status == AwaiterStatus.Canceled)
            {
                return (true, default(T));
            }
            else
            {
                return (false, exceptionlessTask.Result);
            }
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

        // Exceptionless(used by WithIsCanceled)

        struct ExceptionlessUniTask
        {
            readonly UniTask task;

            [DebuggerHidden]
            public ExceptionlessUniTask(UniTask task)
            {
                this.task = task;
            }

            [DebuggerHidden]
            public AwaiterStatus Status => task.Status;

            [DebuggerHidden]
            public bool IsCompleted => task.IsCompleted;

            [DebuggerHidden]
            public void GetResult() => task.GetResult();

            [DebuggerHidden]
            public Awaiter GetAwaiter()
            {
                return new Awaiter(task.GetAwaiter());
            }

            public struct Awaiter : IAwaiter
            {
                readonly UniTask.Awaiter awaiter;

                [DebuggerHidden]
                public Awaiter(UniTask.Awaiter awaiter)
                {
                    this.awaiter = awaiter;
                }

                [DebuggerHidden]
                public bool IsCompleted => awaiter.IsCompleted;

                [DebuggerHidden]
                public AwaiterStatus Status => awaiter.Status;

                [DebuggerHidden]
                public void GetResult()
                {
                    if (awaiter.Status == AwaiterStatus.Succeeded)
                    {
                        awaiter.GetResult();
                    }

                    // don't throw.
                }

                [DebuggerHidden]
                public void OnCompleted(Action continuation)
                {
                    awaiter.OnCompleted(continuation);
                }

                [DebuggerHidden]
                public void UnsafeOnCompleted(Action continuation)
                {
                    awaiter.UnsafeOnCompleted(continuation);
                }
            }
        }

        struct ExceptionlessUniTask<T>
        {
            readonly UniTask<T> task;

            [DebuggerHidden]
            public ExceptionlessUniTask(UniTask<T> task)
            {
                this.task = task;
            }

            [DebuggerHidden]
            public AwaiterStatus Status => task.Status;

            [DebuggerHidden]
            public bool IsCompleted => task.IsCompleted;

            [DebuggerHidden]
            public T Result => task.Result;

            [DebuggerHidden]
            public Awaiter GetAwaiter()
            {
                return new Awaiter(task.GetAwaiter());
            }

            public struct Awaiter : IAwaiter<T>
            {
                readonly UniTask<T>.Awaiter awaiter;

                [DebuggerHidden]
                public Awaiter(UniTask<T>.Awaiter awaiter)
                {
                    this.awaiter = awaiter;
                }

                [DebuggerHidden]
                public bool IsCompleted => awaiter.IsCompleted;

                [DebuggerHidden]
                public AwaiterStatus Status => awaiter.Status;

                [DebuggerHidden]
                void IAwaiter.GetResult() => GetResult();

                [DebuggerHidden]
                public T GetResult()
                {
                    if (awaiter.Status == AwaiterStatus.Succeeded)
                    {
                        return awaiter.GetResult();
                    }

                    // don't throw.
                    return default(T);
                }

                [DebuggerHidden]
                public void OnCompleted(Action continuation)
                {
                    awaiter.OnCompleted(continuation);
                }

                [DebuggerHidden]
                public void UnsafeOnCompleted(Action continuation)
                {
                    awaiter.UnsafeOnCompleted(continuation);
                }
            }
        }
    }
}

#endif