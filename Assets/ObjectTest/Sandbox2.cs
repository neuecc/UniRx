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


    void Start()
    {
        Debug.Log("start");

        var subject = new Subject<int>();
        var rxp = subject.ToReactiveProperty();

        Debug.Log("before subscribe");
        rxp.Subscribe(x => Debug.Log("called:" + x));
        Debug.Log("after subscribe");

        subject.OnNext(0);
    }
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
