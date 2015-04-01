using UnityEngine;
using UniRx.Triggers; // for enable gameObject.EventAsObservbale()

namespace UniRx.Examples
{
    // Sample:Detect Touch event
    public class Sample03_GameObjectAsObservable : MonoBehaviour
    {
        void Start()
        {
            // All events can subscribe by ***AsObservable if enables UniRx.Triggers
            
            // Object specified update
            // or Get Global Update Event => Observable.EveryUpdate()
            // see:Sample8, it is more useful
            this.gameObject.UpdateAsObservable() // extension method
                .Do(_=> Debug.Log("do"))
                .SelectMany(_ => Input.touches.WrapValueToClass()) // aotsafe, wrap struct to class(Tuple1)
                .Where(x => x.Item1.phase == TouchPhase.Began)
                .Where(x => Physics.Raycast(Camera.main.ScreenPointToRay(x.Item1.position)))
                .Subscribe(x =>
                {
                    Debug.Log(x.Item1.position);
                });
        }
    }
}