#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace UniRx.Async.Internal
{
    // 'public', user can use this(but be careful).
    public class ReusablePromise<T> : IAwaiter<T>
    {
        T result;
        object continuation; // Action or Queue<Action>

        public UniTask<T> Task => new UniTask<T>(this);

        public virtual bool IsCompleted => false;

        public virtual T GetResult()
        {
            return result;
        }

        void IAwaiter.GetResult()
        {
            GetResult();
        }

        public void TryInvokeContinuation(T result)
        {
            this.result = result;

            if (continuation == null) return;

            if (continuation is Action act)
            {
                continuation = null;
                act();
            }
            else
            {
                var q = (MinimumQueue<Action>)continuation;
                var size = q.Count;
                for (int i = 0; i < size; i++)
                {
                    q.Dequeue().Invoke();
                }
            }
        }

        public void OnCompleted(Action action)
        {
            UnsafeOnCompleted(action);
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
                    return;
                }
                else
                {
                    ((MinimumQueue<Action>)continuation).Enqueue(action);
                }
            }
        }
    }
}

#endif