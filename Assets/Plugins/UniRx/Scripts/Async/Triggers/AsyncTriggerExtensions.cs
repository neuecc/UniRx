#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using UniRx.Async;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    public static class AsyncTriggerExtensions
    {
        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        public static AsyncUpdateTrigger GetAsyncUpdateTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncUpdateTrigger>(gameObject);
        }

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        public static AsyncUpdateTrigger GetAsyncUpdateTrigger(this Component component)
        {
            return component.gameObject.GetAsyncUpdateTrigger();
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDestroyTrigger>(gameObject);
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDestroyTrigger();
        }

        static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}

#endif