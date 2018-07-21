#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncUpdateTrigger : MonoBehaviour
    {
        ReusablePromise promise;

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        void Update()
        {
            promise?.TryInvokeContinuation();
        }

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        public UniTask UpdateAsync()
        {
            return new UniTask(promise ?? (promise = new ReusablePromise()));
        }
    }
}

#endif