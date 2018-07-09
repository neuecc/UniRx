#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UniRx.Async.CompilerServices;

namespace UniRx.Async
{
    /// <summary>
    /// Lightweight unity specified task-like object.
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncUniTaskMethodBuilder))]
    public partial struct UniTask : IEquatable<UniTask>
    {
        readonly IPromise<AsyncUnit> continuation;

        [DebuggerHidden]
        public UniTask(IPromise<AsyncUnit> continuation)
        {

            this.continuation = continuation;
        }

        [DebuggerHidden]
        public UniTask(Func<UniTask<AsyncUnit>> factory)
        {
            this.continuation = new LazyPromise<AsyncUnit>(factory);
        }

        [DebuggerHidden]
        public bool IsCompleted
        {
            get
            {
                return continuation == null ? true : continuation.IsCompleted;
            }
        }

        [DebuggerHidden]
        internal void GetResult()
        {
            if (continuation == null)
            {
            }
            else
            {
                continuation.GetResult();
            }
        }

        [DebuggerHidden]
        public Awaiter GetAwaiter()
        {
            return new Awaiter(this);
        }

        public bool Equals(UniTask other)
        {
            if (this.continuation == null && other.continuation == null)
            {
                return true;
            }
            else if (this.continuation != null && other.continuation != null)
            {
                return this.continuation == other.continuation;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (this.continuation == null)
            {
                return 0;
            }
            else
            {
                return this.continuation.GetHashCode();
            }
        }

        public struct Awaiter : ICriticalNotifyCompletion
        {
            readonly UniTask task;

            [DebuggerHidden]
            public Awaiter(UniTask task)
            {
                this.task = task;
            }

            [DebuggerHidden]
            public bool IsCompleted => task.IsCompleted;

            [DebuggerHidden]
            public void GetResult() => task.GetResult();

            [DebuggerHidden]
            public void OnCompleted(Action continuation)
            {
                if (task.continuation != null)
                {
                    task.continuation.RegisterContinuation(continuation);
                }
                else
                {
                    continuation();
                }
            }

            [DebuggerHidden]
            public void UnsafeOnCompleted(Action continuation)
            {
                if (task.continuation != null)
                {
                    task.continuation.RegisterContinuation(continuation);
                }
                else
                {
                    continuation();
                }
            }
        }
    }

    /// <summary>
    /// Lightweight unity specified task-like object.
    /// </summary>
    [AsyncMethodBuilder(typeof(AsyncUniTaskMethodBuilder<>))]
    public struct UniTask<T> : IEquatable<UniTask<T>>
    {
        readonly T result;
        readonly IPromise<T> continuation;

        [DebuggerHidden]
        public UniTask(T result)
        {
            this.result = result;
            this.continuation = null;
        }

        [DebuggerHidden]
        public UniTask(IPromise<T> continuation)
        {
            this.result = default(T);
            this.continuation = continuation;
        }

        [DebuggerHidden]
        public UniTask(Func<UniTask<T>> factory)
        {
            this.result = default(T);
            this.continuation = new LazyPromise<T>(factory);
        }

        [DebuggerHidden]
        public bool IsCompleted
        {
            get
            {
                return continuation == null ? true : continuation.IsCompleted;
            }
        }

        [DebuggerHidden]
        internal T GetResult()
        {
            if (continuation == null)
            {
                return result;
            }
            else
            {
                return continuation.GetResult();
            }
        }

        [DebuggerHidden]
        public Awaiter GetAwaiter()
        {
            return new Awaiter(this);
        }

        public bool Equals(UniTask<T> other)
        {
            if (this.continuation == null && other.continuation == null)
            {
                return EqualityComparer<T>.Default.Equals(this.result, other.result);
            }
            else if (this.continuation != null && other.continuation != null)
            {
                return this.continuation == other.continuation;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (this.continuation == null)
            {
                if (result == null)
                    return 0;
                return result.GetHashCode();
            }
            else
            {
                return this.continuation.GetHashCode();
            }
        }

        public struct Awaiter : ICriticalNotifyCompletion
        {
            readonly UniTask<T> task;

            [DebuggerHidden]
            public Awaiter(UniTask<T> task)
            {
                this.task = task;
            }

            [DebuggerHidden]
            public bool IsCompleted => task.IsCompleted;

            [DebuggerHidden]
            public T GetResult() => task.GetResult();

            [DebuggerHidden]
            public void OnCompleted(Action continuation)
            {
                if (task.continuation != null)
                {
                    task.continuation.RegisterContinuation(continuation);
                }
                else
                {
                    continuation();
                }
            }

            [DebuggerHidden]
            public void UnsafeOnCompleted(Action continuation)
            {
                if (task.continuation != null)
                {
                    task.continuation.RegisterContinuation(continuation);
                }
                else
                {
                    continuation();
                }
            }
        }
    }
}
#endif