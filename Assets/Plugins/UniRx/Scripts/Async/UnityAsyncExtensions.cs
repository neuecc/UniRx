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

        public static IAwaitable ToAwaitable(this AsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new AsyncOperationAwaitable(asyncOperation);
        }

        public static IAwaitable ConfigureAwait(this AsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            var awaiter = new AsyncOperationConfiguredAwaitable(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return awaiter;
        }

        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new ResourceRequestAwaiter(asyncOperation);
        }

        public static IAwaitable<UnityEngine.Object> ToAwaitable(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new ResourceRequestAwaitable(asyncOperation);
        }

        public static IAwaitable<UnityEngine.Object> ConfigureAwait(this ResourceRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            var awaiter = new ResourceRequestConfiguredAwaitable(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return awaiter;
        }

#if ENABLE_WWW

        public static IAwaiter GetAwaiter(this WWW www)
        {
            return ConfigureAwait(www).GetAwaiter();
        }

        public static IAwaitable ToAwaitable(this WWW www)
        {
            return ConfigureAwait(www);
        }

        public static IAwaitable ConfigureAwait(this WWW www, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (www == null) ThrowIfNull(nameof(www));
            var awaiter = new WWWConfiguredAwaitable(www, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return awaiter;
        }

#endif

#if ENABLE_UNITYWEBREQUEST

        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
        }

        public static IAwaitable<UnityWebRequest> ToAwaitable(this UnityWebRequestAsyncOperation asyncOperation)
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            return new UnityWebRequestAsyncOperationAwaitable(asyncOperation);
        }

        public static IAwaitable<UnityWebRequest> ConfigureAwait(this UnityWebRequestAsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            if (asyncOperation == null) ThrowIfNull(nameof(asyncOperation));
            var awaiter = new UnityWebRequestAsyncOperationConfiguredAwaitable(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return awaiter;
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

        class AsyncOperationAwaitable : IAwaitable, IAwaiter
        {
            readonly AsyncOperation asyncOperation;

            public AsyncOperationAwaitable(AsyncOperation asyncOperation)
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

            public IAwaiter GetAwaiter()
            {
                return this;
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

        class AsyncOperationConfiguredAwaitable : IAwaitable, IAwaiter, IPlayerLoopItem
        {
            readonly AsyncOperation asyncOperation;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public AsyncOperationConfiguredAwaitable(AsyncOperation asyncOperation, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.asyncOperation = asyncOperation;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public IAwaiter GetAwaiter()
            {
                return this;
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

        class ResourceRequestAwaitable : IAwaitable<UnityEngine.Object>, IAwaiter<UnityEngine.Object>
        {
            readonly ResourceRequest request;

            public ResourceRequestAwaitable(ResourceRequest request)
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

            public IAwaiter<UnityEngine.Object> GetAwaiter()
            {
                return this;
            }

            IAwaiter IAwaitable.GetAwaiter()
            {
                return this;
            }
        }

        class ResourceRequestConfiguredAwaitable : IAwaitable<UnityEngine.Object>, IAwaiter<UnityEngine.Object>, IPlayerLoopItem
        {
            readonly ResourceRequest request;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public ResourceRequestConfiguredAwaitable(ResourceRequest request, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.request = request;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            IAwaiter IAwaitable.GetAwaiter()
            {
                return GetAwaiter();
            }

            public IAwaiter<UnityEngine.Object> GetAwaiter()
            {
                return this;
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

        class WWWConfiguredAwaitable : IAwaitable, IAwaiter, IPlayerLoopItem
        {
            readonly WWW request;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public WWWConfiguredAwaitable(WWW request, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.request = request;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public IAwaiter GetAwaiter()
            {
                return this;
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

        class UnityWebRequestAsyncOperationAwaitable : IAwaitable<UnityWebRequest>, IAwaiter<UnityWebRequest>
        {
            readonly UnityWebRequestAsyncOperation asyncOperation;

            public UnityWebRequestAsyncOperationAwaitable(UnityWebRequestAsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
            }

            public IAwaiter<UnityWebRequest> GetAwaiter()
            {
                return this;
            }

            IAwaiter IAwaitable.GetAwaiter()
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

        class UnityWebRequestAsyncOperationConfiguredAwaitable : IAwaitable<UnityWebRequest>, IAwaiter<UnityWebRequest>, IPlayerLoopItem
        {
            readonly UnityWebRequestAsyncOperation request;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            Action continuation;

            public UnityWebRequestAsyncOperationConfiguredAwaitable(UnityWebRequestAsyncOperation request, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.request = request;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.continuation = null;
            }

            public IAwaiter<UnityWebRequest> GetAwaiter()
            {
                return this;
            }

            IAwaiter IAwaitable.GetAwaiter()
            {
                return this;
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