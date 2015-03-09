using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UniRx.UI
{
    [AddComponentMenu("ObservableUI/Text", 11)]
    public class ObservableText : UnityEngine.UI.Text
    {
        #region UIBehaviour

        Subject<Unit> awake;

        protected override void Awake()
        {
            base.Awake();
            if (awake != null) awake.OnNext(Unit.Default);
        }

        public IObservable<Unit> AwakeAsObservable()
        {
            return awake ?? (awake = new Subject<Unit>());
        }

        Subject<Unit> onBeforeTransformParentChanged;

        protected override void OnBeforeTransformParentChanged()
        {
            base.Awake();
            if (onBeforeTransformParentChanged != null) onBeforeTransformParentChanged.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnBeforeTransformParentChangedAsObservable()
        {
            return onBeforeTransformParentChanged ?? (onBeforeTransformParentChanged = new Subject<Unit>());
        }

        Subject<Unit> onCanvasGroupChanged;

        protected override void OnCanvasGroupChanged()
        {
            base.Awake();
            if (onCanvasGroupChanged != null) onCanvasGroupChanged.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnCanvasGroupChangedAsObservable()
        {
            return onCanvasGroupChanged ?? (onCanvasGroupChanged = new Subject<Unit>());
        }

        Subject<Unit> onDestroy;

        protected override void OnDestroy()
        {
            base.Awake();
            if (onDestroy != null) onDestroy.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnDestroyAsObservable()
        {
            return onDestroy ?? (onDestroy = new Subject<Unit>());
        }

        Subject<Unit> onDidApplyAnimationProperties;

        protected override void OnDidApplyAnimationProperties()
        {
            base.Awake();
            if (onDidApplyAnimationProperties != null) onDidApplyAnimationProperties.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnDidApplyAnimationPropertiesAsObservable()
        {
            return onDidApplyAnimationProperties ?? (onDidApplyAnimationProperties = new Subject<Unit>());
        }

        Subject<Unit> onDisable;

        protected override void OnDisable()
        {
            base.Awake();
            if (onDisable != null) onDisable.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnDisableAsObservable()
        {
            return onDisable ?? (onDisable = new Subject<Unit>());
        }

        Subject<Unit> onEnable;

        protected override void OnEnable()
        {
            base.Awake();
            if (onEnable != null) onEnable.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnEnableAsObservable()
        {
            return onEnable ?? (onEnable = new Subject<Unit>());
        }

        Subject<Unit> onRectTransformDimensionsChange;

        protected override void OnRectTransformDimensionsChange()
        {
            base.Awake();
            if (onRectTransformDimensionsChange != null) onRectTransformDimensionsChange.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnRectTransformDimensionsChangeAsObservable()
        {
            return onRectTransformDimensionsChange ?? (onRectTransformDimensionsChange = new Subject<Unit>());
        }

        Subject<Unit> onTransformParentChanged;

        protected override void OnTransformParentChanged()
        {
            base.Awake();
            if (onTransformParentChanged != null) onTransformParentChanged.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnTransformParentChangedAsObservable()
        {
            return onTransformParentChanged ?? (onTransformParentChanged = new Subject<Unit>());
        }

#if UNITY_EDITOR

        Subject<Unit> onValidate;

        protected override void OnValidate()
        {
            base.Awake();
            if (onValidate != null) onValidate.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnValidateAsObservable()
        {
            return onValidate ?? (onValidate = new Subject<Unit>());
        }

        Subject<Unit> reset;

        protected override void Reset()
        {
            base.Awake();
            if (reset != null) reset.OnNext(Unit.Default);
        }

        public IObservable<Unit> ResetAsObservable()
        {
            return reset ?? (reset = new Subject<Unit>());
        }

#endif

        Subject<Unit> start;

        protected override void Start()
        {
            base.Awake();
            if (start != null) start.OnNext(Unit.Default);
        }

        public IObservable<Unit> StartAsObservable()
        {
            return start ?? (start = new Subject<Unit>());
        }

        #endregion
    }
}