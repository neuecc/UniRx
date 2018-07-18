#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        static readonly UniTask FromCanceledUniTask = new Func<UniTask>(()=>
        {
            var promise = new Promise<AsyncUnit>();
            promise.SetCanceled();
            return new UniTask(promise);
        })();

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

        public static UniTask FromCanceled()
        {
            return FromCanceledUniTask;
        }

        public static UniTask<T> FromCanceled<T>()
        {
            return FromCanceledCache<T>.Task;
        }

        static class FromCanceledCache<T>
        {
            public static readonly UniTask<T> Task;

            static FromCanceledCache()
            {
                var promise = new Promise<T>();
                promise.SetCanceled();
                Task = new UniTask<T>(promise);
            }
        }
    }
}
#endif