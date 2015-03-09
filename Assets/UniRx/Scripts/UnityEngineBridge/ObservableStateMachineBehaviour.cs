using UnityEngine;

namespace UniRx
{
    public class ObservableStateMachineBehaviour : StateMachineBehaviour
    {
        public class OnStateInfo
        {
            public Animator Animator { get; private set; }
            public AnimatorStateInfo StateInfo { get; private set; }
            public int LayerIndex { get; private set; }

            public OnStateInfo(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                Animator = animator;
                StateInfo = stateInfo;
                LayerIndex = layerIndex;
            }
        }

        public class OnStateMachineInfo
        {
            public Animator Animator { get; private set; }
            public int StateMachinePathHash { get; private set; }

            public OnStateMachineInfo(Animator animator, int stateMachinePathHash)
            {
                Animator = animator;
                StateMachinePathHash = stateMachinePathHash;
            }
        }

        // OnStateExit

        Subject<OnStateInfo> onStateExit;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onStateExit != null) onStateExit.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public IObservable<OnStateInfo> OnStateExitAsObservable()
        {
            return onStateExit ?? (onStateExit = new Subject<OnStateInfo>());
        }

        // OnStateEnter

        Subject<OnStateInfo> onStateEnter;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onStateEnter != null) onStateEnter.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public IObservable<OnStateInfo> OnStateEnterAsObservable()
        {
            return onStateEnter ?? (onStateEnter = new Subject<OnStateInfo>());
        }

        // OnStateIK

        Subject<OnStateInfo> onStateIK;

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(onStateIK !=null) onStateIK.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public IObservable<OnStateInfo> OnStateIKAsObservable()
        {
            return onStateIK ?? (onStateIK = new Subject<OnStateInfo>());
        }

        // OnStateMove

        Subject<OnStateInfo> onStateMove;

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onStateMove != null) onStateMove.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public IObservable<OnStateInfo> OnStateMoveAsObservable()
        {
            return onStateMove ?? (onStateMove = new Subject<OnStateInfo>());
        }
        // OnStateUpdate

        Subject<OnStateInfo> onStateUpdate;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onStateUpdate != null) onStateUpdate.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
        }

        public IObservable<OnStateInfo> OnStateUpdateAsObservable()
        {
            return onStateUpdate ?? (onStateUpdate = new Subject<OnStateInfo>());
        }

        // OnStateMachineEnter

        Subject<OnStateMachineInfo> onStateMachineEnter;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (onStateMachineEnter != null) onStateMachineEnter.OnNext(new OnStateMachineInfo(animator, stateMachinePathHash));
        }

        public IObservable<OnStateMachineInfo> OnStateMachineEnterAsObservable()
        {
            return onStateMachineEnter ?? (onStateMachineEnter = new Subject<OnStateMachineInfo>());
        }

        // OnStateMachineExit

        Subject<OnStateMachineInfo> onStateMachineExit;

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            if (onStateMachineExit != null) onStateMachineExit.OnNext(new OnStateMachineInfo(animator, stateMachinePathHash));
        }

        public IObservable<OnStateMachineInfo> OnStateMachineExitAsObservable()
        {
            return onStateMachineExit ?? (onStateMachineExit = new Subject<OnStateMachineInfo>());
        }
    }
}