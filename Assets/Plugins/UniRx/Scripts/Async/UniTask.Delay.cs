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
                var source = new DelayIgnoreTimeScalePromise(delayTimeSpan, cancellationToken);
                PlayerLoopHelper.AddAction(delayTiming, source);
                return source.Task;
            }
            else
            {
                var source = new DelayPromise(delayTimeSpan, cancellationToken);
                PlayerLoopHelper.AddAction(delayTiming, source);
                return source.Task;
            }
        }

        public static UniTask<int> DelayFrame(int delayFrameCount, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (delayFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayFrameCount:" + delayFrameCount);
            }

            var source = new DelayFramePromise(delayFrameCount, cancellationToken);
            PlayerLoopHelper.AddAction(delayTiming, source);
            return source.Task;
        }

        class YieldPromise : ReusablePromise<AsyncUnit>, IPlayerLoopItem
        {
            PlayerLoopTiming timing;
            CancellationToken cancellation;

            public YieldPromise(PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.timing = timing;
                this.cancellation = cancellation;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (cancellation.IsCancellationRequested) return true;

                    PlayerLoopHelper.AddAction(timing, this);
                    return false;
                }
            }

            public override AsyncUnit GetResult()
            {
                cancellation.ThrowIfCancellationRequested();
                return base.GetResult();
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TryInvokeContinuation(AsyncUnit.Default);
                    return false;
                }

                TryInvokeContinuation(AsyncUnit.Default);
                return false;
            }
        }

        class DelayFramePromise : UniTaskCompletionSource<int>, IPlayerLoopItem
        {
            readonly int delayFrameCount;
            CancellationToken cancellation;

            int currentFrameCount;

            public DelayFramePromise(int delayFrameCount, CancellationToken cancellation)
            {
                this.delayFrameCount = delayFrameCount;
                this.cancellation = cancellation;
                this.currentFrameCount = 0;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TrySetCanceled();
                    return false;
                }

                if (currentFrameCount == delayFrameCount)
                {
                    TrySetResult(currentFrameCount);
                    return false;
                }

                currentFrameCount++;
                return true;
            }
        }

        class DelayPromise : UniTaskCompletionSource<AsyncUnit>, IPlayerLoopItem
        {
            readonly float delayFrameTimeSpan;
            float elapsed;
            CancellationToken cancellation;

            public DelayPromise(TimeSpan delayFrameTimeSpan, CancellationToken cancellation)
            {
                this.delayFrameTimeSpan = (float)delayFrameTimeSpan.TotalSeconds;
                this.cancellation = cancellation;
                this.elapsed = 0.0f;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TrySetCanceled();
                    return false;
                }

                elapsed += Time.deltaTime;

                if (elapsed >= delayFrameTimeSpan)
                {
                    TrySetResult(default(AsyncUnit));
                    return false;
                }

                return true;
            }
        }

        class DelayIgnoreTimeScalePromise : UniTaskCompletionSource<AsyncUnit>, IPlayerLoopItem
        {
            readonly float delayFrameTimeSpan;
            float elapsed;
            CancellationToken cancellation;

            public DelayIgnoreTimeScalePromise(TimeSpan delayFrameTimeSpan, CancellationToken cancellation)
            {
                this.delayFrameTimeSpan = (float)delayFrameTimeSpan.TotalSeconds;
                this.cancellation = cancellation;
                this.elapsed = 0.0f;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TrySetCanceled();
                    return false;
                }

                elapsed += Time.unscaledDeltaTime;

                if (elapsed >= delayFrameTimeSpan)
                {
                    TrySetResult(default(AsyncUnit));
                    return false;
                }

                return true;
            }
        }
    }
}
#endif