using UnityEngine;
using System.Collections;
using UnityRx;
using System.Threading;
using System;

namespace Isolate
{
    public class NewBehaviourScript : ObservableMonoBehaviour
    {

        public override void OnMouseDown()
        {
            Debug.Log("Start MouseDown");

            //var a = Observable.Start(() =>
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(1));
            //    return 100;
            //});

            //var b = Observable.Start(() =>
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(2));
            //    return 200;
            //});

            //Observable.Zip(a, b, (x, y) =>
            //{
            //    return new { x, y };
            //})
            //.ObserveOnMainThread()
            //.Subscribe(x =>
            //{
            //    Debug.Log("Subscribe Start");
            //    var p = this.transform.position;
            //    this.transform.TransformPoint(p.x + 10, p.y + 10, p.z + 10);

            //    Debug.Log("End Transform");
            //});


            Observable.Return(1000)
                .Materialize()
                .Do(x=>
                {
                    Debug.Log(x);
                })
                .ObserveOnMainThread()
                .Subscribe(x =>
                {
                    Debug.Log("Subscribe Start");
                    Debug.Log("Subscribe End");
                });


            Debug.Log("End MouseDown");


            base.OnMouseDown();
        }
    }
}