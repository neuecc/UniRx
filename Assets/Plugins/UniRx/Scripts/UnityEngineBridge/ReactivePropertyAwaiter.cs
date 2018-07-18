#if (NET_4_6 || NET_STANDARD_2_0)

#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UniRx.Async;

namespace UniRx
{
    internal sealed class ReactivePropertyAwaiter<T> : IAwaiter<T>
    {
        object continuationField;
        T value;

        public bool IsCompleted => false;

        public void InvokeContinuation(ref T value)
        {
            this.value = value;
            CompletionHelper.InvokeContinuation(ref continuationField);
        }

        void IAwaiter.GetResult()
        {
        }

        public T GetResult()
        {
            return value;
        }

        public void OnCompleted(Action continuation)
        {
            CompletionHelper.AppendContinuation(ref continuationField, continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            CompletionHelper.AppendContinuation(ref continuationField, continuation);
        }
    }

    internal static class CompletionHelper
    {
        public static void AppendContinuation(ref object continuationField, Action newContinuation)
        {
        TRY_AGAIN:
            var field = continuationField;

            if (field == null)
            {
                var ret = Interlocked.CompareExchange(ref continuationField, newContinuation, null);
                if (ret == null)
                {
                    return; // ok to replace.
                }
            }

            if (field is List<Action> list)
            {
                lock (field)
                {
                    list.Add(newContinuation);
                    if (field == continuationField)
                    {
                        return; // ok
                    }
                    goto TRY_AGAIN;
                }
            }
            else
            {
                list = new List<Action>(2) { (Action)field };
                list.Add(newContinuation);

                if (Interlocked.CompareExchange(ref continuationField, (object)list, field) == field)
                {
                    return;
                }
                goto TRY_AGAIN;
            }
        }

        public static void InvokeContinuation(ref object continuationField)
        {
            if (continuationField == null)
            {
                return;
            }

            var continuation = Interlocked.Exchange(ref continuationField, null);

            if (continuation is Action action)
            {
                action();
            }
            else
            {
                var list = (List<Action>)continuation;
                lock (list)
                {
                    var len = list.Count;
                    for (int i = 0; i < len; i++)
                    {
                        list[i].Invoke();
                    }
                }
            }
        }
    }
}

#endif