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
        public enum CullingMode
        {
            /// <summary>
            /// Won't remove any MainThreadDispatchers.
            /// </summary>
            Disabled,

            /// <summary>
            /// Checks if there is an existing MainThreadDispatcher on Awake(). If so, the new dispatcher removes itself.
            /// </summary>
            Self,

            /// <summary>
            /// Search for excess MainThreadDispatchers and removes them all on Awake().
            /// </summary>
            All
        }

        public static CullingMode cullingMode = CullingMode.Self;

        public class EditorThreadDispatcherStub
        {
            private static Type editorThreadDispatcherType = null;

            private static MethodInfo enqueueMethod = null;
            private static MethodInfo unsafeInvokeMethod = null;
            private static MethodInfo pseudoStartCoroutineMethod = null;

            static EditorThreadDispatcherStub()
            {
                editorThreadDispatcherType = TypeLoader.GetType("EditorThreadDispatcher");
                if (editorThreadDispatcherType != null)
                {
                    enqueueMethod = editorThreadDispatcherType.GetMethod(
                        "EnqueueOnInstance",
                        BindingFlags.Public | BindingFlags.Static);

                    unsafeInvokeMethod = editorThreadDispatcherType.GetMethod(
                        "UnsafeInvokeOnInstance",
                        BindingFlags.Public | BindingFlags.Static);

                    pseudoStartCoroutineMethod = editorThreadDispatcherType.GetMethod(
                        "PseudoStartCoroutineOnInstance",
                        BindingFlags.Public | BindingFlags.Static);
                }
            }

            public static void EnqueueOnInstance(Action action)
            {
                if (editorThreadDispatcherType == null)
                {
                    return;
                }

                enqueueMethod.Invoke(null, new object[] { action });
            }

            public static void UnsafeInvokeOnInstance(Action action)
            {
                if (editorThreadDispatcherType == null)
                {
                    return;
                }

                unsafeInvokeMethod.Invoke(null, new object[] { action });
            }

            public static void PseudoStartCoroutineOnInstance(IEnumerator routine)
            {
                if (editorThreadDispatcherType == null)
                {
                    return;
                }

                pseudoStartCoroutineMethod.Invoke(null, new object[] { routine });
            }
        }

        /// <summary>Dispatch Asyncrhonous action.</summary>
        public static void Post(Action action)
        {
            if (!ScenePlaybackDetectorStub.IsPlaying) { EditorThreadDispatcherStub.EnqueueOnInstance(action); return; }

            var dispatcher = Instance;
            if (!isQuitting && !object.ReferenceEquals(dispatcher, null))
            {
                dispatcher.queueWorker.Enqueue(action);
            }
        }

        /// <summary>Dispatch Synchronous action if possible.</summary>
        public static void Send(Action action)
        {
            if (!ScenePlaybackDetectorStub.IsPlaying) { EditorThreadDispatcherStub.EnqueueOnInstance(action); return; }

            if (mainThreadToken != null)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    var dispatcher = MainThreadDispatcher.Instance;
                    if (dispatcher != null)
                    {
                        dispatcher.unhandledExceptionCallback(ex);
                    }
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
            if (!ScenePlaybackDetectorStub.IsPlaying) { EditorThreadDispatcherStub.UnsafeInvokeOnInstance(action); return; }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                var dispatcher = MainThreadDispatcher.Instance;
                if (dispatcher != null)
                {
                    dispatcher.unhandledExceptionCallback(ex);
                }
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
                if (!ScenePlaybackDetectorStub.IsPlaying) { EditorThreadDispatcherStub.PseudoStartCoroutineOnInstance(routine); return; }

                var dispatcher = Instance;
                if (!isQuitting && !object.ReferenceEquals(dispatcher, null))
                {
                    dispatcher.queueWorker.Enqueue(() =>
                    {
                        var distpacher2 = Instance;
                        if (distpacher2 != null)
                        {
                            distpacher2.StartCoroutine_Auto(routine);
                        }
                    });
                }
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (!ScenePlaybackDetectorStub.IsPlaying) { EditorThreadDispatcherStub.PseudoStartCoroutineOnInstance(routine); return null; }

            var dispatcher = Instance;
            if (dispatcher != null)
            {
                return dispatcher.StartCoroutine_Auto(routine);
            }
            else
            {
                return null;
            }
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
        static bool isQuitting = false;

        public static string InstanceName
        {
            get
            {
                if (instance == null)
                {
                    throw new NullReferenceException("MainThreadDispatcher is not initialized.");
                }
                return instance.name;
            }
        }

        public static bool IsInitialized
        {
            get { return initialized && instance != null; }
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

        public static void Initialize()
        {
            if (!initialized)
            {
                if (!ScenePlaybackDetectorStub.IsPlaying) { return; }
                MainThreadDispatcher dispatcher = null;

                try
                {
                    dispatcher = GameObject.FindObjectOfType<MainThreadDispatcher>();
                }
                catch
                {
                    // Throw exception when calling from a worker thread.
                    var ex = new Exception("UniRx requires a MainThreadDispatcher component created on the main thread. Make sure it is added to the scene before calling UniRx from a worker thread.");
                    UnityEngine.Debug.LogException(ex);
                    throw ex;
                }

                if (isQuitting)
                {
                    // don't create new instance after quitting
                    // avoid "Some objects were not cleaned up when closing the scene find target" error.
                    return;
                }

                if (dispatcher == null)
                {
                    instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                }
                else
                {
                    instance = dispatcher;
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
                mainThreadToken = new object();
                initialized = true;

                // Added for consistency with Initialize()
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (cullingMode == CullingMode.Self)
                {
                    Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Removing myself...");
                    // Destroy this dispatcher if there's already one in the scene.
                    DestroyDispatcher(this);
                }
                else if (cullingMode == CullingMode.All)
                {
                    Debug.LogWarning("There is already a MainThreadDispatcher in the scene. Cleaning up all excess dispatchers...");
                    CullAllExcessDispatchers();
                }
                else
                {
                    Debug.LogWarning("There is already a MainThreadDispatcher in the scene.");
                }
            }
        }

        static void DestroyDispatcher(MainThreadDispatcher aDispatcher)
        {
            if (aDispatcher != instance)
            {
                // Try to remove game object if it's empty
                var components = aDispatcher.gameObject.GetComponents<Component>();
                if (aDispatcher.gameObject.transform.childCount == 0 && components.Length == 2)
                {
                    if (components[0] is Transform && components[1] is MainThreadDispatcher)
                    {
                        Destroy(aDispatcher.gameObject);
                    }
                }
                else
                {
                    // Remove component
                    MonoBehaviour.Destroy(aDispatcher);
                }
            }
        }

        public static void CullAllExcessDispatchers()
        {
            var dispatchers = GameObject.FindObjectsOfType<MainThreadDispatcher>();
            for (int i = 0; i < dispatchers.Length; i++)
            {
                DestroyDispatcher(dispatchers[i]);
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = GameObject.FindObjectOfType<MainThreadDispatcher>();
                initialized = instance != null;

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

        void OnLevelWasLoaded(int level)
        {
            // TODO clear queueWorker?
            //queueWorker = new ThreadSafeQueueWorker();
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
            isQuitting = true;
            if (onApplicationQuit != null) onApplicationQuit.OnNext(Unit.Default);
        }

        public static IObservable<Unit> OnApplicationQuitAsObservable()
        {
            return Instance.onApplicationQuit ?? (Instance.onApplicationQuit = new Subject<Unit>());
        }
    }
}
