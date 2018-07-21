#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncDestroyTrigger : MonoBehaviour
    {
        bool called = false;
        UniTaskCompletionSource promise;

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        void OnDestroy()
        {
            called = true;
            promise?.TrySetResult();
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public UniTask OnDestroyAsync()
        {
            if (called) return UniTask.CompletedTask;
            return new UniTask(promise ?? (promise = new UniTaskCompletionSource()));
        }
    }
}

#endif