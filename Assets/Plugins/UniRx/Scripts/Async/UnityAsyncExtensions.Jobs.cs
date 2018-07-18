#if UNITY_2018_1_OR_NEWER && (NET_4_6 || NET_STANDARD_2_0 || CSHARP_7_OR_LATER)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Jobs;

namespace UniRx.Async
{
    public static partial class UnityAsyncExtensions
    {
        public static IAwaiter GetAwaiter(this JobHandle jobHandle)
        {
            return ToAwaitable(jobHandle, CancellationToken.None).GetAwaiter();
        }

        public static IAwaitable ToAwaitable(this JobHandle jobHandle, CancellationToken cancellation = default(CancellationToken))
        {
            var awaiter = new JobHandleAwaitable(jobHandle, cancellation);

            PlayerLoopHelper.AddAction(PlayerLoopTiming.EarlyUpdate, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.PreUpdate, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.PreLateUpdate, awaiter);
            PlayerLoopHelper.AddAction(PlayerLoopTiming.PostLateUpdate, awaiter);

            return awaiter;
        }

        public static IAwaitable ConfigureAwait(this JobHandle jobHandle, PlayerLoopTiming waitTiming, CancellationToken cancellation = default(CancellationToken))
        {
            var awaiter = new JobHandleAwaitable(jobHandle, cancellation);

            PlayerLoopHelper.AddAction(waitTiming, awaiter);

            return awaiter;
        }

        class JobHandleAwaitable : IAwaitable, IAwaiter, IPlayerLoopItem
        {
            readonly JobHandle jobHandle;
            CancellationToken cancellationToken;
            Action continuation;
            bool calledComplete = false;

            public JobHandleAwaitable(JobHandle jobHandle, CancellationToken cancellationToken)
            {
                this.jobHandle = jobHandle;
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
                    return false; // always async do.
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
                    // Call jobHandle.Complete after finished.
                    if (!calledComplete)
                    {
                        PlayerLoopHelper.AddAction(PlayerLoopTiming.EarlyUpdate, new JobHandleAwaitable(jobHandle, CancellationToken.None));
                        this.continuation?.Invoke();
                    }

                    return false;
                }

                if (jobHandle.IsCompleted)
                {
                    if (!calledComplete)
                    {
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