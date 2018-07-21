#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncRectTransformTrigger : MonoBehaviour
    {
        ReusablePromise onRectTransformDimensionsChange;

        // Callback that is sent if an associated RectTransform has it's dimensions changed
        public void OnRectTransformDimensionsChange()
        {
            onRectTransformDimensionsChange?.TryInvokeContinuation();
        }

        /// <summary>Callback that is sent if an associated RectTransform has it's dimensions changed.</summary>
        public UniTask OnRectTransformDimensionsChangeAsync()
        {
            return new UniTask(onRectTransformDimensionsChange ?? (onRectTransformDimensionsChange = new ReusablePromise()));
        }

        ReusablePromise onRectTransformRemoved;

        // Callback that is sent if an associated RectTransform is removed
        public void OnRectTransformRemoved()
        {
            onRectTransformRemoved?.TryInvokeContinuation();
        }

        /// <summary>Callback that is sent if an associated RectTransform is removed.</summary>
        public UniTask OnRectTransformRemovedAsync()
        {
            return new UniTask(onRectTransformRemoved ?? (onRectTransformRemoved = new ReusablePromise()));
        }
    }
}

#endif