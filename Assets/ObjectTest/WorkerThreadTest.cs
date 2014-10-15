using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class WorkerThreadTest : MonoBehaviour
    {
        void Awake()
        {
            Thread worker = new Thread(() =>
            {
                // Bind to application quit event.
                MainThreadDispatcher.OnApplicationQuitAsObservable().Subscribe(_ => Debug.Log("OnApplicationQuitAsObservable"));

                // Create capsules one by one.
                Observable.Interval(TimeSpan.FromMilliseconds(300))
                    .Take(5)
                    .Subscribe((s) =>
                    {
                        var g = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        g.name = "Capsule " + s;
                        g.transform.position += new Vector3(s, 0, 0);
                    });
            });
            worker.Start();
        }
    }
}