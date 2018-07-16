#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace UniRx.Async
{
    public interface IPromise<T>
    {
        bool IsCompleted { get; }
        T GetResult();
        void RegisterContinuation(Action action);
    }

    public interface IResolve<T>
    {
        void SetResult(T value);
    }

    public interface IReject
    {
        void SetException(Exception ex);
    }

    public class Promise<T> : IPromise<T>, IResolve<T>, IReject
    {
        const int Pending = 0;
        const int FulFilled = 1;
        const int Rejected = 2;

        int state = 0;
        T value;
        ExceptionDispatchInfo exception;
        object continuation; // action or list

        public bool IsCompleted => state != Pending;

        public Promise()
        {

        }

        public Promise(Action<IResolve<T>, IReject> executor)
        {
            try
            {
                executor(this, this);
            }
            catch (Exception ex)
            {
                SetException(ex);
            }
        }

        public Promise(Action<IResolve<T>, IReject, object> executor, object state)
        {
            try
            {
                executor(this, this, state);
            }
            catch (Exception ex)
            {
                SetException(ex);
            }
        }

        public T GetResult()
        {
            if (state == FulFilled)
            {
                return value;
            }
            else if (state == Rejected)
            {
                if (exception == null)
                {
                    exception = ExceptionDispatchInfo.Capture(new OperationCanceledException());
                }

                exception.Throw();
            }
            else
            {
                Interlocked.Exchange(ref continuation, CreateNewContinuation(continuation, out var mres));
                mres.Wait();
                return GetResult();
            }
            return default(T);
        }

        public void RegisterContinuation(Action action)
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

        public void SetResult(T value)
        {
            if (Interlocked.CompareExchange(ref state, FulFilled, Pending) == Pending)
            {
                this.value = value;
                TryInvokeContinuation();
            }
        }

        public void SetException(Exception exception)
        {
            if (Interlocked.CompareExchange(ref state, Rejected, Pending) == Pending)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
                TryInvokeContinuation();
            }
        }

        public void SetCanceled()
        {
            if (Interlocked.CompareExchange(ref state, Rejected, Pending) == Pending)
            {
                TryInvokeContinuation();
            }
        }

        Action CreateNewContinuation(object currentContinuation, out ManualResetEventSlim mres)
        {
            var mresEx = mres = new ManualResetEventSlim();
            Action newContinuation = () =>
            {
                if (currentContinuation != null)
                {
                    if (currentContinuation is Action)
                    {
                        ((Action)currentContinuation).Invoke();
                    }
                    else
                    {
                        var l = (List<Action>)currentContinuation;
                        var cnt = l.Count;
                        for (int i = 0; i < cnt; i++)
                        {
                            l[i].Invoke();
                        }
                    }
                }
                mresEx.Set();
            };
            return newContinuation;
        }
    }

    internal sealed class LazyPromise<T> : IPromise<T>
    {
        Func<UniTask<T>> factory;
        UniTask<T> value;

        public LazyPromise(Func<UniTask<T>> factory)
        {
            this.factory = factory;
        }

        void Create()
        {
            var f = Interlocked.Exchange(ref factory, null);
            if (f != null)
            {
                value = f();
            }
        }

        public bool IsCompleted
        {
            get
            {
                Create();
                return value.IsCompleted;
            }
        }

        public T GetResult()
        {
            Create();
            return value.GetResult();
        }

        public void RegisterContinuation(Action continuation)
        {
            Create();
            value.GetAwaiter().UnsafeOnCompleted(continuation);
        }
    }
}

#endif