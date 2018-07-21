#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncJointTrigger : MonoBehaviour
    {
        ReusablePromise<float> onJointBreak;

        void OnJointBreak(float breakForce)
        {
            onJointBreak?.TryInvokeContinuation(breakForce);
        }

        public UniTask<float> OnJointBreakAsync()
        {
            return new UniTask<float>(onJointBreak ?? (onJointBreak = new ReusablePromise<float>()));
        }
    }
}
#endif