using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        static object gate = new object();
        List<Action> actionList = new List<Action>();

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
            Action[] actions;
            lock (gate)
            {
                actions = actionList.ToArray();
                actionList.Clear();
            }
            foreach (var action in actions)
            {
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

        public static void Post(Action item)
        {
            lock (gate)
            {
                Instance.actionList.Add(item);
            }
        }

        new public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine_Auto(routine);
        }
    }
}