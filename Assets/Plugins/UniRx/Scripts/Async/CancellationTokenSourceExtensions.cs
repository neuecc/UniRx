#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
using UniRx.Async.Triggers;
using System;

namespace UniRx.Async
{
    public static class CancellationTokenSourceExtensions
    {
        public static void CancelAfterSlim(this CancellationTokenSource cts, int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
        {
            var delay = UniTask.Delay(millisecondsDelay, ignoreTimeScale, delayTiming);
            CancelAfterCore(cts, delay).Forget();
        }

        public static void CancelAfterSlim(this CancellationTokenSource cts, TimeSpan delayTimeSpan, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
        {
            var delay = UniTask.Delay(delayTimeSpan, ignoreTimeScale, delayTiming);
            CancelAfterCore(cts, delay).Forget();
        }

        static async UniTaskVoid CancelAfterCore(CancellationTokenSource cts, UniTask delayTask)
        {
            await delayTask;
            cts.Cancel();
        }

        public static void RegisterRaiseCancelOnDestroy(this CancellationTokenSource cts, Component component)
        {
            RegisterRaiseCancelOnDestroy(cts, component.gameObject);
        }

        public static void RegisterRaiseCancelOnDestroy(this CancellationTokenSource cts, GameObject gameObject)
        {
            var trigger = gameObject.GetAsyncDestroyTrigger();
            trigger.AddCancellationTriggerOnDestory(cts);
        }
    }
}

#endif