#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace UniRx.Async.Internal
{
    internal sealed class LazyPromise : IAwaiter
    {
        Func<UniTask> factory;
        UniTask value;
        CancellationToken token;

        public LazyPromise(Func<UniTask> factory)
        {
            this.factory = factory;
        }

        void Create()
        {
            var f = Interlocked.Exchange(ref factory, null);
            if (f != null)
            {
                value = f();
                if (token != CancellationToken.None)
                {
                    value.GetAwaiter().SetCancellationToken(token);
                }
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

        public AwaiterStatus Status
        {
            get
            {
                {
                    Create();
                    return value.Status;
                }
            }
        }

        public void GetResult()
        {
            Create();
            value.GetResult();
        }

        void IAwaiter.GetResult()
        {
            GetResult();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            Create();
            value.GetAwaiter().UnsafeOnCompleted(continuation);
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            if (factory == null)
            {
                value.GetAwaiter().SetCancellationToken(token);
            }
            else
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref this.token, token);
            }
        }
    }

    internal sealed class LazyPromise<T> : IAwaiter<T>
    {
        Func<UniTask<T>> factory;
        UniTask<T> value;
        CancellationToken token;

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
                if (token != CancellationToken.None)
                {
                    value.GetAwaiter().SetCancellationToken(token);
                }
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

        public AwaiterStatus Status
        {
            get
            {
                {
                    Create();
                    return value.Status;
                }
            }
        }

        public T GetResult()
        {
            Create();
            return value.Result;
        }

        void IAwaiter.GetResult()
        {
            GetResult();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            Create();
            value.GetAwaiter().UnsafeOnCompleted(continuation);
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            if (factory == null)
            {
                value.GetAwaiter().SetCancellationToken(token);
            }
            else
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref this.token, token);
            }
        }
    }

    // TODO:Nanika Kaku...!
    internal sealed class RetriableLazyPromise : IAwaiter
    {
        Func<UniTask> factory;
        UniTask value;
        CancellationToken token;

        public RetriableLazyPromise(Func<UniTask> factory)
        {
            this.factory = factory;
        }

        void Create()
        {
            var f = Interlocked.Exchange(ref factory, null);
            if (f != null)
            {
                value = f();
                if (token != CancellationToken.None)
                {
                    value.GetAwaiter().SetCancellationToken(token);
                }
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

        public AwaiterStatus Status
        {
            get
            {
                {
                    Create();
                    return value.Status;
                }
            }
        }

        public void GetResult()
        {
            if (Status == AwaiterStatus.Succeeded)
            {
                factory = null; // complete.
            }
            else
            {
                value = default(UniTask);
            }
            value.GetResult();
        }

        void IAwaiter.GetResult()
        {
            GetResult();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            Create();
            value.GetAwaiter().UnsafeOnCompleted(continuation);
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            if (factory == null)
            {
                value.GetAwaiter().SetCancellationToken(token);
            }
            else
            {
                CancellationTokenHelper.TrySetOrLinkCancellationToken(ref this.token, token);
            }
        }
    }
}

#endif