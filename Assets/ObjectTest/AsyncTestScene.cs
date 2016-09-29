#if (ENABLE_MONO_BLEEDING_EDGE_EDITOR || ENABLE_MONO_BLEEDING_EDGE_STANDALONE)

using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ObjectTest
{
    public class AsyncTestScene : MonoBehaviour
    {
        public Button button1;
        public Button button2;
        public Button button3;

        void Awake()
        {
            MainThreadDispatcher.Initialize();

            button1.OnClickAsObservable().Subscribe(async _ =>
            {
                await StandardWWW();
            });

            button2.OnClickAsObservable().Subscribe(async _ =>
            {
                await ThraedingError();
            });

            button3.OnClickAsObservable().Subscribe(async _ =>
            {
                await UniRxSynchronizationContextSolves();
            });
        }


        async Task StandardWWW()
        {
            Debug.Log("start www");

            var result = await ObservableWWW.Get("https://unity3d.com");

            Debug.Log(result);
        }

        async Task ThraedingError()
        {
            Debug.Log("start delay 1");

            await Task.Delay(TimeSpan.FromMilliseconds(300)).ConfigureAwait(false);

            Debug.Log("from another thread, can't touch transform position.");
            Debug.Log(this.transform.position);
        }

        async Task UniRxSynchronizationContextSolves()
        {
            Debug.Log("start delay 2");

            // UniRxSynchronizationContext is automatically used.
            await Task.Delay(TimeSpan.FromMilliseconds(300));

            Debug.Log("from another thread, but you can touch transform position.");
            Debug.Log(this.transform.position);
        }
    }
}


#endif