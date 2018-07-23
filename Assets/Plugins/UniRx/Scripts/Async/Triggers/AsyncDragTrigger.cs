#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncDragTrigger : MonoBehaviour, IEventSystemHandler, IDragHandler
    {
        ReusablePromise<PointerEventData> onDrag;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            onDrag?.TrySetResult(eventData);
        }

        public UniTask<PointerEventData> OnDragAsync()
        {
            return new UniTask<PointerEventData>(onDrag ?? (onDrag = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif