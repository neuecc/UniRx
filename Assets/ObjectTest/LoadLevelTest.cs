#if !UNITY_5_3
using System;
using System.Collections;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class LoadLevelTest : MonoBehaviour
    {
        void Start()
        {
            //var ll = GameObject.Find("LoadLevel");
            //var llcolor = ll.GetComponent<GUIText>().color;
            //var cll = ll.AddComponent<Clicker>();
            //cll.OnClicked += () => Application.LoadLevel("LoadLevelTestNew");
            //cll.OnEntered += () => cll.GetComponent<GUIText>().color = Color.blue;
            //cll.OnExited += () => cll.GetComponent<GUIText>().color = llcolor;

            //var lla = GameObject.Find("LoadLevelAdditive");
            //var llacolor = lla.GetComponent<GUIText>().color;
            //var clla = lla.AddComponent<Clicker>();

            //clla.OnClicked += () => Application.LoadLevelAdditive("LoadLevelTestAdditive");
            //clla.OnEntered += () => clla.GetComponent<GUIText>().color = Color.blue;
            //clla.OnExited += () => clla.GetComponent<GUIText>().color = llacolor;
        }
    }
}

#endif