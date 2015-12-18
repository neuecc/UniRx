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

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.UpdateAsObservable()
                .SampleFrame(30)
                .TakeUntilDestroy(cube) // add line.
                .Subscribe(
                    __ =>
                    {
                        var p = cube.transform.position;
                        cube.transform.position = new Vector3(p.x + 0.4f, p.y, p.z);
                    },
                    e => Debug.LogError("Error! " + e),
                    () => Debug.Log("Completed!"));

            GameObject.Destroy(cube, 3f);

            Debug.Log(cube);
        });
    }
} 