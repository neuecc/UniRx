#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncFixedUpdateTrigger : MonoBehaviour
    {
        ReusablePromise fixedUpdate;

        /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
        void FixedUpdate()
        {
            fixedUpdate?.TryInvokeContinuation();
        }

        /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
        public UniTask FixedUpdateAsync()
        {
            return new UniTask(fixedUpdate ?? (fixedUpdate = new ReusablePromise()));
        }
    }
}
#endif