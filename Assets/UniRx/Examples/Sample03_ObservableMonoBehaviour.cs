using UnityEngine;

namespace UniRx.Examples
{
    // Sample:Detect Touch event
    public class Sample03_ObservableMonoBehaviour : ObservableMonoBehaviour
    {
        public override void Start()
        {
            // All events can subscribe by ***AsObservable

            // Object specified update
            // or Get Global Update Event => Observable.EveryUpdate()
            // see:Sample8, it is more useful
            this.UpdateAsObservable()
                .SelectMany(_ => Input.touches.WrapValueToClass()) // aotsafe, wrap struct to class(Tuple1)
                .Where(x => x.Item1.phase == TouchPhase.Began)
                .Where(x => Physics.Raycast(Camera.main.ScreenPointToRay(x.Item1.position)))
                .Subscribe(x =>
                {
                    Debug.Log(x.Item1.position);
                });

            // If you use ObservableMonoBehaviour, must call base method
            base.Start();
        }
    }
}