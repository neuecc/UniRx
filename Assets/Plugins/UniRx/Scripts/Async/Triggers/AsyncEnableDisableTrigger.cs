#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncEnableDisableTrigger : MonoBehaviour
    {
        ReusablePromise onEnable;

        /// <summary>This function is called when the object becomes enabled and active.</summary>
        void OnEnable()
        {
            onEnable?.TrySetResult();
        }

        /// <summary>This function is called when the object becomes enabled and active.</summary>
        public UniTask OnEnableAsync()
        {
            return new UniTask(onEnable ?? (onEnable = new ReusablePromise()));
        }

        ReusablePromise onDisable;

        /// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
        void OnDisable()
        {
            onDisable?.TrySetResult();
        }

        /// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
        public UniTask OnDisableAsync()
        {
            return new UniTask(onDisable ?? (onDisable = new ReusablePromise()));
        }
    }
}
#endif