using System;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class Clicker : MonoBehaviour
    {
        public event Action OnClicked = delegate { };

        public event Action OnEntered = delegate { };

        public event Action OnExited = delegate { };

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

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

        Subject<Unit> update;
        public int VVV;

        public void Update()
        {
            var t = (int)Time.time;
            if (t % 5 == 0)
            {
                VVV = t;
            }

            if (update != null) update.OnNext(Unit.Default);
        }

        public IObservable<Unit> UpdateAsObservable()
        {
            return update ?? (update = new Subject<Unit>());
        }
    }
}
