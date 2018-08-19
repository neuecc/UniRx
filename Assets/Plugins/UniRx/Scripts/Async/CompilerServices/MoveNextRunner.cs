#if CSHARP_7_OR_LATER

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;

namespace UniRx.Async.CompilerServices
{
    internal class MoveNextRunner<TStateMachine>
        where TStateMachine : IAsyncStateMachine
    {
        public TStateMachine StateMachine;

        public void Run()
        {
            StateMachine.MoveNext();
        }
    }
}

#endif