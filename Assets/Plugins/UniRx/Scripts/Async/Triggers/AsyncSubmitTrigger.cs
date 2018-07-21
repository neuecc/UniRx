#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncSubmitTrigger : MonoBehaviour, IEventSystemHandler, ISubmitHandler
    {
        ReusablePromise<BaseEventData> onSubmit;

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            onSubmit?.TryInvokeContinuation(eventData);
        }

        public UniTask<BaseEventData> OnSubmitAsync()
        {
            return new UniTask<BaseEventData>(onSubmit ?? (onSubmit = new ReusablePromise<BaseEventData>()));
        }
    }
}


#endif