#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UnityEngine;
using RuntimeUnitTestToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Scripting;
using Unity.Jobs;
using System.Threading;
using Unity.Collections;

namespace UniRx.Tests
{
    public class _ManualyTest
    {
        public struct MyJob : IJob
        {
            public void Execute()
            {
                Debug.Log("Exec Begin ID:" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Debug.Log("Exec End ID:" + Thread.CurrentThread.ManagedThreadId);
            }
        }

        


        public void AsyncSandbox()
        {
            // Unity.Collections.LowLevel.Unsafe.UnsafeUtility.

            // T*

            // Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray(


            // RaycastCommand.ScheduleBatch(


            Debug.Log("Sand BEGIN ID:" + Thread.CurrentThread.ManagedThreadId);
            var handle = new MyJob().Schedule();
            JobHandle.ScheduleBatchedJobs();
            Debug.Log("Sand END ID:" + Thread.CurrentThread.ManagedThreadId);
            handle.Complete();
            Debug.Log("Sand HANDLE COMPLETE ID:" + Thread.CurrentThread.ManagedThreadId);
        }
    }

   
}

#endif
#endif

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
