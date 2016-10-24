#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7)

using UnityEngine;
using RuntimeUnitTestToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace UniRx.Tests
{
    [Preserve] // (AllMembers = true)
    public class UnityTest
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            // no more register...
            UnitTest.RegisterAllMethods<UnityTest>();
        }

        public UnityTest()
        {

        }

        [Preserve]
        public void SyncTest()
        {
            int.Parse("100").Is(100);
        }

        [Preserve]
        public IEnumerator AsyncTest()
        {
            var yi = Observable.Return(10).Delay(TimeSpan.FromSeconds(2)).ToYieldInstruction();
            yield return yi;

            yi.Result.Is(10);
        }
    }
}

#endif
