//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace UniRx.Async.Triggers
//{
//    // TODO:trying create triggers.

//    internal class ReusablePromise<T> : IPromise<T>
//    {
//        T result;
//        int nextRegisterContinuation = 0;

//        Action singleContinuationField; // 0
//        Action[] continuationFieldA;    // 1
//        Action[] continuationFieldB;    // 2
//        int lenA;
//        int lenB;

//        public bool IsCompleted => false;

//        public T GetResult()
//        {
//            return result;
//        }

//        void EnsureCapacity(ref Action[] list, int len)
//        {
//            if (list.Length == len)
//            {
//                Array.Resize(ref list, len * 2);
//            }
//        }

//        public void TryInvokeContinuation(T result)
//        {
//            this.result = result;

//            if (singleContinuationField != null)
//            {
//                var action = singleContinuationField;
//                singleContinuationField = null; // clear.
//                nextRegisterContinuation = 0;
//                action();
//            }
//            else
//            {
//                switch (nextRegisterContinuation)
//                {
//                    case 0:
//                        return;
//                    case 1:
//                        {
//                            nextRegisterContinuation = 2;
//                            var length = lenA;
//                            lenA = 0;// clear
//                            for (int i = 0; i < length; i++)
//                            {
//                                var continuation = continuationFieldA[i];
//                                continuationFieldA[i] = null;
//                                continuation.Invoke();
//                            }
//                        }
//                        break;
//                    case 2:
//                        {
//                            nextRegisterContinuation = 1;
//                            var length = lenB;
//                            lenB = 0;// clear
//                            for (int i = 0; i < length; i++)
//                            {
//                                var continuation = continuationFieldB[i];
//                                continuationFieldB[i] = null;
//                                continuation.Invoke();
//                            }
//                        }
//                        break;
//                    default:
//                        break;
//                }
//            }
//        }

//        public void RegisterContinuation(Action newContinuation)
//        {
//            switch (nextRegisterContinuation)
//            {
//                case 0:
//                    singleContinuationField = newContinuation;
//                    nextRegisterContinuation = 1;
//                    break;
//                case 1:
//                    if (continuationFieldA == null)
//                    {
//                        continuationFieldA = new Action[4];
//                        lenA++;
//                    }
//                    if (singleContinuationField != null)
//                    {
//                        continuationFieldA[lenA++] = singleContinuationField;
//                        singleContinuationField = null;
//                    }

//                    EnsureCapacity(ref continuationFieldA, lenA + 1);
//                    continuationFieldA[lenA++] = newContinuation;
//                    break;
//                case 2:
//                    if (continuationFieldB == null)
//                    {
//                        continuationFieldB = new Action[4];
//                        lenB++;
//                    }
//                    if (continuationFieldB != null)
//                    {
//                        continuationFieldB[lenB++] = singleContinuationField;
//                        singleContinuationField = null;
//                    }

//                    EnsureCapacity(ref continuationFieldB, lenB + 1);
//                    continuationFieldB[lenB++] = newContinuation;
//                    break;

//                default:
//                    break;
//            }
//        }
//    }

//    [DisallowMultipleComponent]
//    public class AsyncUpdateTrigger : MonoBehaviour
//    {
//        ReusablePromise<AsyncUnit> promise;

//        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
//        void Update()
//        {
//            promise.TryInvokeContinuation(AsyncUnit.Default);
//        }

//        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
//        public UniTask UpdateAsync()
//        {
//            return new UniTask(promise ?? (promise = new ReusablePromise<AsyncUnit>()));
//        }
//    }
//}
