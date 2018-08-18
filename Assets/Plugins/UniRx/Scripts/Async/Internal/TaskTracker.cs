#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UniRx.Async.Internal
{
    public static class TaskTracker
    {
        static List<KeyValuePair<IAwaiter, (DateTime addTime, string stackTrace)>> listPool = new List<KeyValuePair<IAwaiter, (DateTime addTime, string stackTrace)>>();

        static readonly WeakDictionary<IAwaiter, (DateTime addTime, string stackTrace)> tracking = new WeakDictionary<IAwaiter, (DateTime addTime, string stackTrace)>();

        [Conditional("UNITY_EDITOR")]
        public static void TrackActiveTask(IAwaiter task, int skipFrame = 1)
        {
            // TODO:Configuration Option(don't use stacktrace, etc.)
            tracking.TryAdd(task, (DateTime.UtcNow, new StackTrace(skipFrame, true).CleanupAsyncStackTrace()));
        }

        [Conditional("UNITY_EDITOR")]
        public static void TrackActiveTask(IAwaiter task, StackTrace stackTrace)
        {
            // TODO:Configuration Option(don't use stacktrace, etc.)
            tracking.TryAdd(task, (DateTime.UtcNow, stackTrace.CleanupAsyncStackTrace()));
        }

        public static StackTrace CaptureStackTrace(int skipFrame)
        {
#if UNITY_EDITOR
            // TODO:Configureation Option... return null...
            return new StackTrace(skipFrame + 1, true);
#else
            return null;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveTracking(IAwaiter task)
        {
            // TODO:Configuration Option(don't use stacktrace, etc.)
            tracking.TryRemove(task);
        }

        public static void ForEachActiveTask(Action<AwaiterStatus, DateTime, string> action)
        {
            lock (listPool)
            {
                var count = tracking.ToList(ref listPool, clear: false);
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        action(listPool[i].Key.Status, listPool[i].Value.addTime, listPool[i].Value.stackTrace);
                        listPool[i] = new KeyValuePair<IAwaiter, (DateTime addTime, string stackTrace)>(null, (default(DateTime), null)); // clear
                    }
                }
                catch
                {
                    listPool.Clear();
                    throw;
                }
            }
        }
    }
}

#endif