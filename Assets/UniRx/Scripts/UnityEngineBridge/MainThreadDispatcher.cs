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

            ThreadSafeQueueWorker editorQueueWorker = new ThreadSafeQueueWorker();

            EditorThreadDispatcher()
            {
                UnityEditor.EditorApplication.update += Update;
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
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.Enqueue(action); return; }

#endif

            Instance.queueWorker.Enqueue(action);
        }

        /// <summary>Dispatch Synchronous action if possible.</summary>
        public static void Send(Action action)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.Enqueue(action); return; }
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
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.UnsafeInvoke(action); return; }
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
                // call from other thread, can detect if after Start
                if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return; }
#endif

                Instance.queueWorker.Enqueue(() => Instance.StartCoroutine_Auto(routine));
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            if (!ScenePlaybackDetector.IsPlaying) { EditorThreadDispatcher.Instance.PseudoStartCoroutine(routine); return null; }
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

        public static string InstanceName
        {
            get
            {
                return Instance.name;
            }
        }

        [ThreadStatic]
        static object mainThreadToken;

        static MainThreadDispatcher Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        static void CheckForMainThread()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA || Thread.CurrentThread.ManagedThreadId > 1 ||
                Thread.CurrentThread.IsBackground || Thread.CurrentThread.IsThreadPoolThread)
            {
                // throw exception because the current thread is not the main thread...
                var ex = new Exception("UniRx requires a MainThreadDispatcher component created on the main thread. Make sure it is added to the scene before calling UniRx from a worker thread.");
                UnityEngine.Debug.LogException(ex);
                throw ex;
            }
        }

        public static void Initialize()
        {
            if (!initialized)
            {
                // Throw exception when calling from a worker thread.
                CheckForMainThread();
#if UNITY_EDITOR
                // Don't try to add a GameObject when the scene is not playing. Only valid in the Editor, EditorView.
                if (!ScenePlaybackDetector.IsPlaying) return;
#endif

                var g = GameObject.FindObjectOfType<MainThreadDispatcher>();
                if (g == null)
                {
                    instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                }
                else
                {
                    instance = g;
                }
                DontDestroyOnLoad(instance);
                mainThreadToken = new object();
                initialized = true;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                initialized = true;
            }
            else
            {
                Debug.LogWarning("There is already a MainThreadDispatcher in the scene.");
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                initialized = false;
                instance = null;

                /*
                // Although `this` still refers to a gameObject, it won't be found.
                var foundDispatcher = GameObject.FindObjectOfType<MainThreadDispatcher>();

                if (foundDispatcher != null)
                {
                    // select another game object
                    Debug.Log("new instance: " + foundDispatcher.name);
                    instance = foundDispatcher;
                    initialized = true;
                }
                */
            }
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
