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
        Direct.Subscribe(x => Debug.Log("Direct " + x));
        MS.IRP.Subscribe(x => Debug.Log("IRP " + x));
        MS.Depth.IRP2.Subscribe(x => Debug.Log("IRP2 " + x));
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
