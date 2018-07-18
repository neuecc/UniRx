#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        // UniTask

        public static async UniTask<(bool hasResultLeft, T0 result)> WhenAny<T0>(UniTask<T0> task0, UniTask task1)
        {
            return await new UnitWhenAnyPromise<T0>(task0, task1);
        }

        public static async UniTask<(int winArgumentIndex, T result)> WhenAny<T>(params UniTask<T>[] tasks)
        {
            return await new WhenAnyPromise<T>(tasks);
        }

        public static async UniTask<int> WhenAny(params UniTask[] tasks)
        {
            return await new WhenAnyPromise(tasks);
        }

        // boxed

        public static async UniTask<(bool hasResultLeft, T0 result)> WhenAny<T0>(IAwaitable<T0> task0, IAwaitable task1)
        {
            return await new BoxedUnitWhenAnyPromise<T0>(task0, task1);
        }

        public static async UniTask<(int winArgumentIndex, T result)> WhenAny<T>(params IAwaitable<T>[] tasks)
        {
            return await new BoxedWhenAnyPromise<T>(tasks);
        }

        public static async UniTask<int> WhenAny(params IAwaitable[] tasks)
        {
            return await new BoxedWhenAnyPromise(tasks);
        }

        class UnitWhenAnyPromise<T0>
        {
            T0 result0;
            ExceptionDispatchInfo exception;
            Action whenComplete;
            int completeCount;
            int winArgumentIndex;

            bool IsCompleted => exception != null || Volatile.Read(ref winArgumentIndex) != -1;

            public UnitWhenAnyPromise(UniTask<T0> task0, UniTask task1)
            {
                this.whenComplete = null;
                this.exception = null;
                this.completeCount = 0;
                this.winArgumentIndex = -1;
                this.result0 = default(T0);

                RunTask0(task0).Forget();
                RunTask1(task1).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask0(UniTask<T0> task)
            {
                T0 value;
                try
                {
                    value = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    result0 = value;
                    Volatile.Write(ref winArgumentIndex, 0);
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask1(UniTask task)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    Volatile.Write(ref winArgumentIndex, 1);
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                UnitWhenAnyPromise<T0> parent;

                public Awaiter(UnitWhenAnyPromise<T0> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.IsCompleted;
                    }
                }

                public (bool, T0) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.winArgumentIndex == 0, parent.result0);
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }


        class WhenAnyPromise<T>
        {
            T result;
            int completeCount;
            int winArgumentIndex;
            Action whenComplete;
            ExceptionDispatchInfo exception;

            public bool IsComplete => exception != null || Volatile.Read(ref winArgumentIndex) != -1;

            public WhenAnyPromise(UniTask<T>[] tasks)
            {
                this.completeCount = 0;
                this.winArgumentIndex = -1;
                this.whenComplete = null;
                this.exception = null;
                this.result = default(T);

                for (int i = 0; i < tasks.Length; i++)
                {
                    RunTask(tasks[i], i).Forget();
                }
            }

            async UniTaskVoid RunTask(UniTask<T> task, int index)
            {
                T value;
                try
                {
                    value = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    result = value;
                    Volatile.Write(ref winArgumentIndex, index);
                    TryCallContinuation();
                }
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAnyPromise<T> parent;

                public Awaiter(WhenAnyPromise<T> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.IsComplete;
                    }
                }

                public (int, T) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.winArgumentIndex, parent.result);
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }

        class WhenAnyPromise
        {
            int completeCount;
            int winArgumentIndex;
            Action whenComplete;
            ExceptionDispatchInfo exception;

            public bool IsComplete => exception != null || Volatile.Read(ref winArgumentIndex) != -1;

            public WhenAnyPromise(UniTask[] tasks)
            {
                this.completeCount = 0;
                this.winArgumentIndex = -1;
                this.whenComplete = null;
                this.exception = null;

                for (int i = 0; i < tasks.Length; i++)
                {
                    RunTask(tasks[i], i).Forget();
                }
            }

            async UniTaskVoid RunTask(UniTask task, int index)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    Volatile.Write(ref winArgumentIndex, index);
                    TryCallContinuation();
                }
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAnyPromise parent;

                public Awaiter(WhenAnyPromise parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.IsComplete;
                    }
                }

                public int GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return parent.winArgumentIndex;
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }

        // boxed

        class BoxedUnitWhenAnyPromise<T0>
        {
            T0 result0;
            ExceptionDispatchInfo exception;
            Action whenComplete;
            int completeCount;
            int winArgumentIndex;

            bool IsCompleted => exception != null || Volatile.Read(ref winArgumentIndex) != -1;

            public BoxedUnitWhenAnyPromise(IAwaitable<T0> task0, IAwaitable task1)
            {
                this.whenComplete = null;
                this.exception = null;
                this.completeCount = 0;
                this.winArgumentIndex = -1;
                this.result0 = default(T0);

                RunTask0(task0).Forget();
                RunTask1(task1).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask0(IAwaitable<T0> task)
            {
                T0 value;
                try
                {
                    value = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    result0 = value;
                    Volatile.Write(ref winArgumentIndex, 0);
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask1(IAwaitable task)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    Volatile.Write(ref winArgumentIndex, 1);
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                BoxedUnitWhenAnyPromise<T0> parent;

                public Awaiter(BoxedUnitWhenAnyPromise<T0> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.IsCompleted;
                    }
                }

                public (bool, T0) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.winArgumentIndex == 0, parent.result0);
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }


        class BoxedWhenAnyPromise<T>
        {
            T result;
            int completeCount;
            int winArgumentIndex;
            Action whenComplete;
            ExceptionDispatchInfo exception;

            public bool IsComplete => exception != null || Volatile.Read(ref winArgumentIndex) != -1;

            public BoxedWhenAnyPromise(IAwaitable<T>[] tasks)
            {
                this.completeCount = 0;
                this.winArgumentIndex = -1;
                this.whenComplete = null;
                this.exception = null;
                this.result = default(T);

                for (int i = 0; i < tasks.Length; i++)
                {
                    RunTask(tasks[i], i).Forget();
                }
            }

            async UniTaskVoid RunTask(IAwaitable<T> task, int index)
            {
                T value;
                try
                {
                    value = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    result = value;
                    Volatile.Write(ref winArgumentIndex, index);
                    TryCallContinuation();
                }
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                BoxedWhenAnyPromise<T> parent;

                public Awaiter(BoxedWhenAnyPromise<T> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.IsComplete;
                    }
                }

                public (int, T) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.winArgumentIndex, parent.result);
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }

        class BoxedWhenAnyPromise
        {
            int completeCount;
            int winArgumentIndex;
            Action whenComplete;
            ExceptionDispatchInfo exception;

            public bool IsComplete => exception != null || Volatile.Read(ref winArgumentIndex) != -1;

            public BoxedWhenAnyPromise(IAwaitable[] tasks)
            {
                this.completeCount = 0;
                this.winArgumentIndex = -1;
                this.whenComplete = null;
                this.exception = null;

                for (int i = 0; i < tasks.Length; i++)
                {
                    RunTask(tasks[i], i).Forget();
                }
            }

            async UniTaskVoid RunTask(IAwaitable task, int index)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == 1)
                {
                    Volatile.Write(ref winArgumentIndex, index);
                    TryCallContinuation();
                }
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                BoxedWhenAnyPromise parent;

                public Awaiter(BoxedWhenAnyPromise parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.IsComplete;
                    }
                }

                public int GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return parent.winArgumentIndex;
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }
    }
}

#endif