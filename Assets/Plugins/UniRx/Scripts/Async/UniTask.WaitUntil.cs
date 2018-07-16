#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask WaitUntil(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitUntilPromise(predicate, cancellationToken);
            PlayerLoopHelper.AddAction(timing, promise);
            return promise.Task;
        }

        public static UniTask WaitWhile(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitWhilePromise(predicate, cancellationToken);
            PlayerLoopHelper.AddAction(timing, promise);
            return promise.Task;
        }

        public UniTask<U> WaitUntilValueChanged<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
          where T : class
        {
            var promise = new WaitUntilValueChangedAwaiter<T, U>(target, monitorFunction, null);
            PlayerLoopHelper.AddAction(monitorTiming, promise);
            return promise.Task;
        }

        class WaitUntilPromise : Promise<AsyncUnit>, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            CancellationToken cancellation;

            public UniTask Task => new UniTask(this);

            public WaitUntilPromise(Func<bool> predicate, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.cancellation = cancellation;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    SetException(ex);
                    return false;
                }

                if (result)
                {
                    SetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }

        class WaitWhilePromise : Promise<AsyncUnit>, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            CancellationToken cancellation;

            public UniTask Task => new UniTask(this);

            public WaitWhilePromise(Func<bool> predicate, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.cancellation = cancellation;
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }


                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    SetException(ex);
                    return false;
                }

                if (!result)
                {
                    SetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }
        }

        class WaitUntilValueChangedAwaiter<T, U> : Promise<U>, IPlayerLoopItem
          where T : class
        {
            readonly T target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly U initialValue;
            CancellationToken cancellation;

            public UniTask<U> Task => new UniTask<U>(this);

            public WaitUntilValueChangedAwaiter(T target, Func<T, U> monitorFunction, IEqualityComparer<U> equalityComparer)
            {
                this.target = target;
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer;
                this.initialValue = monitorFunction(target);
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    SetCanceled();
                    return false;
                }

                U nextValue = default(U);
                try
                {
                    nextValue = monitorFunction(target);
                    if (equalityComparer.Equals(initialValue, nextValue))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    SetException(ex);
                    return false;
                }

                SetResult(nextValue);
                return false;
            }
        }
    }
}
#endif