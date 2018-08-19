
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncScrollTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<PointerEventData> onScroll;
        AsyncTriggerPromiseDictionary<PointerEventData> onScrolls;


        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(onScroll, onScrolls);
        }


        void OnScroll(PointerEventData eventData)
        {
            TrySetResult(onScroll, onScrolls, eventData);
        }


        public UniTask OnScrollAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onScroll, ref onScrolls, cancellationToken);
        }


    }
}

#endif

