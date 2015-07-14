using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MyEventClass
{
    public event Action<int> Hoge;
    public void Push(int x)
    {
        Hoge(x);
    }
}

public class MoGe
{
    public void Huga()
    {
    }
}

// for Scenes/NextSandBox
public class Sandbox2 : MonoBehaviour
{
    public Button button;

    void Awake()
    {
        MainThreadDispatcher.Initialize();
    }

    void Aaa(Action action)
    {
    }

    void Start()
    {
        button.OnClickAsObservable().Subscribe(_ =>
        {
            var list = Enumerable.Range(1, 10000).Select(x => new ReactiveProperty<int>(x)).ToArray();

            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in list)
                {
                    item.Select(x => x).Select(x => x).Select(x => x).Select(x => x).Subscribe();
                }
                sw.Stop();
                Debug.Log(sw.Elapsed.TotalMilliseconds + "ms");
            }
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in list)
                {
                    item.Value = -10;
                }
                sw.Stop();
                Debug.Log(sw.Elapsed.TotalMilliseconds + "ms");
            }
        });
    }
}