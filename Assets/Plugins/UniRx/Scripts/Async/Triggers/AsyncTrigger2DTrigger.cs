#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncTrigger2DTrigger : MonoBehaviour
    {
        ReusablePromise<Collider2D> onTriggerEnter2D;

        /// <summary>Sent when another object enters a trigger collider attached to this object (2D physics only).</summary>
        void OnTriggerEnter2D(Collider2D other)
        {
            onTriggerEnter2D?.TryInvokeContinuation(other);
        }

        /// <summary>Sent when another object enters a trigger collider attached to this object (2D physics only).</summary>
        public UniTask<Collider2D> OnTriggerEnter2DAsync()
        {
            return new UniTask<Collider2D>(onTriggerEnter2D ?? (onTriggerEnter2D = new ReusablePromise<Collider2D>()));
        }

        ReusablePromise<Collider2D> onTriggerExit2D;

        /// <summary>Sent when another object leaves a trigger collider attached to this object (2D physics only).</summary>
        void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExit2D?.TryInvokeContinuation(other);
        }

        /// <summary>Sent when another object leaves a trigger collider attached to this object (2D physics only).</summary>
        public UniTask<Collider2D> OnTriggerExit2DAsync()
        {
            return new UniTask<Collider2D>(onTriggerExit2D ?? (onTriggerExit2D = new ReusablePromise<Collider2D>()));
        }

        ReusablePromise<Collider2D> onTriggerStay2D;

        /// <summary>Sent each frame where another object is within a trigger collider attached to this object (2D physics only).</summary>
        void OnTriggerStay2D(Collider2D other)
        {
            onTriggerStay2D?.TryInvokeContinuation(other);
        }

        /// <summary>Sent each frame where another object is within a trigger collider attached to this object (2D physics only).</summary>
        public UniTask<Collider2D> OnTriggerStay2DAsync()
        {
            return new UniTask<Collider2D>(onTriggerStay2D ?? (onTriggerStay2D = new ReusablePromise<Collider2D>()));
        }
    }
}
#endif