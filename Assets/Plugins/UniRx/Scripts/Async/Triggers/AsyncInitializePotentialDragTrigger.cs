#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncInitializePotentialDragTrigger : MonoBehaviour, IEventSystemHandler, IInitializePotentialDragHandler
    {
        ReusablePromise<PointerEventData> onInitializePotentialDrag;

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            onInitializePotentialDrag?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnInitializePotentialDragAsync()
        {
            return new UniTask<PointerEventData>(onInitializePotentialDrag ?? (onInitializePotentialDrag = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif