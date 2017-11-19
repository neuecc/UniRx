using System;
using UnityEngine;

/*
PresenterBase works enough, but too complex.
You can use simple Initialize method and call parent to child, it works for most scenario.
So I don't recommend using PresenterBase, sorry.
*/

namespace UniRx
{
    // InEditor : Construct Children Dependency
    // Awake : Construct Parent Dependency
    // Start(Capture phase)  : Parent to Child, pass argument
    // Start(Bubbling phase) : Child to Parent, initialize(like constructor)

    /// <summary>
    /// [Obsolete]Infrastructure interface for PresenterBase`T
    /// </summary>
    public interface IPresenter
    {
        IPresenter Parent { get; }
        GameObject gameObject { get; }
        void RegisterParent(IPresenter parent);
        void InitializeCore();
        void StartCapturePhase();
        void Awake();
        void ForceInitialize(object argument);
    }

    /// <summary>
    /// [Obsolete]PresenterBase can control dependency of presenter's hierarchy.
    /// </summary>
    public abstract class PresenterBase : PresenterBase<Unit>
    {
        protected sealed override void BeforeInitialize(Unit argument)
        {
            BeforeInitialize();
        }

        /// <summary>
        /// Same as Start but called before children initialized, it's chance for propagate argument to children.
        /// </summary>
        protected abstract void BeforeInitialize();

        protected override void Initialize(Unit argument)
        {
            Initialize();
        }

        /// <summary>
        /// Force Start BeforeInitialize/Initialize. If you create presenter dynamically, maybe useful.
        /// </summary>
        public void ForceInitialize()
        {
            ForceInitialize(Unit.Default);
        }

        /// <summary>
        /// Same as Start but called after all children are initialized.
        /// </summary>
        protected abstract void Initialize();
    }

    /// <summary>
    /// [Obsolete]PresenterBase can control dependency of presenter's hierarchy.
    /// </summary>
    public abstract class PresenterBase<T> : MonoBehaviour, IPresenter
    {
        protected static readonly IPresenter[] EmptyChildren = new IPresenter[0];

        int childrenCount = 0;
        int currentCalledCount = 0;
        bool isAwaken = false;
        bool isInitialized = false;
        bool isStartedCapturePhase = false;
        Subject<Unit> initializeSubject = null;
        IPresenter[] children = null; // set from awake

        IPresenter parent = null;
        T argument = default(T);

        public IPresenter Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Observable sequence called after initialize completed.
        /// </summary>
        public IObservable<Unit> InitializeAsObservable()
        {
            if (isInitialized) return Observable.Return(Unit.Default);
            return initializeSubject ?? (initializeSubject = new Subject<Unit>());
        }

        /// <summary>
        /// Propagate(Set) argument.
        /// </summary>
        public void PropagateArgument(T argument)
        {
            this.argument = argument;
        }

        /// <summary>
        /// Dependency on hierarchy of this presenter. If Children is empty, you can return this.EmptyChildren.
        /// </summary>
        protected abstract IPresenter[] Children { get; }

        /// <summary>
        /// Same as Start but called before children initialized, it's chance for propagate argument to children.
        /// </summary>
        protected abstract void BeforeInitialize(T argument);

        /// <summary>
        /// Same as Start but called after all children are initialized.
        /// </summary>
        protected abstract void Initialize(T argument);

        /// <summary>
        /// Force Start BeforeInitialize/Initialize. If you create presenter dynamically, maybe useful.
        /// </summary>
        public virtual void ForceInitialize(T argument)
        {
            Awake();
            PropagateArgument(argument);
            Start();
        }

        void IPresenter.ForceInitialize(object argument)
        {
            ForceInitialize((T)argument);
        }

        void IPresenter.Awake()
        {
            if (isAwaken) return;
            isAwaken = true;

            children = Children;
            childrenCount = children.Length;

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                child.RegisterParent(this);
                child.Awake(); // call Awake directly
            }
            OnAwake();
        }

        /// <summary>Infrastructure method called by UnityEngine. If you needs override Awake, override OnAwake.</summary>
        protected void Awake()
        {
            (this as IPresenter).Awake();
        }

        /// <summary>An alternative of Awake.</summary>
        protected virtual void OnAwake()
        {
        }

        /// <summary>Infrastructure method called by UnityEngine. don't call directly, don't override, don't hide!</summary>
        protected void Start()
        {
            if (isStartedCapturePhase) return;
            var root = parent;

            // Search root object
            if (root == null)
            {
                root = this;
            }
            else
            {
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
            }

            root.StartCapturePhase();
        }

        void IPresenter.StartCapturePhase()
        {
            isStartedCapturePhase = true;
            BeforeInitialize(argument);

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                child.StartCapturePhase();
            }

            // Start Bubbling phase
            if (children.Length == 0)
            {
                Initialize(argument);
                isInitialized = true;
                if (initializeSubject != null) { initializeSubject.OnNext(Unit.Default); initializeSubject.OnCompleted(); }
                if (parent != null)
                {
                    parent.InitializeCore();
                }
            }
        }

        void IPresenter.RegisterParent(IPresenter parent)
        {
            if (this.parent != null) throw new InvalidOperationException("PresenterBase can't register multiple parent. Name:" + this.name);

            this.parent = parent;
        }

        void IPresenter.InitializeCore()
        {
            currentCalledCount += 1;
            if (childrenCount == currentCalledCount)
            {
                Initialize(argument);
                isInitialized = true;
                if (initializeSubject != null) { initializeSubject.OnNext(Unit.Default); initializeSubject.OnCompleted(); }
                if (parent != null)
                {
                    parent.InitializeCore();
                }
            }
        }
    }
}