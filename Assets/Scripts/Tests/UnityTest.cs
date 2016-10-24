#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7)

using UnityEngine;
using RuntimeUnitTestToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace UniRx.Tests
{
    public class UnityTest
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new UnityTest();
            UnitTest.AddTest(test.SyncTest);
            UnitTest.AddAsyncTest(test.AsyncTest);
        }

        public void SyncTest()
        {
            int.Parse("100").Is(100);
        }

        public IEnumerator AsyncTest()
        {
            var yi = Observable.Return(10).Delay(TimeSpan.FromSeconds(5)).ToYieldInstruction();
            yield return yi;

            yi.Result.Is(10);
        }
    }
}

#endif
