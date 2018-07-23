#if UNITY_2018_1_OR_NEWER && (NET_4_6 || NET_STANDARD_2_0 || CSHARP_7_OR_LATER) && !UNITY_WSA
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using Unity.Jobs;

namespace UniRx.Async
{
    public static partial class UnityAsyncExtensions
    {
        public static UniTask.Awaiter GetAwaiter(this JobHandle jobHandle)
        {
            return ToUniTask(jobHandle, CancellationToken.None).GetAwaiter();
        }

        public static UniTask ToUniTask(this JobHandle jobHandle, CancellationToken cancellation = default(CancellationToken))
        {
            var awaiter = new JobHandleAwaiter(jobHandle, cancellation);

            PlayerLoopHelper.AddAction(PlayerLoopTiming.EarlyUpdate, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.PreUpdate, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.PreLateUpdate, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.PostLateUpdate, awaiter);

            return new UniTask(awaiter);
        }

        public static UniTask ConfigureAwait(this JobHandle jobHandle, PlayerLoopTiming waitTiming, CancellationToken cancellation = default(CancellationToken))
        {
            var awaiter = new JobHandleAwaiter(jobHandle, cancellation);

            PlayerLoopHelper.AddAction(waitTiming, awaiter);

            return new UniTask(awaiter);
        }

        class JobHandleAwaiter : IAwaiter, IPlayerLoopItem
        {
            readonly JobHandle jobHandle;
            CancellationToken cancellationToken;
            Action continuation;
            AwaiterStatus status;
            bool calledComplete = false;
            bool registerFinishedAction = false;

            public JobHandleAwaiter(JobHandle jobHandle, CancellationToken cancellationToken)
            {
                this.jobHandle = jobHandle;
                this.cancellationToken = cancellationToken;
                this.status = AwaiterStatus.Pending;
                this.continuation = null;
            }

            public bool IsCompleted
            {
                get
                {
                    return false; // always async do.
                }
            }

            public AwaiterStatus Status => status;

            public void GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    status = AwaiterStatus.Canceled;
                    if (!calledComplete && !registerFinishedAction)
                    {
                        // Call jobHandle.Complete after finished.
                        registerFinishedAction = true;
                        PlayerLoopHelper.AddAction(PlayerLoopTiming.EarlyUpdate, new JobHandleAwaiter(jobHandle, CancellationToken.None));
                        this.continuation?.Invoke();
                    }

                    return false;
                }

                if (jobHandle.IsCompleted)
                {
                    if (!calledComplete)
                    {
                        status = AwaiterStatus.Succeeded;
                        calledComplete = true;
                        jobHandle.Complete();

                        this.continuation?.Invoke();
                    }

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