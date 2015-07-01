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
    private class Operator : IEnumerator
    {
        public object Current
        {
            get { return 1; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }
    }

    void Awake()
    {
        MainThreadDispatcher.Initialize();
    }
    void Start()
    {
        int frameStarted = Time.frameCount;
        var ope = new Operator();
        ope.ToObservable().Subscribe(
            _ => Debug.Log(string.Format("unirx next: consumed frames = {0}", Time.frameCount - frameStarted)),
            () => Debug.Log(string.Format("unirx cmpl: consumed frames = {0}", Time.frameCount - frameStarted))
        );
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
    public List<ObjectiveThingsModel> AiueoObjects;
    public IntReactiveProperty[] ArrayIntReactiveProps;

    public void Initialize()
    {

        ObjectiveNum = AiueoObjects.Count;

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
