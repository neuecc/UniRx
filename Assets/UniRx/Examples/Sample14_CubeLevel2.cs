using UniRx;
using UnityEngine;

namespace Assets.UniRx.Examples
{
    public class Sample14_CubeLevel2 : PresenterBase<float>
    {
        public IReactiveProperty<float> Number { get;private set; }

        protected override IPresenter[] Children
        {
            get
            {
                return EmptyChildren;
            }
        }

        protected override void OnAwake()
        {
            AnimationMarker.MarkAwakePhase(this);
        }

        protected override void BeforeInitialize(float argument)
        {
            AnimationMarker.MarkCapturePhase(this);
        }

        protected override void Initialize(float argument)
        {
            AnimationMarker.MarkBubblingPhase(this);

            Number = new ReactiveProperty<float>(argument);
            Number.Subscribe(x =>
            {
                this.GetComponent<Renderer>().material.color = new Color(x, 0.2f, 0.2f, 0);
            });
        }
    }
}