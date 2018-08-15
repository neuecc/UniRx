#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncDestroyTrigger : MonoBehaviour
    {
        bool called = false;
        UniTaskCompletionSource promise;
        CancellationTokenSource cancellationTokenSource; // main cancellation
        object canellationTokenSourceOrQueue;            // external from AddCancellationTriggerOnDestory

        public CancellationToken CancellationToken
        {
            get
            {
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                }
                return cancellationTokenSource.Token;
            }
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        void OnDestroy()
        {
            called = true;
            promise?.TrySetResult();
            cancellationTokenSource?.Cancel();
            if (canellationTokenSourceOrQueue != null)
            {
                if (canellationTokenSourceOrQueue is CancellationTokenSource cts)
                {
                    cts.Cancel();
                }
                else
                {
                    var q = (MinimumQueue<CancellationTokenSource>)canellationTokenSourceOrQueue;
                    while (q.Count != 0)
                    {
                        q.Dequeue().Cancel();
                    }
                }
                canellationTokenSourceOrQueue = null;
            }
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public UniTask OnDestroyAsync()
        {
            if (called) return UniTask.CompletedTask;
            return new UniTask(promise ?? (promise = new UniTaskCompletionSource()));
        }

        /// <summary>Add Cancellation Triggers on destroy</summary>
        public void AddCancellationTriggerOnDestory(CancellationTokenSource cts)
        {
            if (called) cts.Cancel();

            if (canellationTokenSourceOrQueue == null)
            {
                canellationTokenSourceOrQueue = cts;
            }
            else if (canellationTokenSourceOrQueue is CancellationTokenSource c)
            {
                var q = new MinimumQueue<CancellationTokenSource>(4);
                q.Enqueue(c);
                q.Enqueue(cts);
                canellationTokenSourceOrQueue = q;
            }
            else
            {
                ((MinimumQueue<CancellationTokenSource>)canellationTokenSourceOrQueue).Enqueue(cts);
            }
        }
    }
}

#endif