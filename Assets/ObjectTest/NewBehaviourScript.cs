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
    // IDisposable _____cancel;

    public class MyClassPropertyField
    {
        public int Property { get; set; }
        public int Field { get; set; }
    }

    GameObject text;

    public override void Awake()
    {
        text = GameObject.Find("myGuiText");

        //var mc = new MyClassPropertyField { Field = 10, Property = 100 };


        //mc.ObserveEveryValueChanged(x => x.Field)
        //    .Subscribe(x => text.guiText.text = x.ToString());

        //Observable.Interval(TimeSpan.FromSeconds(1))
        //    .Subscribe(x => mc.Field = (int)x);


        base.Awake();
    }

    MultipleAssignmentDisposable disp = new MultipleAssignmentDisposable();



    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "LazyTask"))
        {
            StartCoroutine(Work());
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
