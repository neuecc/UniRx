using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UniRx.InternalUtil;

using UnityEngine;

namespace UniRx
{
#if UNITY_EDITOR

    // In UnityEditor's EditorMode can't instantiate and work MonoBehaviour.Update.
    // EditorThreadDispatcher use EditorApplication.update instead of MonoBehaviour.Update.
    public class EditorThreadDispatcher
    {
        static object gate = new object();
        static EditorThreadDispatcher instance;

        public static EditorThreadDispatcher Instance
        {
            get
            {
                // Activate EditorThreadDispatcher is dangerous, completely Lazy.
                lock (gate)
                {
                    if (instance == null)
                    {
                        instance = new EditorThreadDispatcher();
                    }

                    return instance;
                }
            }
        }

        ThreadSafeQueueWorker editorQueueWorker = new ThreadSafeQueueWorker();

        EditorThreadDispatcher()
        {
            UnityEditor.EditorApplication.update += Update;
        }

        public static void EnqueueOnInstance(Action action)
        {
            Instance.Enqueue(action);
        }

        public static void UnsafeInvokeOnInstance(Action action)
        {
            Instance.UnsafeInvoke(action);
        }

        public static void PseudoStartCoroutineOnInstance(IEnumerator routine)
        {
            Instance.PseudoStartCoroutine(routine);
        }

        public void Enqueue(Action action)
        {
            editorQueueWorker.Enqueue(action);
        }

        public void UnsafeInvoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void PseudoStartCoroutine(IEnumerator routine)
        {
            editorQueueWorker.Enqueue(() => ConsumeEnumerator(routine));
        }

        void Update()
        {
            editorQueueWorker.ExecuteAll(x => Debug.LogException(x));
        }

        void ConsumeEnumerator(IEnumerator routine)
        {
            if (routine.MoveNext())
            {
                var current = routine.Current;
                if (current == null)
                {
                    goto ENQUEUE;
                }

                var type = current.GetType();
                if (type == typeof(WWW))
                {
                    var www = (WWW)current;
                    editorQueueWorker.Enqueue(() => ConsumeEnumerator(UnwrapWaitWWW(www, routine)));
                    return;
                }
                else if (type == typeof(AsyncOperation))
                {
                    var asyncOperation = (AsyncOperation)current;
                    editorQueueWorker.Enqueue(() => ConsumeEnumerator(UnwrapWaitAsyncOperation(asyncOperation, routine)));
                    return;
                }
                else if (type == typeof(WaitForSeconds))
                {
                    var waitForSeconds = (WaitForSeconds)current;
                    var accessor = typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
                    var second = (float)accessor.GetValue(waitForSeconds);
                    editorQueueWorker.Enqueue(() => ConsumeEnumerator(UnwrapWaitForSeconds(second, routine)));
                    return;
                }
                else if (type == typeof(Coroutine))
                {
                    Debug.Log("Can't wait coroutine on UnityEditor");
                    goto ENQUEUE;
                }

            ENQUEUE:
                editorQueueWorker.Enqueue(() => ConsumeEnumerator(routine)); // next update
            }
        }

        IEnumerator UnwrapWaitWWW(WWW www, IEnumerator continuation)
        {
            while (!www.isDone)
            {
                yield return null;
            }
            ConsumeEnumerator(continuation);
        }

        IEnumerator UnwrapWaitAsyncOperation(AsyncOperation asyncOperation, IEnumerator continuation)
        {
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            ConsumeEnumerator(continuation);
        }

        IEnumerator UnwrapWaitForSeconds(float second, IEnumerator continuation)
        {
            var startTime = DateTimeOffset.UtcNow;
            while (true)
            {
                yield return null;

                var elapsed = (DateTimeOffset.UtcNow - startTime).TotalSeconds;
                if (elapsed >= second)
                {
                    break;
                }
            };
            ConsumeEnumerator(continuation);
        }
    }

#endif
}
