using System;
using System.Collections;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class LoadLevelTest : MonoBehaviour
    {
        Vector3 angle = new Vector3(2, 4, 4);

        void Start()
        {
            var g = GameObject.Find("Cylinder");

            var interval = Observable
                .Interval(System.TimeSpan.FromMilliseconds(20))
                .Do((l) => g.transform.Rotate(angle));

            interval
                .CatchIgnore((Exception ex) => Debug.LogWarning(ex))
                .Subscribe();
            /*
                .Subscribe((s) =>
            {
                if (g != null)
                    g.transform.Rotate(angle);
                else
                    throw new Exception("Can't find GameObject `Cylinder`!");
            });
             */

            var ll = GameObject.Find("LoadLevel");
            var cll = ll.AddComponent<Clicker>();
            cll.OnClicked += () => Application.LoadLevel("LoadLevelTestNew");
            cll.OnEntered += () => cll.guiText.color = Color.blue;
            cll.OnExited += () => cll.guiText.color = Color.white;

            var lla = GameObject.Find("LoadLevelAdditive");
            var clla = lla.AddComponent<Clicker>();
            clla.OnClicked += () => Application.LoadLevelAdditive("LoadLevelTestAdditive");
            clla.OnEntered += () => clla.guiText.color = Color.blue;
            clla.OnExited += () => clla.guiText.color = Color.white;
        }
    }
}