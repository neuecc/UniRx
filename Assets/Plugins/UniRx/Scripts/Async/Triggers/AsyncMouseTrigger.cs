#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncMouseTrigger : MonoBehaviour
    {
        ReusablePromise onMouseDown;

        /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
        void OnMouseDown()
        {
            onMouseDown?.TryInvokeContinuation();
        }

        /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
        public UniTask OnMouseDownAsync()
        {
            return new UniTask(onMouseDown ?? (onMouseDown = new ReusablePromise()));
        }

        ReusablePromise onMouseDrag;

        /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
        void OnMouseDrag()
        {
            onMouseDrag?.TryInvokeContinuation();
        }

        /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
        public UniTask OnMouseDragAsync()
        {
            return new UniTask(onMouseDrag ?? (onMouseDrag = new ReusablePromise()));
        }

        ReusablePromise onMouseEnter;

        /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
        void OnMouseEnter()
        {
            onMouseEnter?.TryInvokeContinuation();
        }

        /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
        public UniTask OnMouseEnterAsync()
        {
            return new UniTask(onMouseEnter ?? (onMouseEnter = new ReusablePromise()));
        }

        ReusablePromise onMouseExit;

        /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
        void OnMouseExit()
        {
            onMouseExit?.TryInvokeContinuation();
        }

        /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
        public UniTask OnMouseExitAsync()
        {
            return new UniTask(onMouseExit ?? (onMouseExit = new ReusablePromise()));
        }

        ReusablePromise onMouseOver;

        /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
        void OnMouseOver()
        {
            onMouseOver?.TryInvokeContinuation();
        }

        /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
        public UniTask OnMouseOverAsync()
        {
            return new UniTask(onMouseOver ?? (onMouseOver = new ReusablePromise()));
        }

        ReusablePromise onMouseUp;

        /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
        void OnMouseUp()
        {
            onMouseUp?.TryInvokeContinuation();
        }

        /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
        public UniTask OnMouseUpAsync()
        {
            return new UniTask(onMouseUp ?? (onMouseUp = new ReusablePromise()));
        }

        ReusablePromise onMouseUpAsButton;

        /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
        void OnMouseUpAsButton()
        {
            onMouseUpAsButton?.TryInvokeContinuation();
        }

        /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
        public UniTask OnMouseUpAsButtonAsync()
        {
            return new UniTask(onMouseUpAsButton ?? (onMouseUpAsButton = new ReusablePromise()));
        }
    }
}

#endif
#endif