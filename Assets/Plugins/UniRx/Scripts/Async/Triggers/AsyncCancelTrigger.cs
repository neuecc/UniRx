#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncCancelTrigger : MonoBehaviour, IEventSystemHandler, ICancelHandler
    {
        ReusablePromise<BaseEventData> onCancel;

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            onCancel?.TrySetResult(eventData);
        }

        public UniTask<BaseEventData> OnCancelAsync()
        {
            return new UniTask<BaseEventData>(onCancel ?? (onCancel = new ReusablePromise<BaseEventData>()));
        }

        void OnDestroy()
        {
            onCancel?.TrySetCanceled();
        }
    }
}


#endif