#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async UniTask Run(Action action, bool configureAwait = true)
        {
            await UniTask.SwitchToThreadPool();
            action();
            if (configureAwait)
            {
                await UniTask.Yield();
            }
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async UniTask Run(Action<object> action, object state, bool configureAwait = true)
        {
            await UniTask.SwitchToThreadPool();
            action(state);
            if (configureAwait)
            {
                await UniTask.Yield();
            }
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async UniTask<T> Run<T>(Func<T> func, bool configureAwait = true)
        {
            await UniTask.SwitchToThreadPool();
            var result = func();
            if (configureAwait)
            {
                await UniTask.Yield();
            }
            return result;
        }

        /// <summary>Run action on the threadPool and return to main thread if configureAwait = true.</summary>
        public static async UniTask<T> Run<T>(Func<object, T> func, object state, bool configureAwait = true)
        {
            await UniTask.SwitchToThreadPool();
            var result = func(state);
            if (configureAwait)
            {
                await UniTask.Yield();
            }
            return result;
        }
    }
}

#endif