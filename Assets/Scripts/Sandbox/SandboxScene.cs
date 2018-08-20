#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if CSHARP_7_OR_LATER
using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using UnityEngine.UI;
using static UniRx.Async.UnityAsyncExtensions;

public class SandboxScene : MonoBehaviour
{
    public Button buttonA;
    public Button buttonB;
    MyMyClass mc;
    ReactiveProperty<int> rp = new ReactiveProperty<int>();

    void Start()
    {
        UniTaskScheduler.PropagateOperationCanceledException = true;

        var cts = new CancellationTokenSource();
        SingleClick(cts.Token).Forget();
        MultiClick(cts.Token).Forget();
        DestroyA(cts).Forget();

        DoUpdate().Forget();
    }

    async UniTaskVoid DoUpdate()
    {
        var trigger = buttonA.GetAsyncUpdateTrigger();
        while (true)
        {
            await trigger.UpdateAsync(this.GetCancellationTokenOnDestroy());
            UnityEngine.Debug.Log("Update!");
        }
    }

    async UniTask SingleClick(CancellationToken ct)
    {
        var click = buttonA.OnClickAsync(ct);
        await click;

        Debug.Log("After Single Clicked");
    }

    async UniTask MultiClick(CancellationToken ct)
    {
        try
        {
            using (var handler = buttonA.GetAsyncClickEventHandler(CancellationToken.None))
            {
                while (true)
                {
                    var supress = await handler.OnClickAsyncSuppressCancellationThrow();
                    if (supress) return;
                    Debug.Log("MultilLicked");
                }
            }
        }
        finally
        {
            Debug.Log("Finished");
        }
    }

    async UniTask DestroyA(CancellationTokenSource cts)
    {
        await buttonB.OnClickAsync();
        cts.Cancel();
        cts.Dispose();
        Destroy(buttonA.gameObject);
    }


    async UniTask DelayForever()
    {
        Time.timeScale = 1.0f;
        var delay = UniTask.Delay(TimeSpan.FromSeconds(1)).SuppressCancellationThrow();
        while (true)
        {
            var iscancel = await delay;
            if (iscancel) return;
        }
    }

    static async UniTask OnClick(Button button, IAsyncClickEventHandler onclickHandler)
    {
        await onclickHandler.OnClickAsync();
        button.interactable = false;
    }

    async UniTaskVoid HandleDestroyEvent()
    {
        var triggerB = buttonB.GetAsyncDestroyTrigger();

        await triggerB.OnDestroyAsync();
        UnityEngine.Debug.Log("destroyed");
    }

    async UniTaskVoid HandleObserve(UniTask<int> waituntil)
    {
        await waituntil;
        //while (this != null)
        //{
        //    try
        //    {
        //        var v = await monitorTask;
        //        Debug.Log("ValueChanged:" + v);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Log("Catch!" + ex);
        //        throw;
        //    }
        //}
    }

    async UniTaskVoid HandleEventA()
    {
        try
        {
            using (var handlerA = buttonA.GetAsyncClickEventHandler())
            {
                while (this != null)
                {
                    await handlerA.OnClickAsync();
                    UnityEngine.Debug.Log("OK");
                    if (mc != null)
                    {
                        mc.MyProperty++;
                    }
                    else
                    {
                        GC.Collect();
                    }
                }
            }
        }
        finally
        {
            UnityEngine.Debug.Log("Finish Here");
        }
    }

    async UniTaskVoid HandleEventB()
    {
        using (var handlerB = buttonB.GetAsyncClickEventHandler())
        {
            while (this != null)
            {
                await handlerB.OnClickAsync();
                Destroy(buttonA.gameObject);
            }
        }
    }

    async UniTaskVoid HandleEvent()
    {
        var triggerB = buttonB.GetAsyncDestroyTrigger();

        using (var handlerA = buttonA.GetAsyncClickEventHandler())
        using (var handlerB = buttonB.GetAsyncClickEventHandler())
        {
            while (this != null)
            {
                await handlerA.OnClickAsync();




                //await handlerB.OnClickAsync();
                //Object.Destroy(buttonB.gameObject);
                //UnityEngine.Debug.Log("buttonB is destroyed");
                //return;


                //await UniTask.WhenAll(
                //    OnClick(buttonA, handlerA),
                //    OnClick(buttonB, handlerB));

                //Debug.Log("Clicked both button");

                //buttonA.interactable = true;
                //buttonB.interactable = true;

                //await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UnitTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}

public class MyMyClass
{
    public int MyProperty { get; set; }

    ~MyMyClass()
    {
        Debug.Log("GCed");
    }
}

#endif