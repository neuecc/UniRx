#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncPointerEnterTrigger : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler
    {
        ReusablePromise<PointerEventData> onPointerEnter;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnPointerEnterAsync()
        {
            return new UniTask<PointerEventData>(onPointerEnter ?? (onPointerEnter = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif