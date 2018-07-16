#if NET_4_6 || NET_STANDARD_2_0 || CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace UniRx.Async
{
    public static partial class UnityAsyncExtensions
    {
        static void ThrowIfNull(string name)
        {
            throw new ArgumentNullException(name);
        }

        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new AsyncOperationAwaiter(asyncOperation);
        }

        public static AsyncOperationConfiguredAwaiter ConfigureAwait(this AsyncOperation asyncOperation, IProgress<float> progress = null, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new AsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
        }

        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new ResourceRequestAwaiter(asyncOperation);
        }

        public static ResourceRequestConfiguredAwaiter ConfigureAwait(this ResourceRequest asyncOperation, IProgress<float> progress = null, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new ResourceRequestConfiguredAwaiter(asyncOperation, progress, cancellation);
        }

    #if ENABLE_UNITYWEBREQUEST

        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
        }

        public static UnityWebRequestAsyncOperationConfiguredAwaiter ConfigureAwait(this UnityWebRequestAsyncOperation asyncOperation, IProgress<float> progress = null, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new UnityWebRequestAsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
        }

    #endif

        public struct AsyncOperationAwaiter : ICriticalNotifyCompletion
        {
            readonly AsyncOperation asyncOperation;

            public AsyncOperationAwaiter(AsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.isDone;
                }
            }

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                asyncOperation.completed += continuation.AsFuncOfT<AsyncOperation>();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                asyncOperation.completed += continuation.AsFuncOfT<AsyncOperation>();
            }
        }

        public struct AsyncOperationConfiguredAwaiter : ICriticalNotifyCompletion, IPlayerLoopItem
        {
            readonly AsyncOperation asyncOperation;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public AsyncOperationConfiguredAwaiter(AsyncOperation asyncOperation, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.asyncOperation = asyncOperation;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public AsyncOperationConfiguredAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.isDone;
                }
            }

            public void GetResult()
            {
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
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

        public struct ResourceRequestAwaiter : ICriticalNotifyCompletion
        {
            readonly ResourceRequest request;

            public ResourceRequestAwaiter(ResourceRequest request)
            {
                this.request = request;
            }

            public bool IsCompleted
            {
                get
                {
                    return request.isDone;
                }
            }

            public UnityEngine.Object GetResult()
            {
                return request.asset;
            }

            public void OnCompleted(Action continuation)
            {
                request.completed += continuation.AsFuncOfT<AsyncOperation>();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                request.completed += continuation.AsFuncOfT<AsyncOperation>();
            }
        }

        public struct ResourceRequestConfiguredAwaiter : ICriticalNotifyCompletion, IPlayerLoopItem
        {
            readonly ResourceRequest request;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public ResourceRequestConfiguredAwaiter(ResourceRequest request, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.request = request;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public ResourceRequestConfiguredAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get
                {
                    return request.isDone;
                }
            }

            public UnityEngine.Object GetResult()
            {
                return request.asset;
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(request.progress);
                }

                if (request.isDone)
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

    #if ENABLE_UNITYWEBREQUEST

        public struct UnityWebRequestAsyncOperationAwaiter : ICriticalNotifyCompletion
        {
            readonly UnityWebRequestAsyncOperation asyncOperation;

            public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
            }

            public bool IsCompleted
            {
                get
                {
                    return asyncOperation.isDone;
                }
            }

            public UnityWebRequest GetResult()
            {
                return asyncOperation.webRequest;
            }

            public void OnCompleted(Action continuation)
            {
                asyncOperation.completed += continuation.AsFuncOfT<AsyncOperation>();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                asyncOperation.completed += continuation.AsFuncOfT<AsyncOperation>();
            }
        }

        public struct UnityWebRequestAsyncOperationConfiguredAwaiter : ICriticalNotifyCompletion, IPlayerLoopItem
        {
            readonly UnityWebRequestAsyncOperation request;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public UnityWebRequestAsyncOperationConfiguredAwaiter(UnityWebRequestAsyncOperation request, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.request = request;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public UnityWebRequestAsyncOperationConfiguredAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted
            {
                get
                {
                    return request.isDone;
                }
            }

            public UnityWebRequest GetResult()
            {
                return request.webRequest;
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(request.progress);
                }

                if (request.isDone)
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


    #endif
    }
}
#endif