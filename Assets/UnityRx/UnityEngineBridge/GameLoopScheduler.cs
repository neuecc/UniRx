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
        // TODO:UnitTest
        // public static readonly IScheduler GameLoop = new GameLoopScheduler();

        class GameLoopScheduler : IScheduler
        {
            public GameLoopScheduler()
            {
                GameLoopDispatcher.Initialize();
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action)
            {
                yield return new WaitForSeconds((float)dueTime.TotalSeconds);
                GameLoopDispatcher.Post(action);
            }

            public DateTimeOffset Now
            {
                get { return DateTimeOffset.Now; }
            }

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
            {
                var d = new SingleAssignmentDisposable();
                GameLoopDispatcher.Post(() =>
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
                GameLoopDispatcher.StartCoroutine(DelayAction(dueTime, () =>
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