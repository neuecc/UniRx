using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        static object gate = new object();
        Queue<Action> actionQueue = new Queue<Action>();

        static MainThreadDispatcher instance;
        static bool initialized;

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
                if (!Application.isPlaying) return;
                initialized = true;
                instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(instance);
            }

        }

        void Awake()
        {
            instance = this;
            initialized = true;
        }

        public void Update()
        {
            lock (gate)
            {
                while (actionQueue.Count != 0)
                {
                    var action = actionQueue.Dequeue();
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex); // Is log can't handle...?
                    }
                }
            }
        }

        public static void Post(Action item)
        {
            lock (gate)
            {
                Instance.actionQueue.Enqueue(item);
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
            lock (gate)
            {
                return Instance.StartCoroutine_Auto(routine);
            }
        }
    }
}