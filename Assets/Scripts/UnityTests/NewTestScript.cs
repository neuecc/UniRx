#pragma warning disable CS1591

using UnityEngine.TestTools;
using System.Collections;
using System;
using UniRx.Async;
using UnityEngine;

public class NewTestScript
{
    [UnityTest]
    public IEnumerator AsyncScriptTest()
    {
        return AsyncHelper(async () =>
        {
            Debug.Log("hogemoge");
            await UniTask.Delay(10000);
            Debug.Log("hogemoge");
            "hogemoge".Is("hogemoge");
        });
    }

    public IEnumerator AsyncHelper(Func<UniTask> func)
    {
        return func().ToCoroutine();
    }
}
