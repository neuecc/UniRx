#if (NET_4_6 || NET_STANDARD_2_0)
#pragma warning disable CS1591

using System.Runtime.CompilerServices;

namespace UniRx.Async
{
    // This interfaces used by UniTask.WhenAll/WhenAny
    // If implement this interface and if awaiter/awaitable can be struct,
    // use explicit-interface-implementation.

    public interface IAwaitable
    {
        IAwaiter GetAwaiter();
    }

    public interface IAwaitable<out T> : IAwaitable
    {
        new IAwaiter<T> GetAwaiter();
    }

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