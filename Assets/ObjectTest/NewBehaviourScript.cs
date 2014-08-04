using UnityEngine;
using UniRx.UI;
using System.Collections;
using UniRx;
using System.Threading;
using System;
using System.Text;

// test sandbox
public class NewBehaviourScript : ObservableMonoBehaviour
{
    GameObject text;

    public override void Awake()
    {
        text = GameObject.Find("myGuiText");
        base.Awake();
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "LazyTask"))
        {
            StartCoroutine(Work());
        }
        if (GUI.Button(new Rect(0, 100, 100, 100), "Observe"))
        {
            Debug.Log(DateTime.Now.ToString());
            Observable.Timer(TimeSpan.FromSeconds(3))
                .ObserveOnMainThread() // comment out this line, get_guiText can only be called from the main thread.
                .Subscribe(x => text.guiText.text = DateTime.Now.ToString());
        }
    }

    IEnumerator Work()
    {
        var t1 = Observable.Interval(TimeSpan.FromSeconds(1)).Take(4).ToLazyTask();
        var t2 = Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => x * x).Take(4).ToLazyTask();
        var t3 = Observable.Throw<Unit>(new Exception()).ToLazyTask();

        yield return LazyTask.WhenAll(t1, t2, t3);

        Debug.Log(t1.Result + ":" + t2.Result);
        Debug.Log(t3.Exception);
    }
}
