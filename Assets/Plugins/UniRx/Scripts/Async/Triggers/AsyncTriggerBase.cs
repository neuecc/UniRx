#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    public interface ICancelablePromise
    {
        CancellationToken RegisteredCancellationToken { get; }
        bool TrySetCanceled();
    }

    public class AsyncTriggerPromise<T> : ReusablePromise<T>, ICancelablePromise
    {
        public CancellationToken RegisteredCancellationToken { get; private set; }

        public AsyncTriggerPromise()
            : this(CancellationToken.None)
        {
        }

        public AsyncTriggerPromise(CancellationToken cancellationToken)
        {
            this.RegisteredCancellationToken = cancellationToken;
            TaskTracker.TrackActiveTask(this);
        }

        public override bool TrySetCanceled()
        {
            if (Status == AwaiterStatus.Canceled) return false;
            TaskTracker.RemoveTracking(this);
            return base.TrySetCanceled();
        }
    }

    public interface ICancellationTokenKeyDictionary
    {
        void Remove(CancellationToken token);
    }

    public class AsyncTriggerPromiseDictionary<TPromiseType> :
        Dictionary<CancellationToken, AsyncTriggerPromise<TPromiseType>>,
        ICancellationTokenKeyDictionary,
        IEnumerable<ICancelablePromise>
    {
        public AsyncTriggerPromiseDictionary()
            : base(CancellationTokenEqualityComparer.Default)
        {
        }

        IEnumerator<ICancelablePromise> IEnumerable<ICancelablePromise>.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        void ICancellationTokenKeyDictionary.Remove(CancellationToken token)
        {
            this.Remove(token);
        }
    }

    public abstract class AsyncTriggerBase : MonoBehaviour
    {
        static readonly Action<object> Callback = CancelCallback;

        protected abstract IEnumerable<ICancelablePromise> GetPromises();

        protected IEnumerable<ICancelablePromise> Concat(ICancelablePromise p1, IEnumerable<ICancelablePromise> p1s)
        {
            if (p1 != null) yield return p1;
            if (p1s != null) foreach (var item in p1s) yield return item;
        }

        protected IEnumerable<ICancelablePromise> Concat(
            ICancelablePromise p1, IEnumerable<ICancelablePromise> p1s,
            ICancelablePromise p2, IEnumerable<ICancelablePromise> p2s)
        {
            if (p1 != null) yield return p1;
            if (p1s != null) foreach (var item in p1s) yield return item;
            if (p2 != null) yield return p2;
            if (p2s != null) foreach (var item in p2s) yield return item;
        }

        protected IEnumerable<ICancelablePromise> Concat(
            ICancelablePromise p1, IEnumerable<ICancelablePromise> p1s,
            ICancelablePromise p2, IEnumerable<ICancelablePromise> p2s,
            ICancelablePromise p3, IEnumerable<ICancelablePromise> p3s)
        {
            if (p1 != null) yield return p1;
            if (p1s != null) foreach (var item in p1s) yield return item;
            if (p2 != null) yield return p2;
            if (p2s != null) foreach (var item in p2s) yield return item;
            if (p3 != null) yield return p3;
            if (p3s != null) foreach (var item in p3s) yield return item;
        }

        protected IEnumerable<ICancelablePromise> Concat(
            ICancelablePromise p1, IEnumerable<ICancelablePromise> p1s,
            ICancelablePromise p2, IEnumerable<ICancelablePromise> p2s,
            ICancelablePromise p3, IEnumerable<ICancelablePromise> p3s,
            ICancelablePromise p4, IEnumerable<ICancelablePromise> p4s)
        {
            if (p1 != null) yield return p1;
            if (p1s != null) foreach (var item in p1s) yield return item;
            if (p2 != null) yield return p2;
            if (p2s != null) foreach (var item in p2s) yield return item;
            if (p3 != null) yield return p3;
            if (p3s != null) foreach (var item in p3s) yield return item;
            if (p4 != null) yield return p4;
            if (p4s != null) foreach (var item in p4s) yield return item;
        }

        protected UniTask<T> GetOrAddPromise<T>(ref AsyncTriggerPromise<T> promise, ref AsyncTriggerPromiseDictionary<T> promises, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                if (promise == null) promise = new AsyncTriggerPromise<T>();
                return promise.Task;
            }

            if (promises == null) promises = new AsyncTriggerPromiseDictionary<T>();
            var cancellablePromise = new AsyncTriggerPromise<T>();
            promises.Add(cancellationToken, cancellablePromise);
            cancellationToken.Register(Callback, Tuple.Create(promises, cancellablePromise));
            return cancellablePromise.Task;
        }

        static void CancelCallback(object state)
        {
            var tuple = (Tuple<ICancellationTokenKeyDictionary, ICancelablePromise>)state;
            var dict = tuple.Item1;
            var promise = tuple.Item2;

            promise.TrySetCanceled();
            dict.Remove(promise.RegisteredCancellationToken);
        }

        protected void TrySetResult<T>(ReusablePromise<T> promise, AsyncTriggerPromiseDictionary<T> promises, T value)
        {
            if (promise != null)
            {
                promise.TrySetResult(value);
            }
            if (promises != null)
            {
                foreach (var item in promises.Values)
                {
                    item.TrySetResult(value);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var item in GetPromises())
            {
                item.TrySetCanceled();
            }
        }
    }
}

#endif