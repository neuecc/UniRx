#if CSHARP_7_OR_LATER

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace UniRx.Async.CompilerServices
{
    public struct AsyncUniTaskVoidMethodBuilder
    {
        Action moveNext;

        // 1. Static Create method.
        [DebuggerHidden]
        public static AsyncUniTaskVoidMethodBuilder Create()
        {
            var builder = new AsyncUniTaskVoidMethodBuilder();
            return builder;
        }

        // 2. TaskLike Task property(void)
        public UniTaskVoid Task => default(UniTaskVoid);

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult()
        {
            // do nothing
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                var runner = new MoveNextRunner();
                moveNext = runner.Run;

                var boxed = (IAsyncStateMachine)stateMachine;
                runner.StateMachine = boxed; // boxed
            }

            awaiter.OnCompleted(moveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (moveNext == null)
            {
                var runner = new MoveNextRunner();
                moveNext = runner.Run;

                var boxed = (IAsyncStateMachine)stateMachine;
                runner.StateMachine = boxed; // boxed
            }

            awaiter.UnsafeOnCompleted(moveNext);
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        class MoveNextRunner
        {
            public IAsyncStateMachine StateMachine;

            public void Run()
            {
                StateMachine.MoveNext();
            }
        }
    }
}

#endif