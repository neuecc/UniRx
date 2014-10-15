using System;
using UniRx;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class IntervalTest : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log(string.Format("Awake(). Current MainThreadDispatcher: {0}", MainThreadDispatcher.InstanceName));

            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe((s) =>
                    {
                        Debug.Log(string.Format("[Outer interval] {0} running on: {1}", s, MainThreadDispatcher.InstanceName));

                        Observable
                            .Interval(TimeSpan.FromMilliseconds(40))
                            .Take(5)
                            .Subscribe(
                                ms =>
                                {
                                    Debug.Log(string.Format("    Inner interval {0} running on: {1}", ms, MainThreadDispatcher.InstanceName));
                                },
                                innerEx => Debug.LogException(innerEx)
                            );
                    },
                    outerEx => Debug.LogException(outerEx)
                );
        }

        void Start()
        {
            var buttonGo = GameObject.Find("ButtonSphere");
            var clicker = buttonGo.AddComponent<Clicker>();
            var max = 30;

            clicker.OnClicked += () =>
            {
                Debug.Log("Is MainThreadDispatcher initialized? " + MainThreadDispatcher.IsInitialized);

                Observable
                    .Interval(TimeSpan.FromMilliseconds(1))
                    .Take(max)
                    .Subscribe((ms) =>
                    {
                        if (ms >= max / 2)
                        {
                            buttonGo.transform.localScale /= 1.1f;
                        }
                        else
                        {
                            buttonGo.transform.localScale *= 1.1f;
                        }
                        if (ms == max - 1)
                        {
                            buttonGo.transform.localScale = Vector3.one;
                        }
                    });

                Debug.Log(string.Format("Sphere running on: {0}", MainThreadDispatcher.InstanceName));
            };
        }
    }

    public class Clicker : MonoBehaviour
    {
        public event Action OnClicked = delegate { };

        void OnMouseDown()
        {
            OnClicked();
        }
    }
}
