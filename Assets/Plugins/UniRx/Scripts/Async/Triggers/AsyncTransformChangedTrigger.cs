#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncTransformChangedTrigger : MonoBehaviour
    {
        ReusablePromise onBeforeTransformParentChanged;

        // Callback sent to the graphic before a Transform parent change occurs
        void OnBeforeTransformParentChanged()
        {
            onBeforeTransformParentChanged?.TrySetResult();
        }

        /// <summary>Callback sent to the graphic before a Transform parent change occurs.</summary>
        public UniTask OnBeforeTransformParentChangedAsync()
        {
            return new UniTask(onBeforeTransformParentChanged ?? (onBeforeTransformParentChanged = new ReusablePromise()));
        }

        ReusablePromise onTransformParentChanged;

        // This function is called when the parent property of the transform of the GameObject has changed
        void OnTransformParentChanged()
        {
            onTransformParentChanged?.TrySetResult();
        }

        /// <summary>This function is called when the parent property of the transform of the GameObject has changed.</summary>
        public UniTask OnTransformParentChangedAsync()
        {
            return new UniTask(onTransformParentChanged ?? (onTransformParentChanged = new ReusablePromise()));
        }

        ReusablePromise onTransformChildrenChanged;

        // This function is called when the list of children of the transform of the GameObject has changed
        void OnTransformChildrenChanged()
        {
            onTransformChildrenChanged?.TrySetResult();
        }

        /// <summary>This function is called when the list of children of the transform of the GameObject has changed.</summary>
        public UniTask OnTransformChildrenChangedAsync()
        {
            return new UniTask(onTransformChildrenChanged ?? (onTransformChildrenChanged = new ReusablePromise()));
        }
    }
}

#endif