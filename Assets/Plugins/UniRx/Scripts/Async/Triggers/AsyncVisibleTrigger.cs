#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncVisibleTrigger : MonoBehaviour
    {
        ReusablePromise onBecameInvisible;

        /// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
        void OnBecameInvisible()
        {
            onBecameInvisible?.TrySetResult();
        }

        /// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
        public UniTask OnBecameInvisibleAsync()
        {
            return new UniTask(onBecameInvisible ?? (onBecameInvisible = new ReusablePromise()));
        }

        ReusablePromise onBecameVisible;

        /// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
        void OnBecameVisible()
        {
            onBecameVisible?.TrySetResult();
        }

        /// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
        public UniTask OnBecameVisibleAsync()
        {
            return new UniTask(onBecameVisible ?? (onBecameVisible = new ReusablePromise()));
        }
    }
}
#endif