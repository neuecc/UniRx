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
        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation asyncOperation)
        {
            Guard.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));

            return new AsyncOperationAwaiter(asyncOperation);
        }

        public static UniTask ToUniTask(this AsyncOperation asyncOperation)
        {
            return new UniTask(GetAwaiter(asyncOperation));
        }

        public static UniTask ConfigureAwait(this AsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            Guard.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));

            var awaiter = new AsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
            if (!awaiter.IsCompleted)
            {
                PlayerLoopHelper.AddAction(timing, awaiter);
            }
            return new UniTask(awaiter);
        }

        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest resourceRequest)
        {
            Guard.ThrowArgumentNullException(resourceRequest, nameof(resourceRequest));

            return new ResourceRequestAwaiter(resourceRequest);
        }

        public static UniTask<UnityEngine.Object> ToUniTask(this ResourceRequest resourceRequest)
        {
            Guard.ThrowArgumentNullException(resourceRequest, nameof(resourceRequest));

            return new UniTask<UnityEngine.Object>(new ResourceRequestAwaiter(resourceRequest));
        }

        public static UniTask<UnityEngine.Object> ConfigureAwait(this ResourceRequest resourceRequest, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            Guard.ThrowArgumentNullException(resourceRequest, nameof(resourceRequest));

            var awaiter = new ResourceRequestConfiguredAwaiter(resourceRequest, progress, cancellation);
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
            Guard.ThrowArgumentNullException(www, nameof(www));

            var awaiter = new WWWConfiguredAwaiter(www, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return new UniTask(awaiter);
        }

#endif

#if ENABLE_UNITYWEBREQUEST

        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            Guard.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));

            return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
        }

        public static UniTask<UnityWebRequest> ToUniTask(this UnityWebRequestAsyncOperation asyncOperation)
        {
            return new UniTask<UnityWebRequest>(GetAwaiter(asyncOperation));
        }

        public static UniTask<UnityWebRequest> ConfigureAwait(this UnityWebRequestAsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellation = default(CancellationToken))
        {
            Guard.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));

            var awaiter = new UnityWebRequestAsyncOperationConfiguredAwaiter(asyncOperation, progress, cancellation);
            PlayerLoopHelper.AddAction(timing, awaiter);
            return new UniTask<UnityWebRequest>(awaiter);
        }

#endif

        public struct AsyncOperationAwaiter : IAwaiter
        {
            readonly AsyncOperation asyncOperation;
            AwaiterStatus status;

            public AsyncOperationAwaiter(AsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.status = asyncOperation.isDone ? AwaiterStatus.Succeeded : AwaiterStatus.Pending;
            }

            public bool IsCompleted => status.IsCompleted();
            public AwaiterStatus Status => status;

            public void GetResult()
            {
                if (status == AwaiterStatus.Pending)
                {
                    // first timing of call
                    if (asyncOperation.isDone)
                    {
                        status = AwaiterStatus.Succeeded;
                    }
                    else
                    {
                        throw new InvalidOperationException("Not yet completed.");
                    }
                }
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
            AwaiterStatus status;
            Action continuation;

            public AsyncOperationConfiguredAwaiter(AsyncOperation asyncOperation, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.asyncOperation = asyncOperation;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.status = cancellationToken.IsCancellationRequested ? AwaiterStatus.Canceled
                            : asyncOperation.isDone ? AwaiterStatus.Succeeded
                            : AwaiterStatus.Pending;
                this.continuation = null;

                if (this.status != AwaiterStatus.Canceled)
                {
                    TaskTracker.TrackActiveTask(this);
                }
            }

            public bool IsCompleted => status.IsCompleted();
            public AwaiterStatus Status => status;

            public void GetResult()
            {
                if (status == AwaiterStatus.Pending)
                {
                    throw new InvalidOperationException("Not yet completed.");
                }
                else if (status == AwaiterStatus.Canceled)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    TaskTracker.RemoveTracking(this);
                    status = AwaiterStatus.Canceled;
                    this.continuation?.Invoke();
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
                {
                    TaskTracker.RemoveTracking(this);
                    status = AwaiterStatus.Succeeded;
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
            readonly ResourceRequest asyncOperation;
            AwaiterStatus status;

            public ResourceRequestAwaiter(ResourceRequest asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.status = asyncOperation.isDone ? AwaiterStatus.Succeeded : AwaiterStatus.Pending;
            }

            public bool IsCompleted => status.IsCompleted();
            public AwaiterStatus Status => status;

            public UnityEngine.Object GetResult()
            {
                if (status == AwaiterStatus.Pending)
                {
                    // first timing of call
                    if (asyncOperation.isDone)
                    {
                        status = AwaiterStatus.Succeeded;
                    }
                    else
                    {
                        throw new InvalidOperationException("Not yet completed.");
                    }
                }
                return asyncOperation.asset;
            }

            void IAwaiter.GetResult()
            {
                GetResult();
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

        class ResourceRequestConfiguredAwaiter : IAwaiter<UnityEngine.Object>, IPlayerLoopItem
        {
            readonly ResourceRequest asyncOperation;
            readonly IProgress<float> progress;
            CancellationToken cancellationToken;
            AwaiterStatus status;
            Action continuation;

            public ResourceRequestConfiguredAwaiter(ResourceRequest asyncOperation, IProgress<float> progress, CancellationToken cancellationToken)
            {
                this.asyncOperation = asyncOperation;
                this.progress = progress;
                this.cancellationToken = cancellationToken;
                this.status = cancellationToken.IsCancellationRequested ? AwaiterStatus.Canceled
                            : asyncOperation.isDone ? AwaiterStatus.Succeeded
                            : AwaiterStatus.Pending;
                this.continuation = null;

                if (this.status != AwaiterStatus.Canceled)
                {
                    TaskTracker.TrackActiveTask(this);
                }
            }

            public bool IsCompleted => status.IsCompleted();
            public AwaiterStatus Status => status;

            public UnityEngine.Object GetResult()
            {
                if (status == AwaiterStatus.Pending)
                {
                    throw new InvalidOperationException("Not yet completed.");
                }
                else if (status == AwaiterStatus.Canceled)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                return asyncOperation.asset;
            }

            void IAwaiter.GetResult()
            {
                GetResult();
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    TaskTracker.RemoveTracking(this);
                    status = AwaiterStatus.Canceled;
                    this.continuation?.Invoke();
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
                {
                    TaskTracker.RemoveTracking(this);
                    status = AwaiterStatus.Succeeded;
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

            public AwaiterStatus Status => request.isDone ? AwaiterStatus.Succeeded
                     : cancellationToken.IsCancellationRequested ? AwaiterStatus.Canceled
                     : AwaiterStatus.Pending;

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

            public void SetCancellationToken(CancellationToken token)
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellationToken, token);
            }
        }

#endif

#if ENABLE_UNITYWEBREQUEST

        public struct UnityWebRequestAsyncOperationAwaiter : IAwaiter<UnityWebRequest>
        {
            readonly UnityWebRequestAsyncOperation asyncOperation;
            CancellationToken cancellation;

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

            public AwaiterStatus Status => asyncOperation.isDone ? AwaiterStatus.Succeeded : AwaiterStatus.Pending;

            public UnityWebRequest GetResult()
            {
                return asyncOperation.webRequest;
            }

            void IAwaiter.GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
            }

            public void OnCompleted(Action continuation)
            {
                asyncOperation.completed += continuation.AsFuncOfT<AsyncOperation>();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                asyncOperation.completed += continuation.AsFuncOfT<AsyncOperation>();
            }

            void IAwaiter.SetCancellationToken(CancellationToken token)
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellation, token);
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

            public AwaiterStatus Status => request.isDone ? AwaiterStatus.Succeeded
                     : cancellationToken.IsCancellationRequested ? AwaiterStatus.Canceled
                     : AwaiterStatus.Pending;

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

            public void SetCancellationToken(CancellationToken token)
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellationToken, token);
            }
        }

#endif
    }
}
#endif