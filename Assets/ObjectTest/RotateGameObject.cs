using System;
using UniRx;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class RotateGameObject : MonoBehaviour
    {
        public GameObject target;
        public Vector3 angle;

        void Awake()
        {
            if (angle == Vector3.zero)
            {
                angle = new Vector3(2, 4, 4);
            }
        }

        void Start()
        {
            var interval = Observable
                .Interval(System.TimeSpan.FromMilliseconds(20))
                .Do((l) => target.transform.Rotate(angle));

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
        }
    }
}