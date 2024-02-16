using System; // require keep for Windows Universal App
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableEnableTrigger : ObservableTriggerBase
    {
        public class MonoListener
        {
            public MonoBehaviour behaviour;
            public Subject<Unit> onEnable;
            public Subject<Unit> onDisable;

            public MonoListener(MonoBehaviour behaviour)
            {
                this.behaviour = behaviour;
            }
        }

        /// <summary>
        /// A dictionary of monobehaviours on this game object. 
        /// Add each to this dictionary by calling myMonoBehaviour.OnEnableAsObservable() and/or myMonoBehaviour.OnDisableAsObservable().
        /// Each has a unique onEnable and onDisable subject, so their enabled properties can be individually tracked.
        /// </summary>
        private readonly Dictionary<int, MonoListener> listeners = new Dictionary<int, MonoListener>();

        /// <summary>
        /// This is the single subject used when you call myGameObject.OnEnableAsObservable() or myTransform.OnEnableAsObservable().
        /// </summary>
        private Subject<Unit> onEnable;

        /// <summary>
        /// This is the single subject used when you call myGameObject.OnDisableAsObservable() or myTransform.OnDisableAsObservable().
        /// </summary>
        private Subject<Unit> onDisable;

        /// <summary>This function is called when the object becomes enabled and active.</summary>
        void OnEnable()
        {
            // All subjects are null when this executes the first time, so we need this in Start as well
            if (onEnable != null) onEnable.OnNext(Unit.Default);

            foreach (KeyValuePair<int, MonoListener> listenerPair in listeners)
                if (listenerPair.Value.onEnable != null)
                    listenerPair.Value.onEnable.OnNext(Unit.Default);
        }

        /// <summary>This function is called when this trigger is first instantiated, one frame after subjects are allocated.</summary>
        void Start()
        {
            if (onEnable != null) onEnable.OnNext(Unit.Default);

            foreach (KeyValuePair<int, MonoListener> listenerPair in listeners)
                if (listenerPair.Value.onEnable != null && listenerPair.Value.behaviour.enabled)
                    listenerPair.Value.onEnable.OnNext(Unit.Default);
        }

        /// <summary>This function is called when the game object becomes enabled and active.</summary>
        public IObservable<Unit> OnEnableAsObservable()
        {
            return onEnable ?? (onEnable = new Subject<Unit>());
        }

        /// <summary>This function is called when the target behaviour(s) become enabled or active.</summary>
        public IObservable<Unit> OnEnableAsObservable(MonoBehaviour behaviour)
        {
            if (!listeners.ContainsKey(behaviour.GetInstanceID()))
                listeners.Add(behaviour.GetInstanceID(), new MonoListener(behaviour));

            if (listeners[behaviour.GetInstanceID()].onEnable == null)
                InitializeSubject(true, behaviour.GetInstanceID());

            return listeners[behaviour.GetInstanceID()].onEnable;
        }

        /// <summary>This function is called when the game object becomes disabled or inactive.</summary>
        public IObservable<Unit> OnDisableAsObservable()
        {
            return onDisable ?? (onDisable = new Subject<Unit>());
        }

        /// <summary>This overload function is called when the target behaviour(s) become disabled or inactive.</summary>
        public IObservable<Unit> OnDisableAsObservable(MonoBehaviour behaviour)
        {
            if (!listeners.ContainsKey(behaviour.GetInstanceID()))
                listeners.Add(behaviour.GetInstanceID(), new MonoListener(behaviour));

            if (listeners[behaviour.GetInstanceID()].onDisable == null)
                InitializeSubject(false, behaviour.GetInstanceID());

            return listeners[behaviour.GetInstanceID()].onDisable;
        }

        /// <summary>
        /// This function allocates a new onEnable or onDisable subject for a target behaviour and then 
        /// listens for that behaviour's enabled property to inform the subject of changes.
        /// </summary>
        /// <param name="enabled">if true, use the onEnable subject, otherwise use the onDisable subject</param>
        /// <param name="instanceID">the target behaviour's instance ID</param>
        public void InitializeSubject(bool enabled, int instanceID)
        {
            if (enabled) listeners[instanceID].onEnable = new Subject<Unit>();
            else listeners[instanceID].onDisable = new Subject<Unit>();
            listeners[instanceID].behaviour.ObserveEveryValueChanged(x => x.enabled)
                .Subscribe(x => {
                    if (listeners[instanceID].behaviour.enabled == enabled)
                    {
                        if (enabled) listeners[instanceID].onEnable.OnNext(Unit.Default);
                        else listeners[instanceID].onDisable.OnNext(Unit.Default);
                    }
                });
        }

        /// <summary>This function is called when the gameObject becomes disabled () or inactive.</summary>
        void OnDisable()
        {
            if (onDisable != null) onDisable.OnNext(Unit.Default);

            foreach (KeyValuePair<int, MonoListener> listenerPair in listeners)
                if (listenerPair.Value.onEnable != null && listenerPair.Value.behaviour.enabled)
                    listenerPair.Value.onEnable.OnNext(Unit.Default);
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

            foreach (KeyValuePair<int, MonoListener> listenerPair in listeners)
            {
                if (listenerPair.Value.onEnable != null)
                    listenerPair.Value.onEnable.OnCompleted();

                if (listenerPair.Value.onDisable != null)
                    listenerPair.Value.onDisable.OnCompleted();
            }
        }
    }
}