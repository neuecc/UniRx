#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncCanvasGroupChangedTrigger : MonoBehaviour
    {
        ReusablePromise onCanvasGroupChanged;

        // Callback that is sent if the canvas group is changed
        void OnCanvasGroupChanged()
        {
            onCanvasGroupChanged?.TrySetResult();
        }

        /// <summary>Callback that is sent if the canvas group is changed.</summary>
        public UniTask OnCanvasGroupChangedAsync()
        {
            return new UniTask(onCanvasGroupChanged ?? (onCanvasGroupChanged = new ReusablePromise()));
        }

        void OnDestroy()
        {
            onCanvasGroupChanged?.TrySetCanceled();
        }
    }
}

#endif