using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

namespace Assets.ObjectTest
{
    [ExecuteInEditMode]
    public class EditorBehaviourTest : MonoBehaviour
    {
        void Start()
        {
            //ObservableWWW.Get("http://google.co.jp/")
            //    .Delay(TimeSpan.FromSeconds(3))
            //    .Subscribe(x => Debug.Log(x));

            Debug.Log("GO");
            //Observable.Interval(TimeSpan.FromSeconds(1))
            //    .Take(10)
            //    .Subscribe(x => Debug.Log(x));

            // MainThreadDispatcher.StartCoroutine(Hoge());
        }

        IEnumerator Hoge()
        {
            var www = new WWW("http://google.co.jp/");
            Debug.Log("W");
            yield return www;
            Debug.Log(www.text);
        }

        void Update()
        {
        }
    }
}
