#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncStateMachineTrigger : StateMachineBehaviour
    {
        public struct OnStateInfo
        {
            public readonly Animator Animator;
            public readonly AnimatorStateInfo StateInfo;
            public readonly int LayerIndex;

            public OnStateInfo(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                Animator = animator;
                StateInfo = stateInfo;
                LayerIndex = layerIndex;
            }
        }

        public struct OnStateMachineInfo
        {
            public readonly Animator Animator;
            public readonly int StateMachinePathHash;

            public OnStateMachineInfo(Animator animator, int stateMachinePathHash)
            {
                Animator = animator;
                StateMachinePathHash = stateMachinePathHash;
            }
        }

        // OnStateExit

        ReusablePromise<OnStateInfo> onStateExit;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateExit?.TrySetResult(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public UniTask<OnStateInfo> OnStateExitAsync()
        {
            return new UniTask<OnStateInfo>(onStateExit ?? (onStateExit = new ReusablePromise<OnStateInfo>()));
        }

        // OnStateEnter

        ReusablePromise<OnStateInfo> onStateEnter;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateEnter?.TrySetResult(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public UniTask<OnStateInfo> OnStateEnterAsync()
        {
            return new UniTask<OnStateInfo>(onStateEnter ?? (onStateEnter = new ReusablePromise<OnStateInfo>()));
        }

        // OnStateIK

        ReusablePromise<OnStateInfo> onStateIK;

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateIK?.TrySetResult(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public UniTask<OnStateInfo> OnStateIKAsync()
        {
            return new UniTask<OnStateInfo>(onStateIK ?? (onStateIK = new ReusablePromise<OnStateInfo>()));
        }

        // Does not implments OnStateMove.
        // By defining OnAnimatorMove, you are signifying that you want to intercept the movement of the root object and apply it yourself.
        // http://fogbugz.unity3d.com/default.asp?700990_9jqaim4ev33i8e9h

        // OnStateUpdate

        ReusablePromise<OnStateInfo> onStateUpdate;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateUpdate?.TrySetResult(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public UniTask<OnStateInfo> OnStateUpdateAsync()
        {
            return new UniTask<OnStateInfo>(onStateUpdate ?? (onStateUpdate = new ReusablePromise<OnStateInfo>()));
        }

        // OnStateMachineEnter

        ReusablePromise<OnStateMachineInfo> onStateMachineEnter;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            onStateMachineEnter?.TrySetResult(new OnStateMachineInfo(animator, stateMachinePathHash));
        }

        public UniTask<OnStateMachineInfo> OnStateMachineEnterAsync()
        {
            return new UniTask<OnStateMachineInfo>(onStateMachineEnter ?? (onStateMachineEnter = new ReusablePromise<OnStateMachineInfo>()));
        }

        // OnStateMachineExit

        ReusablePromise<OnStateMachineInfo> onStateMachineExit;

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            onStateMachineExit?.TrySetResult(new OnStateMachineInfo(animator, stateMachinePathHash));
        }

        public UniTask<OnStateMachineInfo> OnStateMachineExitAsync()
        {
            return new UniTask<OnStateMachineInfo>(onStateMachineExit ?? (onStateMachineExit = new ReusablePromise<OnStateMachineInfo>()));
        }
    }
}

#endif