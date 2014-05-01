using UnityEngine;
using System.Collections;
using UnityRx;
using System.Threading;
using System;


// test sandbox
public class NewBehaviourScript : ObservableMonoBehaviour
{
    IDisposable cancel;

    public override void Awake()
    {
        cancel = Observable.EveryFrame()
            .Subscribe(_ => Debug.Log(DateTime.Now.ToString()));

        base.Awake();
    }

    public override void OnMouseDown()
    {
        Debug.Log("Start MouseDown");
        cancel.Dispose();

        //Observable.Return(10)
        //    .DelayFrame(180)
        //    .Subscribe(x =>
        //    {
        //        (GameObject.Find("myGuiText")).guiText.text = DateTime.Now.ToString();
        //    });

        //var a = Observable.Start(() =>
        //{
        //    //Thread.Sleep(TimeSpan.FromSeconds(1));
        //    return 100;
        //});

        //var b = Observable.Start(() =>
        //{
        //    //Thread.Sleep(TimeSpan.FromSeconds(2));
        //    return 200;
        //});

        //var notification = new UnityRx.ScheduledNotifier<float>(Scheduler.MainThread);
        //notification.Subscribe(x => Debug.Log(x));

        //Observable.Zip(a, b, (x, y) =>
        //{
        //    return new { x, y };
        //})
        //.Zip(ObservableWWW.Get("http://google.co.jp/", progress: notification), (x, y) => y)
        //.ObserveOnMainThread()
        //.Subscribe(x =>
        //{
        //    Debug.Log("Subscribe Start");
        //    //var p = this.transform.position;
        //    //var np = this.transform.TransformPoint(p.x + 0.5f, p.y + 0.5f, p.z);
        //    //this.transform.position = np;

        //    (GameObject.Find("myGuiText")).guiText.text = Guid.NewGuid() + x;

        //    Debug.Log("End Transform");
        //});

        //Debug.Log("End MouseDown");


        base.OnMouseDown();
    }
}
