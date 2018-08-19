
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncPointerClickTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<PointerEventData> onPointerClick;
        AsyncTriggerPromiseDictionary<PointerEventData> onPointerClicks;


        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(onPointerClick, onPointerClicks);
        }


        void OnPointerClick(PointerEventData eventData)
        {
            TrySetResult(onPointerClick, onPointerClicks, eventData);
        }


        public UniTask OnPointerClickAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onPointerClick, ref onPointerClicks, cancellationToken);
        }


    }
}

#endif

