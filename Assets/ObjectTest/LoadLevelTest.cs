using System;
using System.Collections;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class LoadLevelTest : MonoBehaviour
    {
        void Start()
        {
            var ll = GameObject.Find("LoadLevel");
            var llcolor = ll.guiText.color;
            var cll = ll.AddComponent<Clicker>();
            cll.OnClicked += () => Application.LoadLevel("LoadLevelTestNew");
            cll.OnEntered += () => cll.guiText.color = Color.blue;
            cll.OnExited += () => cll.guiText.color = llcolor;

            var lla = GameObject.Find("LoadLevelAdditive");
            var llacolor = lla.guiText.color;
            var clla = lla.AddComponent<Clicker>();
            clla.OnClicked += () => Application.LoadLevelAdditive("LoadLevelTestAdditive");
            clla.OnEntered += () => clla.guiText.color = Color.blue;
            clla.OnExited += () => clla.guiText.color = llacolor;
        }
    }
}