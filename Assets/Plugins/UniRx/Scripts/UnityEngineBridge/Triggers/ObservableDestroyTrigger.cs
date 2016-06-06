using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableDestroyTrigger : MonoBehaviour
    {
        bool calledDestroy = false;
        Subject<Unit> onDestroy;

        [Obsolete("Internal Use.")]
        internal bool IsMonitoredActivate { get; set; }

        public bool IsActivated { get; private set; }

        void Awake()
        {
            IsActivated = true;
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        void OnDestroy()
        {
            if (!calledDestroy)
            {
                calledDestroy = true;
                if (onDestroy != null) { onDestroy.OnNext(Unit.Default); onDestroy.OnCompleted(); }
            }
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public IObservable<Unit> OnDestroyAsObservable()
        {
            if (this == null) return Observable.Return(Unit.Default);
            if (calledDestroy) return Observable.Return(Unit.Default);
            return onDestroy ?? (onDestroy = new Subject<Unit>());
        }

        /// <summary>Invoke OnDestroy, this method is used on internal.</summary>
        public void ForceRaiseOnDestroy()
        {
            OnDestroy();
        }
    }
}