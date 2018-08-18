#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncAnimatorTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<int> onAnimatorIK;
        AsyncTriggerPromiseDictionary<int> onAnimatorIKs;
        AsyncTriggerPromise<AsyncUnit> onAnimatorMove;
        AsyncTriggerPromiseDictionary<AsyncUnit> onAnimatorMoves;

        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(onAnimatorIK, onAnimatorIKs, onAnimatorMove, onAnimatorMoves);
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        void OnAnimatorIK(int layerIndex)
        {
            TrySetResult(onAnimatorIK, onAnimatorIKs, layerIndex);
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public UniTask<int> OnAnimatorIKAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onAnimatorIK, ref onAnimatorIKs, cancellationToken);
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        void OnAnimatorMove()
        {
            TrySetResult(onAnimatorMove, onAnimatorMoves, AsyncUnit.Default);
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public UniTask OnAnimatorMoveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onAnimatorMove, ref onAnimatorMoves, cancellationToken);
        }
    }
}

#endif