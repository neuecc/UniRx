#if !UNITY_4_5

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountOnly : MonoBehaviour
{
    public Text text;

    int count = 0;

    public void Update()
    {
        text.text = (count++).ToString();
    }
}

#endif