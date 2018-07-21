#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncUpdateSelectedTrigger : MonoBehaviour, IEventSystemHandler, IUpdateSelectedHandler
    {
        ReusablePromise<BaseEventData> onUpdateSelected;

        void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelected?.TryInvokeContinuation(eventData);
        }

        public UniTask<BaseEventData> OnUpdateSelectedAsync()
        {
            return new UniTask<BaseEventData>(onUpdateSelected ?? (onUpdateSelected = new ReusablePromise<BaseEventData>()));
        }
    }
}


#endif