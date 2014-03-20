using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UnityRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler GameLoop = new GameLoopScheduler();

        class GameLoopScheduler : IScheduler
        {
            public GameLoopScheduler()
            {
                GameLoopDispatcher.Initialize();
            }

            IEnumerator DelayAction(Action action, TimeSpan dueTime)
            {
                yield return new WaitForSeconds((float)dueTime.TotalSeconds);
                GameLoopDispatcher.Post(action);
            }

            public IDisposable Schedule(Action action)
            {
                GameLoopDispatcher.Post(action);
                return Disposable.Empty;
            }

            public IDisposable Schedule(Action action, TimeSpan dueTime)
            {
                GameLoopDispatcher.StartCoroutine(DelayAction(action, dueTime));
                return Disposable.Empty;
            }

            public DateTimeOffset Now
            {
                get { return DateTimeOffset.Now; }
            }
        }
    }
}