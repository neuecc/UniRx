#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncParticleTrigger : MonoBehaviour
    {
        ReusablePromise<GameObject> onParticleCollision;
        ReusablePromise onParticleTrigger;

        /// <summary>OnParticleCollision is called when a particle hits a collider.</summary>
        void OnParticleCollision(GameObject other)
        {
            onParticleCollision?.TryInvokeContinuation(other);
        }

        /// <summary>OnParticleCollision is called when a particle hits a collider.</summary>
        public UniTask<GameObject> OnParticleCollisionAsync()
        {
            return new UniTask<GameObject>(onParticleCollision ?? (onParticleCollision = new ReusablePromise<GameObject>()));
        }

        /// <summary>OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.</summary>
        void OnParticleTrigger()
        {
            onParticleTrigger?.TryInvokeContinuation();
        }

        /// <summary>OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.</summary>
        public UniTask OnParticleTriggerAsync()
        {
            return new UniTask(onParticleTrigger ?? (onParticleTrigger = new ReusablePromise()));
        }
    }
}
#endif