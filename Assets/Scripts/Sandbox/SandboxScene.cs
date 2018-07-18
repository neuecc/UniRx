#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if CSHARP_7_OR_LATER
using System.Threading;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using static UniRx.Async.UnityAsyncExtensions;

public class SandboxScene : MonoBehaviour
{
    public Button buttonA;
    public Button buttonB;

    void Start()
    {
        HandleEvent().Forget();
    }

    static async UniTask OnClick(Button button, IAsyncButtonClickEventHandler onclickHandler)
    {
        await onclickHandler.OnClickAsync();
        button.interactable = false;
    }

    async UniTaskVoid HandleEvent()
    {
        using (var handlerA = buttonA.GetAsyncEventHandler())
        using (var handlerB = buttonB.GetAsyncEventHandler())
        {
            while (this != null)
            {
                await UniTask.WhenAll(
                    OnClick(buttonA, handlerA),
                    OnClick(buttonB, handlerB));

                Debug.Log("Clicked both button");

                buttonA.interactable = true;
                buttonB.interactable = true;

                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UnitTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}
#endif