using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Linq;

public class Sandbox2 : MonoBehaviour
{
    void Start()
    {
        var mt = this.gameObject.AddComponent<ObservableMouseTrigger>();
        mt.OnMouseDownAsObservable()
            .Subscribe(_ => Debug.Log("mouse down"));
    }



}
