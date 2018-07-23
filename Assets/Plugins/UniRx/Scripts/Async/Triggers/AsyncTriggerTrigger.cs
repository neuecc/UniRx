#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UniRx.Async.Internal;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncTriggerTrigger : MonoBehaviour
    {
        ReusablePromise<Collider> onTriggerEnter;

        /// <summary>OnTriggerEnter is called when the Collider other enters the trigger.</summary>
        void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.TrySetResult(other);
        }

        /// <summary>OnTriggerEnter is called when the Collider other enters the trigger.</summary>
        public UniTask<Collider> OnTriggerEnterAsync()
        {
            return new UniTask<Collider>(onTriggerEnter ?? (onTriggerEnter = new ReusablePromise<Collider>()));
        }

        ReusablePromise<Collider> onTriggerExit;

        /// <summary>OnTriggerExit is called when the Collider other has stopped touching the trigger.</summary>
        void OnTriggerExit(Collider other)
        {
            onTriggerExit?.TrySetResult(other);
        }

        /// <summary>OnTriggerExit is called when the Collider other has stopped touching the trigger.</summary>
        public UniTask<Collider> OnTriggerExitAsync()
        {
            return new UniTask<Collider>(onTriggerExit ?? (onTriggerExit = new ReusablePromise<Collider>()));
        }

        ReusablePromise<Collider> onTriggerStay;

        /// <summary>OnTriggerStay is called once per frame for every Collider other that is touching the trigger.</summary>
        void OnTriggerStay(Collider other)
        {
            onTriggerStay?.TrySetResult(other);
        }

        /// <summary>OnTriggerStay is called once per frame for every Collider other that is touching the trigger.</summary>
        public UniTask<Collider> OnTriggerStayAsync()
        {
            return new UniTask<Collider>(onTriggerStay ?? (onTriggerStay = new ReusablePromise<Collider>()));
        }
    }
}
#endif