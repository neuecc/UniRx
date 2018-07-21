#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncScrollTrigger : MonoBehaviour, IEventSystemHandler, IScrollHandler
    {
        ReusablePromise<PointerEventData> onScroll;

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            onScroll?.TryInvokeContinuation(eventData);
        }

        public UniTask<PointerEventData> OnScrollAsync()
        {
            return new UniTask<PointerEventData>(onScroll ?? (onScroll = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif