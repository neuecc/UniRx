using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx.InternalUtil;
using UnityEngine;

namespace UniRx
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        /// <summary>Dispatch Asyncrhonous action.</summary>
        public static void Post(Action item)
        {
            Instance.queueWorker.Enqueue(item);
        }

        /// <summary>Dispatch Synchronous action if possible.</summary>
        public static void Send(Action action)
        {
            if (initializedThreadToken != null)
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

        /// <summary>ThreadSafe StartCoroutine.</summary>
        public static void SendStartCoroutine(IEnumerator routine)
        {
            if (initializedThreadToken != null)
            {
                StartCoroutine(routine);
            }
            else
            {
                Instance.queueWorker.Enqueue(() => Instance.StartCoroutine_Auto(routine));
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
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

        private MainThreadDispatcher()
        {

        }

        [ThreadStatic]
        static object initializedThreadToken;

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
                if (!Application.isPlaying) return;
                initialized = true;
                instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(instance);
                initializedThreadToken = new object();
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