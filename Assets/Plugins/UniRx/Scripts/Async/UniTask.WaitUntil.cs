#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask WaitUntil(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitUntilPromise(predicate, timing, cancellationToken);
            return promise.Task;
        }

        public static UniTask WaitWhile(Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
        {
            var promise = new WaitWhilePromise(predicate, timing, cancellationToken);
            return promise.Task;
        }

        public static UniTask<U> WaitUntilValueChanged<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = null, CancellationToken cancellationToken = default(CancellationToken))
          where T : class
        {
            var unityObject = target as UnityEngine.Object;
            var isUnityObject = !object.ReferenceEquals(target, null); // don't use (unityObject == null)

            if (isUnityObject)
            {
                return new WaitUntilValueChangedUnityObjectPromise<T, U>(target, monitorFunction, monitorTiming, equalityComparer, cancellationToken).Task;
            }
            else
            {
                return new WaitUntilValueChangedStandardObjectPromise<T, U>(target, monitorFunction, monitorTiming, equalityComparer, cancellationToken).Task;
            }
        }

        public static UniTask<(bool isDestroyed, U value)> WaitUntilValueChangedWithIsDestroyed<T, U>(T target, Func<T, U> monitorFunction, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = null, CancellationToken cancellationToken = default(CancellationToken))
          where T : UnityEngine.Object
        {
            return new WaitUntilValueChangedUnityObjectWithReturnIsDestoryedPromise<T, U>(target, monitorFunction, monitorTiming, equalityComparer, cancellationToken).Task;
        }

        class WaitUntilPromise : ReusablePromise, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            readonly PlayerLoopTiming timing;
            CancellationToken cancellation;
            bool isRunning = false;

            public WaitUntilPromise(Func<bool> predicate, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.timing = timing;
                this.cancellation = cancellation;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }

                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    isRunning = false;
                    TrySetException(ex);
                    return false;
                }

                if (result)
                {
                    isRunning = false;
                    TrySetResult();
                    return false;
                }

                return true;
            }
        }

        class WaitWhilePromise : ReusablePromise, IPlayerLoopItem
        {
            readonly Func<bool> predicate;
            readonly PlayerLoopTiming timing;
            CancellationToken cancellation;
            bool isRunning = false;

            public WaitWhilePromise(Func<bool> predicate, PlayerLoopTiming timing, CancellationToken cancellation)
            {
                this.predicate = predicate;
                this.timing = timing;
                this.cancellation = cancellation;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }

                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }


                bool result = default(bool);
                try
                {
                    result = predicate();
                }
                catch (Exception ex)
                {
                    isRunning = false;
                    TrySetException(ex);
                    return false;
                }

                if (!result)
                {
                    isRunning = false;
                    TrySetResult();
                    return false;
                }

                return true;
            }
        }

        class WaitUntilValueChangedUnityObjectPromise<T, U> : ReusablePromise<U>, IPlayerLoopItem
        {
            readonly T target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly PlayerLoopTiming timing;
            U currentValue;
            CancellationToken cancellation;
            bool isRunning = false;

            public WaitUntilValueChangedUnityObjectPromise(T target, Func<T, U> monitorFunction, PlayerLoopTiming timing, IEqualityComparer<U> equalityComparer, CancellationToken cancellation)
            {
                this.target = target;
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UnityEqualityComparer.GetDefault<U>();
                this.currentValue = monitorFunction(target);
                this.cancellation = cancellation;
                this.timing = timing;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }

                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                U nextValue = default(U);

                if (target != null)
                {
                    try
                    {
                        nextValue = monitorFunction(target);
                        if (equalityComparer.Equals(currentValue, nextValue))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isRunning = false;
                        TrySetException(ex);
                        return false;
                    }
                }
                else
                {
                    isRunning = false;
                    TrySetException(new InvalidOperationException("Monitoring target is already destoyed."));
                    return false;
                }

                isRunning = false;
                currentValue = nextValue;
                TrySetResult(nextValue);
                return false;
            }
        }

        class WaitUntilValueChangedUnityObjectWithReturnIsDestoryedPromise<T, U> : ReusablePromise<(bool, U)>, IPlayerLoopItem
          where T : UnityEngine.Object
        {
            readonly T target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            readonly PlayerLoopTiming timing;
            U currentValue;
            CancellationToken cancellation;
            bool isRunning = false;

            public WaitUntilValueChangedUnityObjectWithReturnIsDestoryedPromise(T target, Func<T, U> monitorFunction, PlayerLoopTiming timing, IEqualityComparer<U> equalityComparer, CancellationToken cancellation)
            {
                this.target = target;
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UnityEqualityComparer.GetDefault<U>();
                this.currentValue = monitorFunction(target);
                this.cancellation = cancellation;
                this.timing = timing;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        ResetStatus();
                        PlayerLoopHelper.AddAction(timing, this);
                    }

                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                U nextValue = default(U);

                if (target != null)
                {
                    try
                    {
                        nextValue = monitorFunction(target);
                        if (equalityComparer.Equals(currentValue, nextValue))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isRunning = false;
                        TrySetException(ex);
                        return false;
                    }
                }
                else
                {
                    isRunning = false;
                    TrySetResult((true, default(U)));
                    return false;
                }

                isRunning = false;
                currentValue = nextValue;
                TrySetResult((false, nextValue));
                return false;
            }
        }

        class WaitUntilValueChangedStandardObjectPromise<T, U> : ReusablePromise<U>, IPlayerLoopItem
          where T : class
        {
            readonly WeakReference<T> target;
            readonly Func<T, U> monitorFunction;
            readonly IEqualityComparer<U> equalityComparer;
            U currentValue;
            readonly PlayerLoopTiming timing;
            CancellationToken cancellation;
            bool isRunning = false;

            public WaitUntilValueChangedStandardObjectPromise(T target, Func<T, U> monitorFunction, PlayerLoopTiming timing, IEqualityComparer<U> equalityComparer, CancellationToken cancellation)
            {
                // use WeakReference, but maybe target was referenced by async-statemachine so always referenced...
                this.target = new WeakReference<T>(target);
                this.monitorFunction = monitorFunction;
                this.equalityComparer = equalityComparer ?? UnityEqualityComparer.GetDefault<U>();
                this.currentValue = monitorFunction(target);
                this.cancellation = cancellation;
                this.timing = timing;
            }

            public override bool IsCompleted
            {
                get
                {
                    if (Status == AwaiterStatus.Canceled || Status == AwaiterStatus.Faulted) return true;

                    if (!isRunning)
                    {
                        isRunning = true;
                        PlayerLoopHelper.AddAction(timing, this);
                    }
                    return false;
                }
            }

            public bool MoveNext()
            {
                if (cancellation.IsCancellationRequested)
                {
                    isRunning = false;
                    TrySetCanceled();
                    return false;
                }

                U nextValue = default(U);

                if (target.TryGetTarget(out var t))
                {
                    try
                    {
                        nextValue = monitorFunction(t);
                        if (equalityComparer.Equals(currentValue, nextValue))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isRunning = false;
                        TrySetException(ex);
                        return false;
                    }
                }
                else
                {
                    isRunning = false;
                    TrySetException(new InvalidOperationException("Monitoring target is garbage collected."));
                    return false;
                }

                isRunning = false;
                currentValue = nextValue;
                TrySetResult(nextValue);
                return false;
            }
        }
    }
}
#endif