#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncDropTrigger : MonoBehaviour, IEventSystemHandler, IDropHandler
    {
        ReusablePromise<PointerEventData> onDrop;

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            onDrop?.TrySetResult(eventData);
        }

        public UniTask<PointerEventData> OnDropAsync()
        {
            return new UniTask<PointerEventData>(onDrop ?? (onDrop = new ReusablePromise<PointerEventData>()));
        }
    }
}


#endif