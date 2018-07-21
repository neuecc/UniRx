#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncPointerExitTrigger : MonoBehaviour, IEventSystemHandler, IPointerExitHandler
    {
        ReusablePromise<PointerEventData> onPointerExit;

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnPointerExitAsync()
        {
            return new UniTask<PointerEventData>(onPointerExit ?? (onPointerExit = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif