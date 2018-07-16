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

        public static UniTask<int> Delay(int delayFrameCount, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (delayFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayFrameCount:" + delayFrameCount);
            }

            var source = new DelayPromise(delayFrameCount, cancellationToken);
            PlayerLoopHelper.AddAction(delayTiming, source);
            return source.Task;
        }

        public static UniTask Delay(TimeSpan delayTimeSpan, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (delayTimeSpan < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayTimeSpan:" + delayTimeSpan);
            }

            var source = new DelayTimeSpanPromise(delayTimeSpan, cancellationToken);
            PlayerLoopHelper.AddAction(delayTiming, source);
            return source.Task;
        }

        class YieldPromise : Promise<AsyncUnit>, IPlayerLoopItem
        {
            CancellationToken cancellation;

            public UniTask Task => new UniTask(this);

            public YieldPromise(CancellationToken cancellation)
            {
                this.cancellation = cancellation;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                SetResult(AsyncUnit.Default);
                return false;
            }
        }

        class DelayPromise : Promise<int>, IPlayerLoopItem
        {
            readonly int delayFrameCount;
            CancellationToken cancellation;

            int currentFrameCount;

            public UniTask<int> Task => new UniTask<int>(this);

            public DelayPromise(int delayFrameCount, CancellationToken cancellation)
            {
                this.delayFrameCount = delayFrameCount;
                this.cancellation = cancellation;
                this.currentFrameCount = 0;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                if (currentFrameCount == delayFrameCount)
                {
                    SetResult(currentFrameCount);
                    return false;
                }

                currentFrameCount++;
                return true;
            }
        }

        class DelayTimeSpanPromise : Promise<AsyncUnit>, IPlayerLoopItem
        {
            readonly double delayFrameTimeSpan;
            CancellationToken cancellation;

            float initialTime;

            public UniTask Task => new UniTask(this);

            public DelayTimeSpanPromise(TimeSpan delayFrameTimeSpan, CancellationToken cancellation)
            {
                this.delayFrameTimeSpan = delayFrameTimeSpan.TotalSeconds;
                this.cancellation = cancellation;
                this.initialTime = Time.realtimeSinceStartup;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                var diff = Time.realtimeSinceStartup - initialTime;

                if (diff >= delayFrameTimeSpan)
                {
                    SetResult(default(AsyncUnit));
                    return false;
                }

                return true;
            }
        }
    }
}
#endif