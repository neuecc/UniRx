using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.InternalUtil
{
    /// <summary>
    /// Simple supports(only yield return null) lightweight, threadsafe coroutine dispatcher.
    /// </summary>
    public class MicroCoroutine
    {
        const int InitialSize = 16;

        readonly object runningAndQueueLock = new object();
        readonly object arrayLock = new object();
        readonly Func<bool> refreshCycleStrategy;
        readonly Action<Exception> unhandledExceptionCallback;

        int tail = 0;
        bool running = false;
        bool needsRefresh = false;
        IEnumerator[] coroutines = new IEnumerator[InitialSize];
        Queue<IEnumerator> waitQueue = new Queue<IEnumerator>();

        public MicroCoroutine(Func<bool> refreshCycleStrategy, Action<Exception> unhandledExceptionCallback)
        {
            this.refreshCycleStrategy = refreshCycleStrategy;
            this.unhandledExceptionCallback = unhandledExceptionCallback;
        }

        public void AddCoroutine(IEnumerator enumerator)
        {
            lock (runningAndQueueLock)
            {
                if (running)
                {
                    waitQueue.Enqueue(enumerator);
                    return;
                }
            }

            // worst case at multi threading, wait lock until finish Run() but it is super rarely.
            lock (arrayLock)
            {
                // Ensure Capacity
                if (coroutines.Length == tail)
                {
                    Array.Resize(ref coroutines, checked(tail * 2));
                }
                coroutines[tail++] = enumerator;
            }
        }

        public void Run()
        {
            lock (runningAndQueueLock)
            {
                running = true;
            }

            lock (arrayLock)
            {
                for (int i = 0; i < tail; i++)
                {
                    var coroutine = coroutines[i];

                    if (coroutine != null)
                    {
                        try
                        {
                            if (!coroutine.MoveNext())
                            {
                                coroutines[i] = null;
                                needsRefresh = true;
                            }
                            else
                            {
#if UNITY_EDITOR
                                // validation only on Editor.
                                if (coroutine.Current != null)
                                {
                                    UnityEngine.Debug.LogWarning("MicroCoroutine supports only yield return null. return value = " + coroutine.Current);
                                }
#endif
                            }
                        }
                        catch (Exception ex)
                        {
                            coroutines[i] = null;
                            try
                            {
                                unhandledExceptionCallback(ex);
                            }
                            catch { }
                        }
                    }
                }

                lock (runningAndQueueLock)
                {
                    running = false;
                    while (waitQueue.Count != 0)
                    {
                        if (coroutines.Length == tail)
                        {
                            Array.Resize(ref coroutines, checked(tail * 2));
                        }
                        coroutines[tail++] = waitQueue.Dequeue();
                    }
                }

                if (needsRefresh && refreshCycleStrategy())
                {
                    Refresh();
                    needsRefresh = false;
                }
            }
        }

        void Refresh()
        {
            var j = tail - 1;

            // eliminate array-bound check for i
            for (int i = 0; i < coroutines.Length; i++)
            {
                if (coroutines[i] == null)
                {
                    while (i < j)
                    {
                        var fromTail = coroutines[j];
                        if (fromTail != null)
                        {
                            coroutines[i] = fromTail;
                            coroutines[j] = null;
                            j--;
                            goto NEXTLOOP;
                        }
                        j--;
                    }

                    tail = i; // loop end
                    break;
                }
                NEXTLOOP:
                continue;
            }
        }
    }
}