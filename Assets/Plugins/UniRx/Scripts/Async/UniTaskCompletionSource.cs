#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using UniRx.Async.Internal;

namespace UniRx.Async
{
    public class UniTaskCompletionSource : IAwaiter
    {
        // State(= AwaiterStatus)
        const int Pending = 0;
        const int Succeeded = 1;
        const int Faulted = 2;
        const int Canceled = 3;

        static readonly Action<object> cancellationCallbackDelegate = new Action<object>(CancellationCallback);
        CancellationToken cancellationToken; // used for SetCancelation.

        int state = 0;
        ExceptionDispatchInfo exception;
        object continuation; // action or list

        AwaiterStatus IAwaiter.Status => (AwaiterStatus)state;

        bool IAwaiter.IsCompleted => state != Pending;

        public UniTask Task => new UniTask(this);

        void IAwaiter.GetResult()
        {
            if (state == Succeeded)
            {
                return;
            }
            else if (state == Faulted)
            {
                exception.Throw();
            }
            else if (state == Canceled)
            {
                if (exception == null)
                {
                    exception = ExceptionDispatchInfo.Capture(new OperationCanceledException());
                }
                exception.Throw();
            }
            else // Pending
            {
                throw new NotSupportedException("UniTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action action)
        {
            if (Interlocked.CompareExchange(ref continuation, (object)action, null) == null)
            {
                if (state != Pending)
                {
                    TryInvokeContinuation();
                }
            }
            else
            {
                var c = continuation;
                if (c is Action)
                {
                    var list = new List<Action>();
                    list.Add((Action)c);
                    list.Add(action);
                    if (Interlocked.CompareExchange(ref continuation, list, c) == c)
                    {
                        goto TRYINVOKE;
                    }
                }

                var l = (List<Action>)continuation;
                lock (l)
                {
                    l.Add(action);
                }

                TRYINVOKE:
                if (state != Pending)
                {
                    TryInvokeContinuation();
                }
            }
        }

        void TryInvokeContinuation()
        {
            var c = Interlocked.Exchange(ref continuation, null);
            if (c != null)
            {
                if (c is Action)
                {
                    ((Action)c).Invoke();
                }
                else
                {
                    var l = (List<Action>)c;
                    var cnt = l.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        l[i].Invoke();
                    }
                }
            }
        }

        public bool TrySetResult()
        {
            if (Interlocked.CompareExchange(ref state, Succeeded, Pending) == Pending)
            {
                TryInvokeContinuation();
                return true;
            }
            return false;
        }

        public bool TrySetException(Exception exception)
        {
            if (Interlocked.CompareExchange(ref state, Faulted, Pending) == Pending)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
                TryInvokeContinuation();
                return true;
            }
            return false;
        }

        public bool TrySetCanceled()
        {
            if (Interlocked.CompareExchange(ref state, Canceled, Pending) == Pending)
            {
                TryInvokeContinuation();
                return true;
            }
            return false;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(continuation);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            if (CancellationTokenHelper.TrySetOrLinkCancellationToken(ref this.cancellationToken, token))
            {
                this.cancellationToken.Register(cancellationCallbackDelegate, this, false);
            }
        }

        static void CancellationCallback(object state)
        {
            var source = (UniTaskCompletionSource)state;
            source.TrySetCanceled();
        }
    }

    public class UniTaskCompletionSource<T> : IAwaiter<T>
    {
        // State(= AwaiterStatus)
        const int Pending = 0;
        const int Succeeded = 1;
        const int Faulted = 2;
        const int Canceled = 3;

        static readonly Action<object> cancellationCallbackDelegate = new Action<object>(CancellationCallback);
        CancellationToken cancellationToken; // used for SetCancelation.

        int state = 0;
        T value;
        ExceptionDispatchInfo exception;
        object continuation; // action or list

        bool IAwaiter.IsCompleted => state != Pending;

        public UniTask<T> Task => new UniTask<T>(this);
        public UniTask UnitTask => new UniTask(this);

        AwaiterStatus IAwaiter.Status => (AwaiterStatus)state;

        T IAwaiter<T>.GetResult()
        {
            if (state == Succeeded)
            {
                return value;
            }
            else if (state == Faulted)
            {
                exception.Throw();
            }
            else if (state == Canceled)
            {
                if (exception == null)
                {
                    // lazy create to reduce cancellation cost.
                    exception = ExceptionDispatchInfo.Capture(new OperationCanceledException());
                }
                exception.Throw();
            }
            else // Pending
            {
                throw new NotSupportedException("UniTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }

            return default(T);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action action)
        {
            if (Interlocked.CompareExchange(ref continuation, (object)action, null) == null)
            {
                if (state != Pending)
                {
                    TryInvokeContinuation();
                }
            }
            else
            {
                var c = continuation;
                if (c is Action)
                {
                    var list = new List<Action>();
                    list.Add((Action)c);
                    list.Add(action);
                    if (Interlocked.CompareExchange(ref continuation, list, c) == c)
                    {
                        goto TRYINVOKE;
                    }
                }

                var l = (List<Action>)continuation;
                lock (l)
                {
                    l.Add(action);
                }

                TRYINVOKE:
                if (state != Pending)
                {
                    TryInvokeContinuation();
                }
            }
        }

        void TryInvokeContinuation()
        {
            var c = Interlocked.Exchange(ref continuation, null);
            if (c != null)
            {
                if (c is Action)
                {
                    ((Action)c).Invoke();
                }
                else
                {
                    var l = (List<Action>)c;
                    var cnt = l.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        l[i].Invoke();
                    }
                }
            }
        }

        public bool TrySetResult(T value)
        {
            if (Interlocked.CompareExchange(ref state, Succeeded, Pending) == Pending)
            {
                this.value = value;
                TryInvokeContinuation();
                return true;
            }
            return false;
        }

        public bool TrySetException(Exception exception)
        {
            if (Interlocked.CompareExchange(ref state, Faulted, Pending) == Pending)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
                TryInvokeContinuation();
                return true;
            }
            return false;
        }

        public bool TrySetCanceled()
        {
            if (Interlocked.CompareExchange(ref state, Canceled, Pending) == Pending)
            {
                TryInvokeContinuation();
                return true;
            }
            return false;
        }

        void IAwaiter.GetResult()
        {
            ((IAwaiter<T>)this).GetResult();
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion)this).UnsafeOnCompleted(continuation);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            if (CancellationTokenHelper.TrySetOrLinkCancellationToken(ref this.cancellationToken, token))
            {
                this.cancellationToken.Register(cancellationCallbackDelegate, this, false);
            }
        }

        static void CancellationCallback(object state)
        {
            var source = (UniTaskCompletionSource<T>)state;
            source.TrySetCanceled();
        }
    }
}

#endif