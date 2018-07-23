#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncJoint2DTrigger : MonoBehaviour
    {
        ReusablePromise<Joint2D> onJointBreak2D;

        void OnJointBreak2D(Joint2D brokenJoint)
        {
            onJointBreak2D?.TrySetResult(brokenJoint);
        }

        public UniTask<Joint2D> OnJointBreak2DAsync()
        {
            return new UniTask<Joint2D>(onJointBreak2D ?? (onJointBreak2D = new ReusablePromise<Joint2D>()));
        }
    }
}
#endif