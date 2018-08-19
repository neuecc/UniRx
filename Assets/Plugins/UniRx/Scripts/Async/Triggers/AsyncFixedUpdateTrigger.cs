
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncFixedUpdateTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<AsyncUnit> fixedUpdate;
        AsyncTriggerPromiseDictionary<AsyncUnit> fixedUpdates;


        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(fixedUpdate, fixedUpdates);
        }


        void FixedUpdate()
        {
            TrySetResult(fixedUpdate, fixedUpdates, AsyncUnit.Default);
        }


        public UniTask FixedUpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref fixedUpdate, ref fixedUpdates, cancellationToken);
        }


    }
}

#endif

