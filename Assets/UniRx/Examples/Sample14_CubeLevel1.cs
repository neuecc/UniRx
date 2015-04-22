using UniRx;
using UnityEngine;

namespace Assets.UniRx.Examples
{
    // see:Sample14_PresenterBase
    public class Sample14_CubeLevel1 : PresenterBase<float>
    {
        public IReactiveProperty<float> Number { get; private set; }

        public Sample14_CubeLevel2 child1;
        public Sample14_CubeLevel2 child2;

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[] { child1, child2 };
            }
        }

        protected override void OnAwake()
        {
            AnimationMarker.MarkAwakePhase(this);
        }

        protected override void BeforeInitialize(float argument)
        {
            AnimationMarker.MarkCapturePhase(this);

            child1.PropagateArgument(argument * 2);
            child2.PropagateArgument(argument * 2);
        }

        // Initialize is like constructor.
        // This phase, called child -> parent.
        // Therefore you can touch child's property safety.
        protected override void Initialize(float argument)
        {
            AnimationMarker.MarkBubblingPhase(this);

            Number = new ReactiveProperty<float>(argument);
            Number.Subscribe(x =>
            {
                child1.Number.Value = x;
                child2.Number.Value = x;
                this.GetComponent<Renderer>().material.color = new Color(0.2f, x, 0.2f, 0);
            });
        }
    }
}