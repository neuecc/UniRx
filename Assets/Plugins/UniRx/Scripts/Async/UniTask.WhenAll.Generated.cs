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
        public static async UniTask<(T1 result1, T2 result2)> WhenAll<T1, T2>(UniTask<T1> task1, UniTask<T2> task2)
        {
            return await new WhenAllPromise<T1, T2>(task1, task2);
        }

        public static async UniTask<(T1 result1, T2 result2, T3 result3)> WhenAll<T1, T2, T3>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
        {
            return await new WhenAllPromise<T1, T2, T3>(task1, task2, task3);
        }

        public static async UniTask<(T1 result1, T2 result2, T3 result3, T4 result4)> WhenAll<T1, T2, T3, T4>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4)
        {
            return await new WhenAllPromise<T1, T2, T3, T4>(task1, task2, task3, task4);
        }

        public static async UniTask<(T1 result1, T2 result2, T3 result3, T4 result4, T5 result5)> WhenAll<T1, T2, T3, T4, T5>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5)
        {
            return await new WhenAllPromise<T1, T2, T3, T4, T5>(task1, task2, task3, task4, task5);
        }

        public static async UniTask<(T1 result1, T2 result2, T3 result3, T4 result4, T5 result5, T6 result6)> WhenAll<T1, T2, T3, T4, T5, T6>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6)
        {
            return await new WhenAllPromise<T1, T2, T3, T4, T5, T6>(task1, task2, task3, task4, task5, task6);
        }

        public static async UniTask<(T1 result1, T2 result2, T3 result3, T4 result4, T5 result5, T6 result6, T7 result7)> WhenAll<T1, T2, T3, T4, T5, T6, T7>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7)
        {
            return await new WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>(task1, task2, task3, task4, task5, task6, task7);
        }

        class WhenAllPromise<T1, T2>
        {
            const int MaxCount = 2;

            T1 result1;
            T2 result2;
            ExceptionDispatchInfo exception;
            int completeCount;
            Action whenComplete;

            public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.result1 = default(T1);
                this.result2 = default(T2);
                this.exception = null;

                RunTask1(task1).Forget();
                RunTask2(task2).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask1(UniTask<T1> task)
            {
                try
                {
                    result1 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask2(UniTask<T2> task)
            {
                try
                {
                    result2 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T1, T2> parent;

                public Awaiter(WhenAllPromise<T1, T2> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.completeCount == MaxCount;
                    }
                }

                public (T1, T2) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.result1, parent.result2);
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

        class WhenAllPromise<T1, T2, T3>
        {
            const int MaxCount = 3;

            T1 result1;
            T2 result2;
            T3 result3;
            ExceptionDispatchInfo exception;
            int completeCount;
            Action whenComplete;

            public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.result1 = default(T1);
                this.result2 = default(T2);
                this.result3 = default(T3);
                this.exception = null;

                RunTask1(task1).Forget();
                RunTask2(task2).Forget();
                RunTask3(task3).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask1(UniTask<T1> task)
            {
                try
                {
                    result1 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask2(UniTask<T2> task)
            {
                try
                {
                    result2 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask3(UniTask<T3> task)
            {
                try
                {
                    result3 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T1, T2, T3> parent;

                public Awaiter(WhenAllPromise<T1, T2, T3> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.completeCount == MaxCount;
                    }
                }

                public (T1, T2, T3) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.result1, parent.result2, parent.result3);
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

        class WhenAllPromise<T1, T2, T3, T4>
        {
            const int MaxCount = 4;

            T1 result1;
            T2 result2;
            T3 result3;
            T4 result4;
            ExceptionDispatchInfo exception;
            int completeCount;
            Action whenComplete;

            public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.result1 = default(T1);
                this.result2 = default(T2);
                this.result3 = default(T3);
                this.result4 = default(T4);
                this.exception = null;

                RunTask1(task1).Forget();
                RunTask2(task2).Forget();
                RunTask3(task3).Forget();
                RunTask4(task4).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask1(UniTask<T1> task)
            {
                try
                {
                    result1 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask2(UniTask<T2> task)
            {
                try
                {
                    result2 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask3(UniTask<T3> task)
            {
                try
                {
                    result3 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask4(UniTask<T4> task)
            {
                try
                {
                    result4 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T1, T2, T3, T4> parent;

                public Awaiter(WhenAllPromise<T1, T2, T3, T4> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.completeCount == MaxCount;
                    }
                }

                public (T1, T2, T3, T4) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.result1, parent.result2, parent.result3, parent.result4);
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

        class WhenAllPromise<T1, T2, T3, T4, T5>
        {
            const int MaxCount = 5;

            T1 result1;
            T2 result2;
            T3 result3;
            T4 result4;
            T5 result5;
            ExceptionDispatchInfo exception;
            int completeCount;
            Action whenComplete;

            public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.result1 = default(T1);
                this.result2 = default(T2);
                this.result3 = default(T3);
                this.result4 = default(T4);
                this.result5 = default(T5);
                this.exception = null;

                RunTask1(task1).Forget();
                RunTask2(task2).Forget();
                RunTask3(task3).Forget();
                RunTask4(task4).Forget();
                RunTask5(task5).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask1(UniTask<T1> task)
            {
                try
                {
                    result1 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask2(UniTask<T2> task)
            {
                try
                {
                    result2 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask3(UniTask<T3> task)
            {
                try
                {
                    result3 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask4(UniTask<T4> task)
            {
                try
                {
                    result4 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask5(UniTask<T5> task)
            {
                try
                {
                    result5 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T1, T2, T3, T4, T5> parent;

                public Awaiter(WhenAllPromise<T1, T2, T3, T4, T5> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.completeCount == MaxCount;
                    }
                }

                public (T1, T2, T3, T4, T5) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.result1, parent.result2, parent.result3, parent.result4, parent.result5);
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

        class WhenAllPromise<T1, T2, T3, T4, T5, T6>
        {
            const int MaxCount = 6;

            T1 result1;
            T2 result2;
            T3 result3;
            T4 result4;
            T5 result5;
            T6 result6;
            ExceptionDispatchInfo exception;
            int completeCount;
            Action whenComplete;

            public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.result1 = default(T1);
                this.result2 = default(T2);
                this.result3 = default(T3);
                this.result4 = default(T4);
                this.result5 = default(T5);
                this.result6 = default(T6);
                this.exception = null;

                RunTask1(task1).Forget();
                RunTask2(task2).Forget();
                RunTask3(task3).Forget();
                RunTask4(task4).Forget();
                RunTask5(task5).Forget();
                RunTask6(task6).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask1(UniTask<T1> task)
            {
                try
                {
                    result1 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask2(UniTask<T2> task)
            {
                try
                {
                    result2 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask3(UniTask<T3> task)
            {
                try
                {
                    result3 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask4(UniTask<T4> task)
            {
                try
                {
                    result4 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask5(UniTask<T5> task)
            {
                try
                {
                    result5 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask6(UniTask<T6> task)
            {
                try
                {
                    result6 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T1, T2, T3, T4, T5, T6> parent;

                public Awaiter(WhenAllPromise<T1, T2, T3, T4, T5, T6> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.completeCount == MaxCount;
                    }
                }

                public (T1, T2, T3, T4, T5, T6) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.result1, parent.result2, parent.result3, parent.result4, parent.result5, parent.result6);
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

        class WhenAllPromise<T1, T2, T3, T4, T5, T6, T7>
        {
            const int MaxCount = 7;

            T1 result1;
            T2 result2;
            T3 result3;
            T4 result4;
            T5 result5;
            T6 result6;
            T7 result7;
            ExceptionDispatchInfo exception;
            int completeCount;
            Action whenComplete;

            public WhenAllPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3, UniTask<T4> task4, UniTask<T5> task5, UniTask<T6> task6, UniTask<T7> task7)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.result1 = default(T1);
                this.result2 = default(T2);
                this.result3 = default(T3);
                this.result4 = default(T4);
                this.result5 = default(T5);
                this.result6 = default(T6);
                this.result7 = default(T7);
                this.exception = null;

                RunTask1(task1).Forget();
                RunTask2(task2).Forget();
                RunTask3(task3).Forget();
                RunTask4(task4).Forget();
                RunTask5(task5).Forget();
                RunTask6(task6).Forget();
                RunTask7(task7).Forget();
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask1(UniTask<T1> task)
            {
                try
                {
                    result1 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask2(UniTask<T2> task)
            {
                try
                {
                    result2 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask3(UniTask<T3> task)
            {
                try
                {
                    result3 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask4(UniTask<T4> task)
            {
                try
                {
                    result4 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask5(UniTask<T5> task)
            {
                try
                {
                    result5 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask6(UniTask<T6> task)
            {
                try
                {
                    result6 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }

            async UniTaskVoid RunTask7(UniTask<T7> task)
            {
                try
                {
                    result7 = await task;
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                    return;
                }

                var count = Interlocked.Increment(ref completeCount);
                if (count == MaxCount)
                {
                    TryCallContinuation();
                }
            }


            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> parent;

                public Awaiter(WhenAllPromise<T1, T2, T3, T4, T5, T6, T7> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.completeCount == MaxCount;
                    }
                }

                public (T1, T2, T3, T4, T5, T6, T7) GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return (parent.result1, parent.result2, parent.result3, parent.result4, parent.result5, parent.result6, parent.result7);
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