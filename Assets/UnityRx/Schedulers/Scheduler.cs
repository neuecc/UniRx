using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UnityRx
{
    // rx old simple scheduler

    public interface IScheduler
    {
        IDisposable Schedule(Action action);
        IDisposable Schedule(Action action, TimeSpan dueTime);
        DateTimeOffset Now { get; }
    }

    public static class Scheduler
    {
        public static readonly GameLoopScheduler GameLoop = new GameLoopScheduler();
        public static readonly ImmediateScheduler Immediate = new ImmediateScheduler();
        public static readonly ThreadPoolScheduler ThreadPool = new ThreadPoolScheduler();
    }

    public class GameLoopScheduler : IScheduler
    {
        public GameLoopScheduler()
        {
            var _ = GameLoopDispatcher.Instance;
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

    public class ImmediateScheduler : IScheduler
    {
        public IDisposable Schedule(Action action)
        {
            action();
            return Disposable.Empty;
        }

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            System.Threading.Thread.Sleep(dueTime);
            action();
            return Disposable.Empty;
        }

        public DateTimeOffset Now
        {
            get { return DateTimeOffset.Now; }
        }
    }

    public class ThreadPoolScheduler : IScheduler
    {
        public IDisposable Schedule(Action action)
        {
            // TODO:BooleanDisposable
            ThreadPool.QueueUserWorkItem(_ => action());
            return Disposable.Empty;
        }

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            var timer = new System.Threading.Timer(_ => action(), null, dueTime, TimeSpan.Zero); // TODO:is period Zero?

            // TODO:timer dispose
            return Disposable.Empty;
        }

        public DateTimeOffset Now
        {
            get { return DateTimeOffset.Now; }
        }
    }
}