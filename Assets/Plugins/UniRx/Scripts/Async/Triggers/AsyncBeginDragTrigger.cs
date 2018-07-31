#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncBeginDragTrigger : MonoBehaviour, IEventSystemHandler, IBeginDragHandler
    {
        ReusablePromise<PointerEventData> onBeginDrag;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.TrySetResult(eventData);
        }

        public UniTask<PointerEventData> OnBeginDragAsync()
        {
            return new UniTask<PointerEventData>(onBeginDrag ?? (onBeginDrag = new ReusablePromise<PointerEventData>()));
        }

        void OnDestroy()
        {
            onBeginDrag?.TrySetCanceled();
        }
    }
}


#endif