#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncPointerClickTrigger : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {
        ReusablePromise<PointerEventData> onPointerClick;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnPointerClickAsync()
        {
            return new UniTask<PointerEventData>(onPointerClick ?? (onPointerClick = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif