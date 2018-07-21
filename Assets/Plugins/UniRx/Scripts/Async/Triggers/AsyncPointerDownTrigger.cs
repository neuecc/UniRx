#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncPointerDownTrigger : MonoBehaviour, IEventSystemHandler, IPointerDownHandler
    {
        ReusablePromise<PointerEventData> onPointerDown;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnPointerDownAsync()
        {
            return new UniTask<PointerEventData>(onPointerDown ?? (onPointerDown = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif