#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
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
using UniRx;
using UniRx.Async;

namespace UniRx.Tests
{
    public class _AsyncTest
    {
#if CSHARP_7_OR_LATER

        public async UniTask DelayAnd()
        {
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            var x = await UniTask.Delay(100);
            x.Is(100);
        }

        public async UniTask WhenAll()
        {
            var a = UniTask.FromResult(999);
            var b = UniTask.Yield().AsAsyncUnitUniTask();
            var c = UniTask.Delay(99);

            var (a2, b2, c2) = await UniTask.WhenAll(a, b, c);
            a2.Is(999);
            b2.Is(AsyncUnit.Default);
            c2.Is(99);
        }

        public async UniTask WhenAny()
        {
            var a = UniTask.FromResult(999);
            var b = UniTask.Yield().AsAsyncUnitUniTask();
            var c = UniTask.Delay(99);

            var (win, a2, b2, c2) = await UniTask.WhenAny(a, b, c);
            win.Is(0);
            a2.hasResult.IsTrue();
            a2.result0.Is(999);
            b2.hasResult.IsFalse();
            c2.hasResult.IsFalse();
        }

        public async UniTask BothEnumeratorCheck()
        {
            await Test(); // wait 5 frame:)
            await Test().ConfigureAwait(PlayerLoopTiming.PostLateUpdate);
        }

        IEnumerator Test()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
        }

#endif
    }
}

#endif
