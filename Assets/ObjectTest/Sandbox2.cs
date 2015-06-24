using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Linq;
using System;

public class Sandbox2 : MonoBehaviour
{
    [SerializeField]
    public MySerializable MS;

    [SerializeField]
    public IntReactiveProperty Direct;

    [SerializeField]
    public ObjectiveModel HogeMoge;

    [SerializeField]
    public IntReactiveProperty[] FirstLayerArray;


    void Start()
    {
        FirstLayerArray[0].Subscribe(x => Debug.Log(x));
        HogeMoge.ArrayIntReactiveProps[0].Subscribe(x => Debug.Log(x));
        HogeMoge.AiueoObjects[1].Obtained.Subscribe(x => Debug.Log(x));
        HogeMoge.AiueoObjects[1].IntHogeMoge.Subscribe(x => Debug.Log(x));
    }
}


[System.Serializable]
public class ObjectiveModel
{
    public string Id;
    public string Name;
    public string Info;
    public BoolReactiveProperty Finished;

    public int ObjectiveNum = 0;

    public IntReactiveProperty Array; // not array:)
    public ObjectiveThingsModel[] AiueoObjects;
    public IntReactiveProperty[] ArrayIntReactiveProps;

    public void Initialize()
    {

        ObjectiveNum = AiueoObjects.Length;

        //observe everything in objectivethings model
        foreach (ObjectiveThingsModel otm in AiueoObjects)
        {
            otm.Obtained.Where(obtained => obtained == true)
                .Subscribe(_ =>
                {
                    ObjectiveNum--;
                });

        }
    }
}

[Serializable]
public class ObjectiveThingsModel
{
    public BoolReactiveProperty Obtained;
    public IntReactiveProperty IntHogeMoge;
    public int TadanoInt;
    public IntReactiveProperty[] MoreInArray;
}


[Serializable]
public class MySerializable
{
    [SerializeField]
    public int Huga;

    [SerializeField]
    public IntReactiveProperty IRP;

    [SerializeField]
    public Depth2 Depth;
}

[Serializable]
public class Depth2
{
    [SerializeField]
    public int Hage;

    [SerializeField]
    public IntReactiveProperty IRP2;

    [SerializeField]
    private ColorReactiveProperty CRR;

    [SerializeField]
    private QuaternionReactiveProperty Qor;
}
