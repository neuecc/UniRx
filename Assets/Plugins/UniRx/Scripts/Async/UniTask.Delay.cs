#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask Yield(PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UniTask(new YieldPromise(timing, cancellationToken));
        }

        public static UniTask Delay(int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Delay(TimeSpan.FromMilliseconds(millisecondsDelay), ignoreTimeScale, delayTiming, cancellationToken);
        }

        public static UniTask Delay(TimeSpan delayTimeSpan, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (delayTimeSpan < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayTimeSpan:" + delayTimeSpan);
            }

            if (ignoreTimeScale)
            {
                var source = new DelayIgnoreTimeScalePromise(delayTimeSpan, delayTiming, cancellationToken);
                return source.Task;
            }
            else
            {
                var source = new DelayPromise(delayTimeSpan, delayTiming, cancellationToken);
                return source.Task;
            }
        }

        public static UniTask<int> DelayFrame(int delayFrameCount, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (delayFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayFrameCount:" + delayFrameCount);
            }

            var source = new DelayFramePromise(delayFrameCount, delayTiming, cancellationToken);
            return source.Task;
        }

        // policy of playerloop cancellation.
        // does not raise cancel immediately, check only MoveNext timing(for performance).

        class YieldPromise : ReusablePromise, IPlayerLoopItem
        {
            readonly PlayerLoopTiming timing;
            CancellationToken cancellation;
            bool isRunning = false;

            public YieldPromise(PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.timing = timing;
                this.cancellation = cancellation;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }
                    return false;
                }
            }

            public bool MoveNext()
            {
                isRunning = false;

                if (cancellation.IsCancellationRequested)
                {
                    TrySetCanceled();
                }
                else
                {
                    TrySetResult();
                }

                return false;
            }

            public override void SetCancellationToken(CancellationToken token)
            {
                if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return;
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellation, token);
            }
        }

        class DelayFramePromise : ReusablePromise<int>, IPlayerLoopItem
        {
            readonly int delayFrameCount;
            readonly PlayerLoopTiming timing;
            CancellationToken cancellation;

            bool isRunning = false;
            int currentFrameCount;

            public DelayFramePromise(int delayFrameCount, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.delayFrameCount = delayFrameCount;
                this.cancellation = cancellation;
                this.timing = timing;
                this.currentFrameCount = 0;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        currentFrameCount = 0;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }
                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                if (currentFrameCount == delayFrameCount)
                {
                    isRunning = false;
                    TrySetResult(currentFrameCount);
                    return false;
                }

                currentFrameCount++;
                return true;
            }

            public override void SetCancellationToken(CancellationToken token)
            {
                if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return;
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellation, token);
            }
        }

        class DelayPromise : ReusablePromise, IPlayerLoopItem
        {
            readonly float delayFrameTimeSpan;
            readonly PlayerLoopTiming timing;
            float elapsed;
            CancellationToken cancellation;
            bool isRunning = false;

            public DelayPromise(TimeSpan delayFrameTimeSpan, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.delayFrameTimeSpan = (float)delayFrameTimeSpan.TotalSeconds;
                this.timing = timing;
                this.cancellation = cancellation;
                this.elapsed = 0.0f;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        this.elapsed = 0.0f;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }
                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                elapsed += Time.deltaTime;
                if (elapsed >= delayFrameTimeSpan)
                {
                    isRunning = false;
                    TrySetResult();
                    return false;
                }

                return true;
            }

            public override void SetCancellationToken(CancellationToken token)
            {
                if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return;
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellation, token);
            }
        }

        class DelayIgnoreTimeScalePromise : ReusablePromise, IPlayerLoopItem
        {
            readonly float delayFrameTimeSpan;
            readonly PlayerLoopTiming timing;
            float elapsed;
            CancellationToken cancellation;
            bool isRunning = false;

            public DelayIgnoreTimeScalePromise(TimeSpan delayFrameTimeSpan, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.delayFrameTimeSpan = (float)delayFrameTimeSpan.TotalSeconds;
                this.timing = timing;
                this.cancellation = cancellation;
                this.elapsed = 0.0f;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        ResetStatus();
                        this.elapsed = 0.0f;
                        PlayerLoopHelper.AddAction(timing, this);
                    }
                    return false;
                }
            }

            public override void GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                elapsed += Time.unscaledDeltaTime;

                if (elapsed >= delayFrameTimeSpan)
                {
                    isRunning = false;
                    TrySetResult();
                    return false;
                }

                return true;
            }

            public override void SetCancellationToken(CancellationToken token)
            {
                if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return;
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref cancellation, token);
            }
        }
    }
}
#endif