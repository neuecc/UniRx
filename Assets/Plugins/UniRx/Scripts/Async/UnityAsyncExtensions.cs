#if (NET_4_6 || NET_STANDARD_2_0) && CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
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

        public static UniTask ToUniTask(this AsyncOperation asyncOperation)
        {
            return new UniTask(GetAwaiter(asyncOperation));
        }

        public static UniTask ConfigureAwait(this AsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            var awaiter = new AsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return new UniTask(awaiter);
        }

        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new ResourceRequestAwaiter(asyncOperation);
        }

        public static UniTask<UnityEngine.Object> ToUniTask(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new UniTask<UnityEngine.Object>(new ResourceRequestAwaiter(asyncOperation));
        }

        public static UniTask<UnityEngine.Object> ConfigureAwait(this ResourceRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            var awaiter = new ResourceRequestConfiguredAwaiter(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return new UniTask<UnityEngine.Object>(awaiter);
        }

#if ENABLE_WWW

        public static UniTask.Awaiter GetAwaiter(this WWW www)
        {
            return ConfigureAwait(www).GetAwaiter();
        }

        public static UniTask ToUniTask(this WWW www)
        {
            return ConfigureAwait(www);
        }

        public static UniTask ConfigureAwait(this WWW www, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (www == null) ThrowIfNull(nameof(www));
            var awaiter = new WWWConfiguredAwaiter(www, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return new UniTask(awaiter);
        }

#endif

#if ENABLE_UNITYWEBREQUEST

        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
        }

        public static UniTask<UnityWebRequest> ToUniTask(this UnityWebRequestAsyncOperation asyncOperation)
        {
            return new UniTask<UnityWebRequest>(GetAwaiter(asyncOperation));
        }

        public static UniTask<UnityWebRequest> ConfigureAwait(this UnityWebRequestAsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            var awaiter = new UnityWebRequestAsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return new UniTask<UnityWebRequest>(awaiter);
        }

#endif

        public struct AsyncOperationAwaiter : IAwaiter
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

        class AsyncOperationConfiguredAwaiter : IAwaiter, IPlayerLoopItem
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

            public bool IsCompleted
            {
                get
                {
                    return cancellationToken.IsCancellationRequested || asyncOperation.isDone;
                }
            }

            public void GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    this.continuation?.Invoke();
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

        public struct ResourceRequestAwaiter : IAwaiter<UnityEngine.Object>
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

            void IAwaiter.GetResult()
            {
                // do nothing(no throw)
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

        class ResourceRequestConfiguredAwaiter : IAwaiter<UnityEngine.Object>, IPlayerLoopItem
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

            public bool IsCompleted
            {
                get
                {
                    return cancellationToken.IsCancellationRequested || request.isDone;
                }
            }

            public UnityEngine.Object GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
                return request.asset;
            }

            void IAwaiter.GetResult()
            {
                GetResult();
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    this.continuation?.Invoke();
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

#if ENABLE_WWW

        class WWWConfiguredAwaiter : IAwaiter, IPlayerLoopItem
        {
            readonly WWW request;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public WWWConfiguredAwaiter(WWW request, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.request = request;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public bool IsCompleted
            {
                get
                {
                    return cancellationToken.IsCancellationRequested || request.isDone;
                }
            }

            public void GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    this.continuation?.Invoke();
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

#if ENABLE_UNITYWEBREQUEST

        public struct UnityWebRequestAsyncOperationAwaiter : IAwaiter<UnityWebRequest>
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

            void IAwaiter.GetResult()
            {
                // do nothing(no throw)
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

        class UnityWebRequestAsyncOperationConfiguredAwaiter : IAwaiter<UnityWebRequest>, IPlayerLoopItem
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

            public bool IsCompleted
            {
                get
                {
                    return cancellationToken.IsCancellationRequested || request.isDone;
                }
            }

            public UnityWebRequest GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
                return request.webRequest;
            }

            void IAwaiter.GetResult()
            {
                GetResult();
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    this.continuation?.Invoke();
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