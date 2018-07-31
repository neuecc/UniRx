#if (CSHARP_7_OR_LATER)

#pragma warning disable CS1591

using System;
using System.Threading;
using UniRx.Async;
using UniRx.Async.Internal;

namespace UniRx
{
    internal class ReactivePropertyReusablePromise<T> : IAwaiter<T>
    {
        T result;
        object continuation; // Action or Queue<Action>
        MinimumQueue<(int, T)> queueValues;
        bool running;
        int waitingContinuationCount;
        AwaiterStatus status;

        public bool IsCompleted => false;
        public UniTask<T> Task => new UniTask<T>(this);

        public AwaiterStatus Status => status;

        public T GetResult()
        {
            return result;
        }

        void IAwaiter.GetResult()
        {

        }

        public void InvokeContinuation(ref T value)
        {
            if (continuation == null) return;

            if (continuation is Action act)
            {
                this.result = value;
                status = AwaiterStatus.Succeeded;
                continuation = null;
                act();
                status = AwaiterStatus.Pending;
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
                    status = AwaiterStatus.Succeeded;
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
                        status = AwaiterStatus.Pending;
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

        public void SetCancellationToken(CancellationToken token)
        {
            // TODO:...
            throw new NotImplementedException();
        }
    }
}

#endif