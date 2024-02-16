#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using UnityEngine;
using UnityEngine.UI;

public class SandboxScene : MonoBehaviour
{
    public Button buttonA;
    public Button buttonB;
    // MyMyClass mc;
    //ReactiveProperty<int> rp = new ReactiveProperty<int>();


    //public async void Start()
    //{
    //    rp.Value = 10;

    //    buttonA.onClick.AddListener(() =>
    //    {
    //        rp.Value = 99;
    //    });

    //    Debug.Log("Begin:" + rp.Value);
    //    var v= await rp;
    //    Debug.Log("End:" + v);
    //}

}

public class MyMyClass
{
    public int MyProperty { get; set; }

    ~MyMyClass()
    {
        Debug.Log("GCed");
    }
}

#endif