#if (CSHARP_7_OR_LATER)

#pragma warning disable CS1591

using System;
using System.Threading;
using UniRx.Async;
using UniRx.Async.Internal;

namespace UniRx
{
    internal class ReactivePropertyReusablePromise<T> : IAwaiter<T>, IResolvePromise<T>
    {
        T result;
        object continuation; // Action or Queue<Action>
        MinimumQueue<(int, T)> queueValues;
        bool running;
        int waitingContinuationCount;
        AwaiterStatus status;

        internal readonly CancellationToken RegisteredCancelationToken;

        public bool IsCompleted => status.IsCompleted();
        public UniTask<T> Task => new UniTask<T>(this);
        public AwaiterStatus Status => status;

        public ReactivePropertyReusablePromise(CancellationToken cancellationToken)
        {
            this.RegisteredCancelationToken = cancellationToken;
            this.status = AwaiterStatus.Pending;

            TaskTracker.TrackActiveTask(this, 3);
        }

        public T GetResult()
        {
            if (status == AwaiterStatus.Canceled) throw new OperationCanceledException();
            return result;
        }

        void IAwaiter.GetResult()
        {
            GetResult();
        }

        public void SetCanceled()
        {
            status = AwaiterStatus.Canceled;
            // run rest continuation.
            TaskTracker.RemoveTracking(this);

            result = default(T);
            InvokeContinuation(ref result);
            // clear
            continuation = null;
            queueValues = null;
        }

        public void InvokeContinuation(ref T value)
        {
            if (continuation == null) return;

            if (continuation is Action act)
            {
                this.result = value;
                continuation = null;
                act();
            }
            else
            {
                if (waitingContinuationCount == 0) return;

                var q = (MinimumQueue<Action>)continuation;
                if (queueValues == null) queueValues = new MinimumQueue<(int, T)>(4);
                queueValues.Enqueue((waitingContinuationCount, value));
                waitingContinuationCount = 0;

                if (!running)
                {
                    running = true;
                    try
                    {
                        while (queueValues.Count != 0)
                        {
                            var (runCount, v) = queueValues.Dequeue();
                            this.result = v;
                            for (int i = 0; i < runCount; i++)
                            {
                                q.Dequeue().Invoke();
                            }
                        }
                    }
                    finally
                    {
                        running = false;
                    }
                }
            }
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action action)
        {
            if (continuation == null)
            {
                continuation = action;
                return;
            }
            else
            {
                if (continuation is Action act)
                {
                    var q = new MinimumQueue<Action>(4);
                    q.Enqueue(act);
                    q.Enqueue(action);
                    continuation = q;
                    waitingContinuationCount = 2;
                    return;
                }
                else
                {
                    ((MinimumQueue<Action>)continuation).Enqueue(action);
                    waitingContinuationCount++;
                }
            }
        }

        bool IResolvePromise<T>.TrySetResult(T value)
        {
            InvokeContinuation(ref value);
            return true;
        }
    }
}

#endif