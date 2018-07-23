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
#if CSHARP_7_OR_LATER
using System.Threading.Tasks;
#endif
using UnityEngine.Networking;
using UnityEngine.Experimental.LowLevel;
#if !UNITY_WSA
using Unity.Jobs;
#endif
using Unity.Collections;
using System.Threading;

namespace UniRx.Tests
{
    public class _AsyncTest
    {
#if CSHARP_7_OR_LATER
#if !UNITY_WSA

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

        class MyMyClass
        {
            public int MyProperty { get; set; }
        }

        public async UniTask WaitUntil()
        {
            bool t = false;

            Observable.TimerFrame(10, FrameCountType.EndOfFrame).Subscribe(_ => t = true);

            var startFrame = Time.frameCount;
            await UniTask.WaitUntil(() => t, PlayerLoopTiming.EarlyUpdate);

            var diff = Time.frameCount - startFrame;
            diff.Is(11);
        }

        public async UniTask WaitWhile()
        {
            bool t = true;

            Observable.TimerFrame(10, FrameCountType.EndOfFrame).Subscribe(_ => t = false);

            var startFrame = Time.frameCount;
            await UniTask.WaitWhile(() => t, PlayerLoopTiming.EarlyUpdate);

            var diff = Time.frameCount - startFrame;
            diff.Is(11);
        }

        public async UniTask WaitUntilValueChanged()
        {
            var v = new MyMyClass { MyProperty = 99 };

            Observable.TimerFrame(10, FrameCountType.EndOfFrame).Subscribe(_ => v.MyProperty = 1000);

            var startFrame = Time.frameCount;
            await UniTask.WaitUntilValueChanged(v, x => x.MyProperty, PlayerLoopTiming.EarlyUpdate);

            var diff = Time.frameCount - startFrame;
            diff.Is(11);
        }


        public async UniTask SwitchTo()
        {
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;

            await UniTask.SwitchToThreadPool();
            await UniTask.SwitchToThreadPool();
            await UniTask.SwitchToThreadPool();

            var switchedThreadId = Thread.CurrentThread.ManagedThreadId;

            await UniTask.Yield();

            currentThreadId.IsNot(switchedThreadId);
        }

        public async UniTask ObservableConversion()
        {
            var v = await Observable.Range(1, 10).ToUniTask();
            v.Is(10);

            v = await Observable.Range(1, 10).ToUniTask(useFirstValue: true);
            v.Is(1);

            v = await UniTask.DelayFrame(10).ToObservable().ToTask();
            v.Is(10);

            v = await UniTask.FromResult(99).ToObservable();
            v.Is(99);
        }

        public async UniTask AwaitableReactiveProperty()
        {
            var rp1 = new ReactiveProperty<int>(99);

            UniTask.DelayFrame(100).ContinueWith(x => rp1.Value = x).Forget();

            await rp1;

            rp1.Value.Is(100);

            // var delay2 = UniTask.DelayFrame(10);
            // var (a, b ) = await UniTask.WhenAll(rp1.WaitUntilValueChangedAsync(), delay2);

        }

        public async UniTask AwaitableReactiveCommand()
        {
            var rc = new ReactiveCommand<int>();

            UniTask.DelayFrame(100).ContinueWith(x => rc.Execute(x)).Forget();

            var v = await rc;

            v.Is(100);
        }

        public async UniTask ExceptionlessCancellation()
        {
            var cts = new CancellationTokenSource();

            UniTask.DelayFrame(10).ContinueWith(_ => cts.Cancel()).Forget();

            var first = Time.frameCount;
            var (canceled, value) = await UniTask.DelayFrame(100, cancellationToken: cts.Token).WithIsCanceled();

            (Time.frameCount - first).Is(11); // 10 frame canceled
            canceled.IsTrue();
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
#endif
    }
}

#endif