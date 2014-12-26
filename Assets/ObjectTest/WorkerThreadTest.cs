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
#if UNITY_METRO
            // Windows Store doesn't support System.Threading.Thread.
            // Other platforms can use both ThreadPool and System.Threading.Thread.
            // ThreadPool.QueueUserWorkItem(RegisterApplicationQuitEvent);
            // ThreadPool.QueueUserWorkItem(SpawnCapsules);
#else
            Thread thread = new Thread(SpawnCapsules);
            thread.Start();
#endif
        }

        private void RegisterApplicationQuitEvent(object a)
        {
            MainThreadDispatcher.OnApplicationQuitAsObservable().Subscribe(_ => Debug.Log("OnApplicationQuitAsObservable"));
        }

        private void SpawnCapsules(object a)
        {
            // Create capsules one by one.
            Observable.Interval(TimeSpan.FromMilliseconds(300))
                .Take(5)
                .Subscribe((s) =>
                {
                    var g = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    g.name = "Capsule " + s;
                    g.transform.position += new Vector3(s, 0, 0);
                });
        }
    }
}