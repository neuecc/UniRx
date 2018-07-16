#if NET_4_6 || NET_STANDARD_2_0 || CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace UniRx.Async
{
    /// <summary>
    /// Lightweight IProgress[T] factory.
    /// </summary>
    public static class Progress
    {
        public static IProgress<T> Create<T>(Action<T> handler)
        {
            if (handler == null) return NullProgress<T>.Instance;
            return new AnonymousProgress<T>(handler);
        }

        sealed class NullProgress<T> : IProgress<T>
        {
            public static readonly IProgress<T> Instance = new NullProgress<T>();

            NullProgress()
            {

            }

            public void Report(T value)
            {
            }
        }

        sealed class AnonymousProgress<T> : IProgress<T>
        {
            readonly Action<T> action;

            public AnonymousProgress(Action<T> action)
            {
                this.action = action;
            }

            public void Report(T value)
            {
                action(value);
            }
        }
    }
}

#endif