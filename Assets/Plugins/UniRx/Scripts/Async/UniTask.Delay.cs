#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UnityEngine;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask Yield(PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var source = new YieldPromise(cancellationToken);
            PlayerLoopHelper.AddAction(timing, source);
            return source.Task;
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

        class YieldPromise : UniTaskCompletionSource<AsyncUnit>, IPlayerLoopItem
        {
            CancellationToken cancellation;

            public YieldPromise(CancellationToken cancellation)
            {
                this.cancellation = cancellation;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    TrySetCanceled();
                    return false;
                }

                TrySetResult(AsyncUnit.Default);
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