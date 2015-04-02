using System;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

namespace UniRx
{
    public static partial class DisposableExtensions
    {
        /// <summary>Dispose self on target gameObject has been destroyed. Return value is self disposable.</summary>
        public static IDisposable AddTo(this IDisposable disposable, GameObject gameObject)
        {
            if (gameObject == null)
            {
                disposable.Dispose();
                return disposable;
            }

            var trigger = gameObject.GetComponent<ObservableDestroyTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<ObservableDestroyTrigger>();
            }

            trigger.OnDestroyAsObservable().Subscribe(_ => disposable.Dispose());
            return disposable;
        }

        /// <summary>Dispose self on target gameObject has been destroyed. Return value is self disposable.</summary>
        public static IDisposable AddTo(this IDisposable disposable, Component gameObjectComponent)
        {
            if (gameObjectComponent == null)
            {
                disposable.Dispose();
                return disposable;
            }

            return AddTo(disposable, gameObjectComponent.gameObject);
        }

        /// <summary>
        /// <para>Add disposable(self) to CompositeDisposable(or other ICollection) and Dispose self on target gameObject has been destroyed.</para>
        /// <para>Return value is self disposable.</para>
        /// </summary>
        public static IDisposable AddTo(this IDisposable disposable, ICollection<IDisposable> container, GameObject gameObject)
        {
            return disposable.AddTo(container).AddTo(gameObject);
        }

        /// <summary>
        /// <para>Add disposable(self) to CompositeDisposable(or other ICollection) and Dispose self on target gameObject has been destroyed.</para>
        /// <para>Return value is self disposable.</para>
        /// </summary>
        public static IDisposable AddTo(this IDisposable disposable, ICollection<IDisposable> container, Component gameObjectComponent)
        {
            return disposable.AddTo(container).AddTo(gameObjectComponent);
        }
    }
}