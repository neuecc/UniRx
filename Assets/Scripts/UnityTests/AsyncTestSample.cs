#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591

using UnityEngine.TestTools;
using System.Collections;
using System;
using UniRx.Async;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.Networking;

public class AsyncTestSample
{
#if ENABLE_WWW
#pragma warning disable CS0618

    [UnityTest]
    public IEnumerator AsyncTest() => UniTask.ToCoroutine(async () =>
    {
        var flag = true;

        var initialTime = DateTime.UtcNow;

        // wait 1sec
        UniTask.Delay(TimeSpan.FromSeconds(1)).ContinueWith(() => flag = false).Forget();

        await UniTask.WaitWhile(() => flag);

        // IsApproximatelyEqual
        (DateTime.UtcNow - initialTime).TotalMilliseconds.IsApproximatelyEqual(1000, 10f);

        // WWW Get
        var req = await UnityWebRequest.Get("http://google.co.jp/").SendWebRequest();
        req.downloadHandler.text.Contains("<title>Google</title>").IsTrue();
    });

#pragma warning restore CS0618
#endif
}

#endif