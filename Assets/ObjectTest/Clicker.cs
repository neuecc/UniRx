using System;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class Clicker : MonoBehaviour
    {
        public event Action OnClicked = delegate { };

        public event Action OnEntered = delegate { };

        public event Action OnExited = delegate { };

#if !UNITY_ANDROID

        // Disable OnMouse_ event handlers to make it easy to confirm warning.

        void OnMouseDown()
        {
            OnClicked();
        }

        void OnMouseEnter()
        {
            OnEntered();
        }

        void OnMouseExit()
        {
            OnExited();
        }

#endif

    }
}
