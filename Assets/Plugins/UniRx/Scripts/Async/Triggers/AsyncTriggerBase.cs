#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    public abstract class AsyncTriggerBase : MonoBehaviour
    {
        CancellationTokenSource cts;

        protected CancellationToken CancellationToken
        {
            get
            {
                if (cts == null) cts = new CancellationTokenSource();
                return cts.Token;
            }
        }

        protected abstract void TrySetCanceledOnDestroy();

        private void OnDestroy()
        {
            TrySetCanceledOnDestroy();
            cts?.Cancel();
        }
    }

    public abstract class AsyncTriggerBase1 : MonoBehaviour
    {
        CancellationTokenSource cts;

        protected CancellationToken CancellationToken
        {
            get
            {
                if (cts == null) cts = new CancellationTokenSource();
                return cts.Token;
            }
        }

        protected abstract ICancelablePromise Promises { get; }

        private void OnDestroy()
        {
            Promises?.TrySetCanceled();
            cts?.Cancel();
        }
    }

    public abstract class AsyncTriggerBase2 : MonoBehaviour
    {
        CancellationTokenSource cts;

        protected CancellationToken CancellationToken
        {
            get
            {
                if (cts == null) cts = new CancellationTokenSource();
                return cts.Token;
            }
        }

        protected abstract (ICancelablePromise, ICancelablePromise) Promises { get; }

        private void OnDestroy()
        {
            Promises.Item1?.TrySetCanceled();
            Promises.Item2?.TrySetCanceled();
            cts?.Cancel();
        }
    }

    public abstract class AsyncTriggerBase3 : MonoBehaviour
    {
        CancellationTokenSource cts;

        protected CancellationToken CancellationToken
        {
            get
            {
                if (cts == null) cts = new CancellationTokenSource();
                return cts.Token;
            }
        }

        protected abstract (ICancelablePromise, ICancelablePromise, ICancelablePromise) Promises { get; }

        private void OnDestroy()
        {
            Promises.Item1?.TrySetCanceled();
            Promises.Item2?.TrySetCanceled();
            Promises.Item3?.TrySetCanceled();
            cts?.Cancel();
        }
    }

    public abstract class AsyncTriggerBase4 : MonoBehaviour
    {
        CancellationTokenSource cts;

        protected CancellationToken CancellationToken
        {
            get
            {
                if (cts == null) cts = new CancellationTokenSource();
                return cts.Token;
            }
        }

        protected abstract (ICancelablePromise, ICancelablePromise, ICancelablePromise, ICancelablePromise) Promises { get; }

        private void OnDestroy()
        {
            Promises.Item1?.TrySetCanceled();
            Promises.Item2?.TrySetCanceled();
            Promises.Item3?.TrySetCanceled();
            Promises.Item4?.TrySetCanceled();
            cts?.Cancel();
        }
    }
}

#endif