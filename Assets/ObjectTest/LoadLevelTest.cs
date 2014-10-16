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
            MainThreadDispatcher.IsCullingEnabled = true;
            Debug.Log("MainThreadDispatcher.IsCullingEnabled = " + MainThreadDispatcher.IsCullingEnabled.ToString());

            var g = GameObject.Find("Cylinder");

            var interval = Observable
                .Interval(System.TimeSpan.FromMilliseconds(20));

            interval.CatchIgnore();

            interval.Subscribe((s) =>
            {
                if (g != null)
                    g.transform.Rotate(angle);
                else
                    throw new Exception("Can't find GameObject `Cylinder`!");
            });

            var ll = GameObject.Find("LoadLevelButton");
            var cll = ll.AddComponent<Clicker>();
            cll.OnClicked += () => Application.LoadLevel("LoadLevelTestNew");

            var lla = GameObject.Find("LoadLevelAdditiveButton");
            var clla = lla.AddComponent<Clicker>();
            clla.OnClicked += () => Application.LoadLevelAdditive("LoadLevelTestAdditive");
        }
    }
}