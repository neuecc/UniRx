#if (NET_4_6 || NET_STANDARD_2_0)
#pragma warning disable CS1591

using System.Runtime.CompilerServices;

namespace UniRx.Async
{
    public interface IAwaiter : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }

    public interface IAwaiter<out T> : IAwaiter
    {
        new T GetResult();
    }

}

#endif