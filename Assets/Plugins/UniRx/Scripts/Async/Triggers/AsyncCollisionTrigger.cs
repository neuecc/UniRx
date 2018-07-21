#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncCollisionTrigger : MonoBehaviour
    {
        ReusablePromise<Collision> onCollisionEnter;

        /// <summary>OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.</summary>
         void OnCollisionEnter(Collision collision)
        {
            onCollisionEnter?.TryInvokeContinuation(collision);
        }

        /// <summary>OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.</summary>
        public UniTask<Collision> OnCollisionEnterAsync()
        {
            return new UniTask<Collision>(onCollisionEnter ?? (onCollisionEnter = new ReusablePromise<Collision>()));
        }

        ReusablePromise<Collision> onCollisionExit;

        /// <summary>OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.</summary>
         void OnCollisionExit(Collision collisionInfo)
        {
            onCollisionExit?.TryInvokeContinuation(collisionInfo);
        }

        /// <summary>OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.</summary>
        public UniTask<Collision> OnCollisionExitAsync()
        {
            return new UniTask<Collision>(onCollisionExit ?? (onCollisionExit = new ReusablePromise<Collision>()));
        }

        ReusablePromise<Collision> onCollisionStay;

        /// <summary>OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.</summary>
         void OnCollisionStay(Collision collisionInfo)
        {
            onCollisionStay?.TryInvokeContinuation(collisionInfo);
        }

        /// <summary>OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.</summary>
        public UniTask<Collision> OnCollisionStayAsync()
        {
            return new UniTask<Collision>(onCollisionStay ?? (onCollisionStay = new ReusablePromise<Collision>()));
        }
    }
}
#endif