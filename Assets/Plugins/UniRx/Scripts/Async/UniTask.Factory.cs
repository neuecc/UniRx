#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        public static UniTask CompletedTask
        {
            get
            {
                return new UniTask();
            }
        }

        public static UniTask FromException(Exception ex)
        {
            var promise = new Promise<AsyncUnit>();
            promise.SetException(ex);
            return new UniTask(promise);
        }

        public static UniTask<T> FromException<T>(Exception ex)
        {
            var promise = new Promise<T>();
            promise.SetException(ex);
            return new UniTask<T>(promise);
        }

        public static UniTask<T> FromResult<T>(T value)
        {
            return new UniTask<T>(value);
        }
    }
}
#endif