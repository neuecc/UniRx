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
        public static readonly IScheduler MainThread = new MainThreadScheduler();

        class MainThreadScheduler : IScheduler
        {
            public MainThreadScheduler()
            {
                MainThreadDispatcher.Initialize();
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action)
            {
                yield return new WaitForSeconds((float)dueTime.TotalSeconds);
                MainThreadDispatcher.Post(action);
            }

            public DateTimeOffset Now
            {
                get { return DateTimeOffset.Now; }
            }

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
            {
                var d = new SingleAssignmentDisposable();
                MainThreadDispatcher.Post(() =>
                {
                    if (!d.IsDisposed)
                    {
                        d.Disposable = action(this, state);
                    }
                });
                return d;
            }

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                return Schedule(state, dueTime - Now, action);
            }

            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                var d = new SingleAssignmentDisposable();
                MainThreadDispatcher.StartCoroutine(DelayAction(dueTime, () =>
                {
                    if (!d.IsDisposed)
                    {
                        d.Disposable = action(this, state);
                    }
                }));

                return d;
            }
        }
    }
}