using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnityRx
{
    // rx old simple scheduler

    public interface IScheduler
    {
        IDisposable Schedule(Action action);
        IDisposable Schedule(Action action, TimeSpan dueTime);
        DateTimeOffset Now { get; }
    }

    public static partial class Scheduler
    {
        public static readonly IScheduler Immediate = new ImmediateScheduler();
        public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

        class ImmediateScheduler : IScheduler
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

        class ThreadPoolScheduler : IScheduler
        {
            public IDisposable Schedule(Action action)
            {
                // TODO:BooleanDisposable
                System.Threading.ThreadPool.QueueUserWorkItem(_ => action());
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
}