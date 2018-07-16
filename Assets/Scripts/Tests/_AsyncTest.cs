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
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.Experimental.LowLevel;
using Unity.Jobs;
using Unity.Collections;

namespace UniRx.Tests
{
    public class _AsyncTest
    {
#if CSHARP_7_OR_LATER

        public struct MyJob : IJob
        {
            public int loopCount;
            public NativeArray<int> inOut;
            public int result;

            public void Execute()
            {
                result = 0;
                for (int i = 0; i < loopCount; i++)
                {
                    result++;
                }
                inOut[0] = result;
            }
        }

        public async UniTask DelayAnd()
        {
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

            var time = Time.realtimeSinceStartup;

            Time.timeScale = 0.5f;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(3));

                var elapsed = Time.realtimeSinceStartup - time;
                ((int)Math.Round(TimeSpan.FromSeconds(elapsed).TotalSeconds, MidpointRounding.ToEven)).Is(6);
            }
            finally
            {
                Time.timeScale = 1.0f;
            }
        }

        public async UniTask DelayIgnore()
        {
            var time = Time.realtimeSinceStartup;

            Time.timeScale = 0.5f;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(3), ignoreTimeScale: true);

                var elapsed = Time.realtimeSinceStartup - time;
                ((int)Math.Round(TimeSpan.FromSeconds(elapsed).TotalSeconds, MidpointRounding.ToEven)).Is(3);
            }
            finally
            {
                Time.timeScale = 1.0f;
            }
        }

        public async UniTask WhenAll()
        {
            var a = UniTask.FromResult(999);
            var b = UniTask.Yield().AsAsyncUnitUniTask();
            var c = UniTask.DelayFrame(99);

            var (a2, b2, c2) = await UniTask.WhenAll(a, b, c);
            a2.Is(999);
            b2.Is(AsyncUnit.Default);
            c2.Is(99);
        }

        public async UniTask WhenAny()
        {
            var a = UniTask.FromResult(999);
            var b = UniTask.Yield().AsAsyncUnitUniTask();
            var c = UniTask.DelayFrame(99);

            var (win, a2, b2, c2) = await UniTask.WhenAny(a, b, c);
            win.Is(0);
            a2.hasResult.IsTrue();
            a2.result0.Is(999);
            b2.hasResult.IsFalse();
            c2.hasResult.IsFalse();
        }

        public async UniTask BothEnumeratorCheck()
        {
            await ToaruCoroutineEnumerator(); // wait 5 frame:)
            await ToaruCoroutineEnumerator().ConfigureAwait(PlayerLoopTiming.PostLateUpdate);
        }

        public async UniTask JobSystem()
        {
            var job = new MyJob() { loopCount = 999, inOut = new NativeArray<int>(1, Allocator.TempJob) };
            await job.Schedule();
            job.inOut[0].Is(999);
            job.inOut.Dispose();
        }

        IEnumerator ToaruCoroutineEnumerator()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
        }

        // to public, sandbox of testing.
        async UniTask Do()
        {
            await DemoAsync();
        }

        // You can return type as struct UniTask<T>, it is unity specialized lightweight alternative of Task<T>
        // no(or less) allocation and fast excution for zero overhead async/await integrate with Unity
        async UniTask<string> DemoAsync()
        {
            // You can await Unity's AsyncObject
            var asset = await Resources.LoadAsync<TextAsset>("foo");

            // .ConfigureAwait accepts progress callback
            await SceneManager.LoadSceneAsync("scene2").ConfigureAwait(new Progress<float>(x => Debug.Log(x)));

            // await frame-based operation(You can also pass TimeSpan, it is same as calculate frame-based)
            await UniTask.Delay(100); // be careful, arg is not millisecond, is frame count

            // like 'yield return WaitForEndOfFrame', or Rx's ObserveOn(scheduler)
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

            // You can await standard task
            await Task.Run(() => 100);

            // You can await IEnumerator coroutine
            await ToaruCoroutineEnumerator();

            // get async webrequest
            async UniTask<string> GetTextAsync(UnityWebRequest req)
            {
                var op = await req.SendWebRequest();
                return op.downloadHandler.text;
            }

            var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
            var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
            var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));

            // concurrent async-wait and get result easily by tuple syntax
            var (google, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);

            // You can handle timeout easily
            await GetTextAsync(UnityWebRequest.Get("http://unity.com")).Timeout(TimeSpan.FromMilliseconds(300));

            // return async-value.(or you can use `UniTask`(no result), `UniTaskVoid`(fire and forget)).
            return (asset as TextAsset)?.text ?? throw new InvalidOperationException("Asset not found");
        }


        public async Task<string> GetTextAsync(string path)
        {
            var asset = await Resources.LoadAsync<TextAsset>(path);
            return (asset as TextAsset).text;
        }



#endif
    }

    // ...for readme

    public class SceneAssets
    {
        public readonly UniTask<Sprite> Front;
        public readonly UniTask<Sprite> Background;
        public readonly UniTask<Sprite> Effect;

        public SceneAssets()
        {
            // ctor(Func) overload is AsyncLazy, initialized once when await.
            // and after it, await returns zero-allocation value immediately.
            Front = new UniTask<Sprite>(() => LoadAsSprite("foo"));
            Background = new UniTask<Sprite>(() => LoadAsSprite("bar"));
            Effect = new UniTask<Sprite>(() => LoadAsSprite("baz"));
        }

        async UniTask<Sprite> LoadAsSprite(string path)
        {
            var resource = await Resources.LoadAsync<Sprite>(path);
            return (resource as Sprite);
        }
    }

    public class MyClass
    {
        public UniTask<int> WrapByPromise1()
        {
            var promise = new Promise<int>((resolve, reject) =>
            {
                // when complete, call resolve.SetResult();
                // when failed, call reject.SetException();
            });

            return new UniTask<int>(promise);
        }

        public UniTask<int> WrapByPromise2()
        {
            // also allows outer methods(no use constructor resolve/reject).
            var promise = new Promise<int>();

            // when complete, call promise.SetResult();
            // when failed, call promise.SetException();
            // when cancel, call promise.SetCancel();

            return new UniTask<int>(promise);
        }
    }
}

#endif
