using UnityEngine;

namespace UniRx.Examples
{
    // Sample:Detect Touch event
    public class Sample3_ObservableMonoBehaviour : ObservableMonoBehaviour
    {
        public override void Awake()
        {
            // All events can subscribe by ***AsObservable

            // Object specified update
            // or Get Global Update Event => Observable.EveryUpdate()
            this.UpdateAsObservable()
                .SelectMany(_ => Input.touches)
                .Where(x => x.phase == TouchPhase.Began)
                .Where(x => Physics.Raycast(Camera.main.ScreenPointToRay(x.position)))
                .Subscribe(x =>
                {
                    Debug.Log(x.position);
                });

            // If you use ObservableMonoBehaviour, must call base method
            base.Awake();
        }
    }
}