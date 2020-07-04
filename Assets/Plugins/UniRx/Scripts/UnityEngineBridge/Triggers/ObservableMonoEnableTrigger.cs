using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableMonoEnableTrigger : ObservableTriggerBase
    {
        Subject<Unit> onEnable;
        Subject<Unit> onDisable;
        private MonoBehaviour behaviour;

        private void Start()
        {
            behaviour.ObserveEveryValueChanged(x => x.enabled).Subscribe(_=>{
                if (behaviour.enabled && onEnable != null) onEnable.OnNext(Unit.Default);
            });

            behaviour.ObserveEveryValueChanged(x => !x.enabled).Subscribe(_ => {
                if (!behaviour.enabled && onDisable != null) onDisable.OnNext(Unit.Default);
            });
        }
        
        public IObservable<Unit> OnMonoEnableAsObservable(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
            return onEnable ?? (onEnable = new Subject<Unit>());
        }
        
        /// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
        public IObservable<Unit> OnMonoDisableAsObservable(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
            return onDisable ?? (onDisable = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onEnable != null)
            {
                onEnable.OnCompleted();
            }
            if (onDisable != null)
            {
                onDisable.OnCompleted();
            }
        }
    }
}