#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using UniRx.Async;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    public static class AsyncTriggerExtensions
    {
        /// <summary>Get for OnUpdateAsync.</summary>
        public static AsyncUpdateTrigger GetAsyncUpdateTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncUpdateTrigger>(gameObject);
        }

        /// <summary>Get for OnUpdateAsync.</summary>
        public static AsyncUpdateTrigger GetAsyncUpdateTrigger(this Component component)
        {
            return component.gameObject.GetAsyncUpdateTrigger();
        }

        /// <summary>Get for OnDestroyAsync.</summary>
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDestroyTrigger>(gameObject);
        }

        /// <summary>Get for OnDestroyAsync.</summary>
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDestroyTrigger();
        }

        /// <summary>Get for OnAnimatorIKAsync | OnAnimatorMoveAsync.</summary>
        public static AsyncAnimatorTrigger GetAsyncAnimatorTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncAnimatorTrigger>(gameObject);
        }

        /// <summary>Get for OnAnimatorIKAsync | OnAnimatorMoveAsync.</summary>
        public static AsyncAnimatorTrigger GetAsyncAnimatorTrigger(this Component component)
        {
            return component.gameObject.GetAsyncAnimatorTrigger();
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