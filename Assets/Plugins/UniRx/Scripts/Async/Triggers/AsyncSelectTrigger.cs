#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncSelectTrigger : MonoBehaviour, IEventSystemHandler, ISelectHandler
    {
        ReusablePromise<BaseEventData> onSelect;

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            onSelect?.TrySetResult(eventData);
        }

        public UniTask<BaseEventData> OnSelectAsync()
        {
            return new UniTask<BaseEventData>(onSelect ?? (onSelect = new ReusablePromise<BaseEventData>()));
        }
    }
}


#endif