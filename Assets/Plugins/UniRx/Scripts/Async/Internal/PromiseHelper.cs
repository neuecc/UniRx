#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace UniRx.Async.Internal
{
    public static class PromiseHelper
    {
        public static void TrySetResultAll<TPromise, T>(IEnumerable<TPromise> source, T value)
            where TPromise : class, IResolvePromise<T>
        {
            var rentArray = ArrayPoolUtil.Materialize(source);
            var clearArray = true;
            try
            {
                var array = rentArray.Array;
                var len = rentArray.Length;
                for (int i = 0; i < len; i++)
                {
                    array[i].TrySetResult(value);
                    array[i] = null;
                }
                clearArray = false;
            }
            finally
            {
                rentArray.DisposeManually(clearArray);
            }
        }
    }
}

#endif