using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using UniRx;

using UnityEditor;

using UnityEngine;

namespace UniRx.Test
{
    
    public static class MainThreadSchedulerTest
    {
        [MenuItem("UniRxTest/Test MainThreadScheduler")]
        public static void TestMainThreadScheduler()
        {
            Debug.Log("Before anything On thread " + Thread.CurrentThread.ManagedThreadId);

            var result = Observable.Start(
                () =>
                {
                    Debug.Log("Started On thread " + Thread.CurrentThread.ManagedThreadId);

                    return 1;
                })
                //.ObserveOnMainThread()
                    .Wait(TimeSpan.FromSeconds(5));
            //.Subscribe(
            //    n =>
            //        {
            //            Debug.Log("Next On thread " + Thread.CurrentThread.ManagedThreadId);
            //        });

            Debug.Log("Wait with result " + result + " complete On thread " + Thread.CurrentThread.ManagedThreadId);

        }
    }
}
