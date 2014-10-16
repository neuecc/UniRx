using System;
using UniRx;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class IntervalTest : MonoBehaviour
    {
        private GUIText cullLabel;
        private int counter = 0;

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
            cullLabel = GameObject.Find("CullLabel").GetComponent<GUIText>();
            cullLabel.gameObject.AddComponent<Clicker>().OnClicked += () =>
            {
                MainThreadDispatcher.IsCullingEnabled = !MainThreadDispatcher.IsCullingEnabled;
                new GameObject("New MTD #" + counter++).AddComponent<MainThreadDispatcher>();
            };

            var buttonGo = GameObject.Find("ButtonSphere");
            var clicker = buttonGo.AddComponent<Clicker>();
            var max = 30;

            clicker.OnClicked += () =>
            {
                Debug.Log(string.Format("Is MainThreadDispatcher initialized? {0}{1}", MainThreadDispatcher.IsInitialized,
                    MainThreadDispatcher.IsInitialized ? ", running on " + MainThreadDispatcher.InstanceName : string.Empty));

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

        void Update()
        {
            cullLabel.text = string.Format("Culling excess dispatchers: <b>{0}</b>.\nClick to toggle and create a new dispatcher."
                , MainThreadDispatcher.IsCullingEnabled.ToString());
        }
    }

    public class Clicker : MonoBehaviour
    {
        public event Action OnClicked = delegate { };

        void OnMouseDown()
        {
            //Debug.Log("Clicked");
            OnClicked();
        }
    }
}
