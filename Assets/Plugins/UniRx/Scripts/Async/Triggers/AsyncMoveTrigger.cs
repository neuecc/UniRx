#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncMoveTrigger : MonoBehaviour, IEventSystemHandler, IMoveHandler
    {
        ReusablePromise<AxisEventData> onMove;

        void IMoveHandler.OnMove(AxisEventData eventData)
        {
            onMove?.TryInvokeContinuation(eventData);
        }

        public UniTask<AxisEventData> OnMoveAsync()
        {
            return new UniTask<AxisEventData>(onMove ?? (onMove = new ReusablePromise<AxisEventData>()));
        }
    }
}


#endif