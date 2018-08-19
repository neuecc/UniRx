#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncAwakeTrigger : MonoBehaviour
    {
        bool called = false;
        UniTaskCompletionSource promise;

        void Awake()
        {
            called = true;
            promise?.TrySetResult();
        }

        public UniTask AwakeAsync()
        {
            if (called) return UniTask.CompletedTask;
            PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
            return new UniTask(promise ?? (promise = new UniTaskCompletionSource()));
        }

        private void OnDestroy()
        {
            promise?.TrySetCanceled();
        }

        class AwakeMonitor : IPlayerLoopItem
        {
            readonly AsyncAwakeTrigger trigger;

            public AwakeMonitor(AsyncAwakeTrigger trigger)
            {
                this.trigger = trigger;
            }

            public bool MoveNext()
            {
                if (trigger.called) return false;
                if (trigger == null)
                {
                    trigger.OnDestroy();
                    return false;
                }
                return true;
            }
        }
    }
}

#endif