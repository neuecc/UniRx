using UnityEngine;
using System.Collections;
using UniRx;

public class SimpleMultiline : MonoBehaviour
{

    public StringReactiveProperty SinglelineString;

    [MultilineReactiveProperty]
    public StringReactiveProperty MultineString;

    [Multiline(3)]
    public string ML;

    [TextArea(5, 10)]
    public string TA;

    void Start()
    {
        SinglelineString.Subscribe(x =>
        {
            Debug.Log(x);
        });

        MultineString.Subscribe(x =>
        {
            Debug.Log(x);
        });
    }
}
