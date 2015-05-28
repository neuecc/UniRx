using System;
using UniRx;
using UnityEngine;

namespace Assets.UniRx.Examples
{
    // PresenterBase can control multiple ReactivePresenter dependency.
    // This sample visualize PresenterBase's three phase initialization.

    // InEditor : Construct Children Dependency
    // Awake : Construct Parent Dependency
    // Start(Capture phase)  : Parent to Child, pass argument
    // Start(Bubbling phase) : Child to Parent, initialize(like constructor)

    // When play this sample scene, You can monitor cube color changes.
    // onplay, gray cube -> dark cube is can monitor initial value has been propageted.
    // yellow -> Awake, green -> Capture phase, red -> bubbling phase
    public class Sample14_PresenterBase : PresenterBase
    {
        public Sample14_CubeLevel1 child1;
        public Sample14_CubeLevel1 child2;

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[] { child1, child2 };
            }
        }

        // nomarly you don't need override OnAwake(Awake phase)
        protected override void OnAwake()
        {
            AnimationMarker.MarkAwakePhase(this);
        }

        // BeforeInitialize, you can propagate initial argument to child Presenter.
        // This phase, called parent -> child.
        // Therefore child isn't initialized yet, touch property is maybe dangerous(it's null!).
        // You should only do make initial value and propagate argument.
        protected override void BeforeInitialize()
        {
            AnimationMarker.MarkCapturePhase(this);

            // pass the argument by PresenterBase.PropagetArgument method.
            child1.PropagateArgument(0.4f);
            child2.PropagateArgument(0.4f);
        }

        protected override void Initialize()
        {
            // Initialize is like constructor!
            // see:Sample14_CubeLevel1.cs, Sample14_CubeLevel2.cs
            AnimationMarker.MarkBubblingPhase(this);
        }

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

        public void OnMouseDown()
        {
            // value is reactive!
            var f = UnityEngine.Random.Range(0f, 1f);
            this.GetComponent<Renderer>().material.color = new Color(f, f, f, 0);
            child1.Number.Value = f;
            child2.Number.Value = f;
        }

#endif
    }

    public static class AnimationMarker
    {
        const int AnimationSpeed = 10;

        static int Count = 0;

        public static IDisposable MarkAwakePhase(MonoBehaviour target)
        {
            Count++;
            return Observable.TimerFrame(AnimationSpeed * Count)
                .Subscribe(_ => target.GetComponent<Renderer>().material.color = Color.yellow);

        }

        public static IDisposable MarkCapturePhase(MonoBehaviour target)
        {
            Count++;
            return Observable.TimerFrame(AnimationSpeed * Count)
                .Subscribe(_ => target.GetComponent<Renderer>().material.color = Color.green);

        }

        public static IDisposable MarkBubblingPhase(MonoBehaviour target)
        {
            Count++;
            return Observable.TimerFrame(AnimationSpeed * Count)
                .Subscribe(_ => target.GetComponent<Renderer>().material.color = Color.red);
        }

        public static void Reset()
        {
            Count = 0;
        }
    }
}

