#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if CSHARP_7_OR_LATER
using System.Threading;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using UnityEngine.UI;
using static UniRx.Async.UnityAsyncExtensions;

public class SandboxScene : MonoBehaviour
{
    public Button buttonA;
    public Button buttonB;

    void Start()
    {
        HandleDestroyEvent().Forget();
        HandleEvent().Forget();
        // HandleUpdateLoop().Forget();
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

    async UniTaskVoid HandleUpdateLoop()
    {
        var trigger = this.GetAsyncUpdateTrigger();
        while (this != null)
        {
            await trigger.UpdateAsync();
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

                await handlerB.OnClickAsync();
                Object.Destroy(buttonB.gameObject);
                UnityEngine.Debug.Log("buttonB is destroyed");
                return;


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
#endif