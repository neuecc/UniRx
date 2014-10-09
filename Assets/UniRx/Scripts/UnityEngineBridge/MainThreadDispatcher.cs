using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UniRx.InternalUtil;
using UnityEngine;

namespace UniRx
{
    public sealed class MainThreadDispatcher : MonoBehaviour
    {
#if UNITY_EDITOR

        // In UnityEditor's EditorMode can't instantiate and work MonoBehaviour.Update.
        // EditorThreadDispatcher use EditorApplication.update instead of MonoBehaviour.Update.
        class EditorThreadDispatcher
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

            bool isDisposed;
            ThreadSafeQueueWorker queueWorker = new ThreadSafeQueueWorker();

            EditorThreadDispatcher()
            {
                UnityEditor.EditorApplication.update += Update;
            }

            public void Enqueue(Action action)
            {
                queueWorker.Enqueue(action);
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
                queueWorker.Enqueue(() => ConsumeEnumerator(routine));
            }

            void Update()
            {
                queueWorker.ExecuteAll(x => Debug.LogException(x));
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
                        queueWorker.Enqueue(() => ConsumeEnumerator(UnwrapWaitWWW(www, routine)));
                        return;
                    }
                    else if (type == typeof(WaitForSeconds))
                    {
                        var waitForSeconds = (WaitForSeconds)current;
                        var accessor = typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
                        var second = (float)accessor.GetValue(waitForSeconds);
                        queueWorker.Enqueue(() => ConsumeEnumerator(UnwrapWaitForSeconds(second, routine)));
                        return;
                    }
                    else if (type == typeof(Coroutine))
                    {
                        Debug.Log("Can't wait coroutine on UnityEditor");
                        goto ENQUEUE;
                    }

                ENQUEUE:
                    queueWorker.Enqueue(() => ConsumeEnumerator(routine)); // next update
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

        /// <summary>Dispatch Asyncrhonous action.</summary>
        public static void Post(Action action)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                // can detect if after Start
                if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.Enqueue(action); return; }
            }
#endif

            Instance.queueWorker.Enqueue(action);
        }

        /// <summary>Dispatch Synchronous action if possible.</summary>
        public static void Send(Action action)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                // can detect if after Start
                if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.Enqueue(action); return; }
            }
#endif

            if (mainThreadToken != null)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    MainThreadDispatcher.Instance.unhandledExceptionCallback(ex);
                }
            }
            else
            {
                Post(action);
            }
        }

        /// <summary>Run Synchronous action.</summary>
        public static void UnsafeSend(Action action)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                if (!Application.isPlaying) { EditorThreadDispatcher.Instance.UnsafeInvoke(action); return; }
            }
#endif

            try
            {
                action();
            }
            catch (Exception ex)
            {
                MainThreadDispatcher.Instance.unhandledExceptionCallback(ex);
            }
        }

        /// <summary>ThreadSafe StartCoroutine.</summary>
        public static void SendStartCoroutine(IEnumerator routine)
        {
            if (mainThreadToken != null)
            {
                StartCoroutine(routine);
            }
            else
            {
#if UNITY_EDITOR
                if (!initialized)
                {
                    // call from other thread, can detect if after Start
                    if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
                }
#endif

                Instance.queueWorker.Enqueue(() => Instance.StartCoroutine_Auto(routine));
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!initialized)
            {
                if (!Application.isPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return null; }
            }
#endif

            return Instance.StartCoroutine_Auto(routine);
        }

        public static void RegisterUnhandledExceptionCallback(Action<Exception> exceptionCallback)
        {
            if (exceptionCallback == null)
            {
                // do nothing
                Instance.unhandledExceptionCallback = Stubs.Ignore<Exception>;
            }
            else
            {
                Instance.unhandledExceptionCallback = exceptionCallback;
            }
        }

        ThreadSafeQueueWorker queueWorker = new ThreadSafeQueueWorker();
        Action<Exception> unhandledExceptionCallback = ex => Debug.LogException(ex); // default

        static MainThreadDispatcher instance;
        static bool initialized;

        [ThreadStatic]
        static object mainThreadToken;

        private MainThreadDispatcher()
        {

        }

        static MainThreadDispatcher Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        public static void Initialize()
        {
            if (!initialized)
            {
                // Don't try to add a GameObject when the scene is not playing. Only valid in the Editor, EditorView.
                if (!Application.isPlaying) return;

                initialized = true;
                instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(instance);
                mainThreadToken = new object();
            }
        }

        /// <summary>
        /// <para>Unsafe(don't valid playing or not) initialization.</para>
        /// <para>You should call from application entry point once.</para>
        /// </summary>
        public static void UnsafeInitialize()
        {
            if (!initialized)
            {
                initialized = true;
                instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(instance);
                mainThreadToken = new object();
            }
        }

        void Awake()
        {
            instance = this;
            initialized = true;
        }

        void Update()
        {
            queueWorker.ExecuteAll(unhandledExceptionCallback);
        }

        // for Lifecycle Management

        Subject<bool> onApplicationFocus;
        void OnApplicationFocus(bool focus)
        {
            if (onApplicationFocus != null) onApplicationFocus.OnNext(focus);
        }
        public static IObservable<bool> OnApplicationFocusAsObservable()
        {
            return Instance.onApplicationFocus ?? (Instance.onApplicationFocus = new Subject<bool>());
        }

        Subject<bool> onApplicationPause;
        void OnApplicationPause(bool pause)
        {
            if (onApplicationPause != null) onApplicationPause.OnNext(pause);
        }
        public static IObservable<bool> OnApplicationPauseAsObservable()
        {
            return Instance.onApplicationPause ?? (Instance.onApplicationPause = new Subject<bool>());
        }

        Subject<Unit> onApplicationQuit;
        void OnApplicationQuit()
        {
            if (onApplicationQuit != null) onApplicationQuit.OnNext(Unit.Default);
        }
        public static IObservable<Unit> OnApplicationQuitAsObservable()
        {
            return Instance.onApplicationQuit ?? (Instance.onApplicationQuit = new Subject<Unit>());
        }
    }
}
