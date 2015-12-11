using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
// using UnityEngine.SceneManagement;

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

    //int clickCount = 0;
    //AsyncOperation ao = null;

    void Start()
    {
        
        button.OnClickAsObservable().Subscribe(_ =>
        {
            //if (clickCount++ == 0)
            //{
            //    ao = SceneManager.LoadSceneAsync("TestSandbox");
            //    // Debug.Log(ao.allowSceneActivation);
            //    ao.allowSceneActivation = false;
            //    ao.AsAsyncOperationObservable(new Progress<float>(x =>
            //    {
            //        Debug.Log(x);
            //    })).Subscribe();
            //}
            //else
            //{
            //    ao.allowSceneActivation = true;
            //}
        });
    }
}