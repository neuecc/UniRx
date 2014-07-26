using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample8_DetectDoubleClick : TypedMonoBehaviour
    {
        public override void Awake()
        {
            // Global event handling is very useful.
            // UniRx can handle there events.
            // Observable.EveryUpdate/EveryFixedUpdate/EveryEndOfFrame
            // Observable.EveryApplicationFocus/EveryApplicationPause
            // Observable.OnceApplicationQuit

            //Observable.EveryUpdate()
            //    .Where(_ => Input.GetMouseButtonDown(0))
            //    .Buffer(TimeSpan.FromSeconds(2)
        }
    }
}