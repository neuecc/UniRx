#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncCollision2DTrigger : MonoBehaviour
    {
        ReusablePromise<Collision2D> onCollisionEnter2D;

        /// <summary>Sent when an incoming collider makes contact with this object's collider (2D physics only).</summary>
        void OnCollisionEnter2D(Collision2D coll)
        {
            onCollisionEnter2D?.TrySetResult(coll);
        }

        /// <summary>Sent when an incoming collider makes contact with this object's collider (2D physics only).</summary>
        public UniTask<Collision2D> OnCollisionEnter2DAsync()
        {
            return new UniTask<Collision2D>(onCollisionEnter2D ?? (onCollisionEnter2D = new ReusablePromise<Collision2D>()));
        }

        ReusablePromise<Collision2D> onCollisionExit2D;

        /// <summary>Sent when a collider on another object stops touching this object's collider (2D physics only).</summary>
        void OnCollisionExit2D(Collision2D coll)
        {
            onCollisionExit2D?.TrySetResult(coll);
        }

        /// <summary>Sent when a collider on another object stops touching this object's collider (2D physics only).</summary>
        public UniTask<Collision2D> OnCollisionExit2DAsync()
        {
            return new UniTask<Collision2D>(onCollisionExit2D ?? (onCollisionExit2D = new ReusablePromise<Collision2D>()));
        }

        ReusablePromise<Collision2D> onCollisionStay2D;

        /// <summary>Sent each frame where a collider on another object is touching this object's collider (2D physics only).</summary>
        void OnCollisionStay2D(Collision2D coll)
        {
            onCollisionStay2D?.TrySetResult(coll);
        }

        /// <summary>Sent each frame where a collider on another object is touching this object's collider (2D physics only).</summary>
        public UniTask<Collision2D> OnCollisionStay2DAsync()
        {
            return new UniTask<Collision2D>(onCollisionStay2D ?? (onCollisionStay2D = new ReusablePromise<Collision2D>()));
        }
    }
}
#endif