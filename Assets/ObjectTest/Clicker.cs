using System;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class Clicker : MonoBehaviour
    {
        public event Action OnClicked = delegate { };

        public event Action OnEntered = delegate { };

        public event Action OnExited = delegate { };

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
    }
}
