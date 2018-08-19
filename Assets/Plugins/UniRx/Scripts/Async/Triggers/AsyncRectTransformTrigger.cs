
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncRectTransformTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<AsyncUnit> onRectTransformDimensionsChange;
        AsyncTriggerPromiseDictionary<AsyncUnit> onRectTransformDimensionsChanges;
        AsyncTriggerPromise<AsyncUnit> onRectTransformRemoved;
        AsyncTriggerPromiseDictionary<AsyncUnit> onRectTransformRemoveds;


        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(onRectTransformDimensionsChange, onRectTransformDimensionsChanges, onRectTransformRemoved, onRectTransformRemoveds);
        }


        void OnRectTransformDimensionsChange()
        {
            TrySetResult(onRectTransformDimensionsChange, onRectTransformDimensionsChanges, AsyncUnit.Default);
        }


        public UniTask OnRectTransformDimensionsChangeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onRectTransformDimensionsChange, ref onRectTransformDimensionsChanges, cancellationToken);
        }


        void OnRectTransformRemoved()
        {
            TrySetResult(onRectTransformRemoved, onRectTransformRemoveds, AsyncUnit.Default);
        }


        public UniTask OnRectTransformRemovedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onRectTransformRemoved, ref onRectTransformRemoveds, cancellationToken);
        }


    }
}

#endif

