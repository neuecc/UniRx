#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncLateUpdateTrigger : MonoBehaviour
    {
        ReusablePromise lateUpdate;

        /// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
        void LateUpdate()
        {
            lateUpdate?.TrySetResult();
        }

        /// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
        public UniTask LateUpdateAsync()
        {
            return new UniTask(lateUpdate ?? (lateUpdate = new ReusablePromise()));
        }
    }
}
#endif