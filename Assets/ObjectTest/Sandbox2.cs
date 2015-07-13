using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

// for Scenes/NextSandBox
public class Sandbox2 : MonoBehaviour
{
    void Awake()
    {
        MainThreadDispatcher.Initialize();
    }
    void Start()
    {



        Observable.Range(1, 10)
            .Subscribe(x => Debug.Log(x));





    }
}

