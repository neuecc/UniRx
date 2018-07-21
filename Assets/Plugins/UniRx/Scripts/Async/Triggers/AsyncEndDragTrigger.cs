#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncEndDragTrigger : MonoBehaviour, IEventSystemHandler, IEndDragHandler
    {
        ReusablePromise<PointerEventData> onEndDrag;

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnEndDragAsync()
        {
            return new UniTask<PointerEventData>(onEndDrag ?? (onEndDrag = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif