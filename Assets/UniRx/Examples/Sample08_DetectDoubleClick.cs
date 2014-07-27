using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample08_DetectDoubleClick : TypedMonoBehaviour
    {
        public override void Awake()
        {
            // Global event handling is very useful.
            // UniRx can handle there events.
            // Observable.EveryUpdate/EveryFixedUpdate/EveryEndOfFrame
            // Observable.EveryApplicationFocus/EveryApplicationPause
            // Observable.OnceApplicationQuit

            var detected = false;
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Buffer(TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(100))
                .Where(xs => xs.Count >= 2 && !detected)
                .Do(_ =>
                {
                    detected = true;
                    Scheduler.ThreadPool.Schedule(TimeSpan.FromSeconds(1), () => detected = false);
                })
                .ObserveOnMainThread()
                .Subscribe(_ => Debug.Log("DoubleClick Detected"));
        }
    }
}