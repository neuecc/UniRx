#if FALSE // currently unity.com.resourcemanagement does not append any compiler symbol. If you want to use, remove this symbol manually.

#if NET_4_6 || NET_STANDARD_2_0 || CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.ResourceManagement;
using System.Runtime.CompilerServices;
using UniRx.Async.Internal;
using System.Threading;

namespace UniRx.Async
{
    public static partial class UnityAsyncExtensions
    {
        public static InterfaceAsyncOperationAwaiter GetAwaiter(IAsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new InterfaceAsyncOperationAwaiter(asyncOperation);
        }

        public static InterfaceAsyncOperationConfiguredAwaiter ConfigureAwait(this IAsyncOperation asyncOperation, IProgress<float> progress = null, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new InterfaceAsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
        }

        public static InterfaceAsyncOperationAwaiter<T> GetAwaiter<T>(IAsyncOperation<T> asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new InterfaceAsyncOperationAwaiter<T>(asyncOperation);
        }

        public static InterfaceAsyncOperationConfiguredAwaiter<T> ConfigureAwait<T>(this IAsyncOperation<T> asyncOperation, IProgress<float> progress = null, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new InterfaceAsyncOperationConfiguredAwaiter<T>(asyncOperation, progress, cancellation);
        }

        public struct InterfaceAsyncOperationAwaiter : ICriticalNotifyCompletion
        {
            readonly IAsyncOperation asyncOperation;

            public InterfaceAsyncOperationAwaiter(IAsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.IsDone;
                }
            }

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                asyncOperation.Completed += continuation.AsFuncOfT<IAsyncOperation>();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                asyncOperation.Completed += continuation.AsFuncOfT<IAsyncOperation>();
            }
        }

        public struct InterfaceAsyncOperationConfiguredAwaiter : ICriticalNotifyCompletion, IPlayerLoopItem
        {
            readonly IAsyncOperation asyncOperation;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public InterfaceAsyncOperationConfiguredAwaiter(IAsyncOperation asyncOperation, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.asyncOperation = asyncOperation;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public InterfaceAsyncOperationConfiguredAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.IsDone;
                }
            }

            public void GetResult()
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.PercentComplete);
                }

                if (asyncOperation.IsDone)
                {
                    this.continuation?.Invoke();
                    return false;
                }

                return true;
            }

            public void OnCompleted(Action continuation)
            {
                this.continuation = continuation;
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                this.continuation = continuation;
            }
        }

        public struct InterfaceAsyncOperationAwaiter<T> : ICriticalNotifyCompletion
        {
            readonly IAsyncOperation<T> asyncOperation;

            public InterfaceAsyncOperationAwaiter(IAsyncOperation<T> asyncOperation)
            {
                this.asyncOperation = asyncOperation;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.IsDone;
                }
            }

            public T GetResult()
            {
                return asyncOperation.Result;
            }

            public void OnCompleted(Action continuation)
            {
                asyncOperation.Completed += continuation.AsFuncOfT<IAsyncOperation>();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                asyncOperation.Completed += continuation.AsFuncOfT<IAsyncOperation>();
            }
        }

        public struct InterfaceAsyncOperationConfiguredAwaiter<T> : ICriticalNotifyCompletion, IPlayerLoopItem
        {
            readonly IAsyncOperation<T> asyncOperation;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public InterfaceAsyncOperationConfiguredAwaiter(IAsyncOperation<T> asyncOperation, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.asyncOperation = asyncOperation;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public InterfaceAsyncOperationConfiguredAwaiter<T> GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.IsDone;
                }
            }

            public T GetResult()
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
                return asyncOperation.Result;
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.PercentComplete);
                }

                if (asyncOperation.IsDone)
                {
                    this.continuation?.Invoke();
                    return false;
                }

                return true;
            }

            public void OnCompleted(Action continuation)
            {
                this.continuation = continuation;
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                this.continuation = continuation;
            }
        }
    }
}
#endif
#endif