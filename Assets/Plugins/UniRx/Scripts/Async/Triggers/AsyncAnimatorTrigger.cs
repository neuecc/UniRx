#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncAnimatorTrigger : AsyncTriggerBase2
    {
        ReusablePromise<int> onAnimatorIK;
        ReusablePromise onAnimatorMove;

        protected override (ICancelablePromise, ICancelablePromise) Promises => (onAnimatorIK, onAnimatorMove);

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        void OnAnimatorIK(int layerIndex)
        {
            onAnimatorIK?.TrySetResult(layerIndex);
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public UniTask<int> OnAnimatorIKAsync()
        {
            return new UniTask<int>(onAnimatorIK ?? (onAnimatorIK = new ReusablePromise<int>()));
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public UniTask<(bool, int)> OnAnimatorIKWithIsCanceledAsync()
        {
            return new UniTask<int>(onAnimatorIK ?? (onAnimatorIK = new ReusablePromise<int>())).WithIsCanceled(CancellationToken);
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        void OnAnimatorMove()
        {
            onAnimatorMove?.TrySetResult();
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public UniTask<AsyncUnit> OnAnimatorMoveAsync()
        {
            return new UniTask(onAnimatorMove ?? (onAnimatorMove = new ReusablePromise()));
        }
    }
}

#endif