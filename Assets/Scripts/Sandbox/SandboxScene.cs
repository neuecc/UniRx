#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if CSHARP_7_OR_LATER
using System;
using System.Threading;
using System.Threading.Tasks;
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

    void Start()
    {
        var _ = RunAsync();
    }

    private async UniTask RunAsync()
    {
        await UniTask.Yield();
        var a = RunAsyncInternal();
        try
        {
            await a;
        }
        finally
        {
            Debug.Log("RunAsync status= " + a.Status);
        }
    }

    private async UniTask RunAsyncInternal()
    {
        await UniTask.Yield();
        var tcs = new UniTaskCompletionSource();
        tcs.TrySetCanceled();
        try
        {
            await tcs.Task;
        }
        finally
        {
            Debug.Log("RunAsyncInternal status= " + tcs.Task.Status);
        }
    }

    async void DoAsync()
    {
        // await UniTask.Yield();

        Debug.Log(Thread.CurrentThread.ManagedThreadId);

        await UniTask.SwitchToTaskPool();




        Debug.Log("WHOO=" + Thread.CurrentThread.ManagedThreadId);
        Thread.Sleep(TimeSpan.FromSeconds(3));
        Debug.Log("END OF SLEEP" + Thread.CurrentThread.ManagedThreadId);

        await UniTask.Yield();

        Debug.Log(Thread.CurrentThread.ManagedThreadId);
    }



    async UniTask DelayForever()
    {
        Time.timeScale = 1.0f;
        var delay = UniTask.Delay(TimeSpan.FromSeconds(1));
        var count = 0;
        while (this != null)
        {
            await delay;
            UnityEngine.Debug.Log("delay:" + count++ + " " + Time.realtimeSinceStartup);
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
        using (var handlerA = buttonA.GetAsyncClickEventHandler())
        {
            while (this != null)
            {
                await handlerA.OnClickAsync();
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

    async UniTaskVoid HandleEventB()
    {
        using (var handlerB = buttonB.GetAsyncClickEventHandler())
        {
            while (this != null)
            {
                await handlerB.OnClickAsync();
                mc = null;
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