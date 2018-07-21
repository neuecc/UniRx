#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncPointerUpTrigger : MonoBehaviour, IEventSystemHandler, IPointerUpHandler
    {
        ReusablePromise<PointerEventData> onPointerUp;

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnPointerUpAsync()
        {
            return new UniTask<PointerEventData>(onPointerUp ?? (onPointerUp = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif