using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountOnly : MonoBehaviour
{
    public Text text;

    int count = 0;

    void Update()
    {
        text.text = (count++).ToString();
    }
}
