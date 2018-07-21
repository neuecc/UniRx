#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncDeselectTrigger : MonoBehaviour, IEventSystemHandler, IDeselectHandler
    {
        ReusablePromise<BaseEventData> onDeselect;

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            onDeselect?.TryInvokeContinuation(eventData);
        }

        public UniTask<BaseEventData> OnDeselectAsync()
        {
            return new UniTask<BaseEventData>(onDeselect ?? (onDeselect = new ReusablePromise<BaseEventData>()));
        }
    }
}


#endif