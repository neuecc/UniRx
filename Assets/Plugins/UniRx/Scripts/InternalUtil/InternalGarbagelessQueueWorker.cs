using System;

namespace UniRx.InternalUtil
{
    internal interface IActionList
    {
        int Count { get; }
        void InvokeAll();
    }

    // used for MainThreadDispatcher, optimized version of ThreadSafeQueueWorker.
    internal static class InternalGarbagelessQueueWorker
    {
        const int InitialSize = 16;

        static readonly object gate = new object();
        static bool isDequing = false;

        static bool useWorkerOne = true; // CoWorker1 or CoWorker2

        static int tail1 = 0;
        static IActionList[] workers1 = new IActionList[InitialSize];

        static int tail2 = 0;
        static IActionList[] workers2 = new IActionList[InitialSize];

        public static void Enqueue<T>(Action<T> action, T state)
        {
            Enqueue(ref action, ref state);
        }

        public static void Enqueue<T>(ref Action<T> action, ref T state)
        {
            lock (gate)
            {
                var enqueueTargetIsOne =
                       (isDequing && useWorkerOne) ? false
                     : (isDequing && !useWorkerOne) ? true
                     : (!isDequing && useWorkerOne) ? true
                     : false;

                if (enqueueTargetIsOne)
                {
                    var worker = CoWorker<T>.Instance1;
                    if (worker.Count == 0)
                    {
                        if (workers1.Length == tail1)
                        {
                            Array.Resize(ref workers1, tail1 * 2);
                        }

                        workers1[tail1++] = worker;
                    }
                    worker.Add(ref action, ref state);
                }
                else
                {
                    var worker = CoWorker<T>.Instance2;
                    if (worker.Count == 0)
                    {
                        if (workers2.Length == tail2)
                        {
                            Array.Resize(ref workers2, tail2 * 2);
                        }

                        workers2[tail2++] = worker;
                    }
                    worker.Add(ref action, ref state);
                }
            }
        }

        // TODO:handle exception
        public static void ExecuteAll(Action<Exception> unhandledException)
        {
            lock (gate)
            {
                isDequing = true;
            }

            if (useWorkerOne)
            {
                for (int i = 0; i < tail1; i++)
                {
                    workers1[i].InvokeAll();
                    workers1[i] = null;
                }
            }
            else
            {
                for (int i = 0; i < tail2; i++)
                {
                    workers2[i].InvokeAll();
                    workers2[i] = null;
                }
            }

            lock (gate)
            {
                if (useWorkerOne)
                {
                    tail1 = 0;
                }
                else
                {
                    tail2 = 0;
                }

                useWorkerOne = !useWorkerOne;
                isDequing = false;
            }
        }

        class CoWorker<T> : IActionList
        {
            const int InitialSize = 4; // small start is beautiful
            readonly object gate = new object();

            static readonly CoWorker<T> instance1 = new CoWorker<T>();
            public static CoWorker<T> Instance1
            {
                get
                {
                    return instance1;
                }
            }

            static readonly CoWorker<T> instance2 = new CoWorker<T>();
            public static CoWorker<T> Instance2
            {
                get
                {
                    return instance2;
                }
            }

            CoWorker()
            {
            }

            int actionListCount = 0;
            Action<T>[] actionList = new Action<T>[InitialSize];
            T[] actionStates = new T[InitialSize];

            public int Count
            {
                get
                {
                    return actionListCount;
                }
            }

            public void Add(ref Action<T> action, ref T state)
            {
                if (actionList.Length == actionListCount)
                {
                    Array.Resize(ref actionList, actionListCount * 2);
                }

                actionList[actionListCount] = action;
                actionStates[actionListCount] = state;
                actionListCount++;
            }

            public void InvokeAll()
            {
                for (int i = 0; i < actionListCount; i++)
                {
                    try
                    {
                        actionList[i].Invoke(actionStates[i]);
                    }
                    finally
                    {
                        actionList[i] = null;
                        actionStates[i] = default(T);
                    }
                }

                actionListCount = 0;
            }
        }
    }
}
