#if UNITY_METRO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UniRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

        class ThreadPoolScheduler : IScheduler
        {
            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
            {
                return action(this, state);
            }

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                return Schedule(state, dueTime - Now, action);
            }

            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                var wait = Scheduler.Normalize(dueTime);

                var d = new SingleAssignmentDisposable();

                Action act = () =>
                {
                    if (!d.IsDisposed)
                    {
                        if (wait.Ticks > 0)
                        {
                            Thread.Sleep(wait);
                        }
                        d.Disposable = action(this, state);
                    }
                };

                act.BeginInvoke(ar => act.EndInvoke(ar), null);

                //System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                //{
                //    if (!d.IsDisposed)
                //    {
                //        if (wait.Ticks > 0)
                //        {
                //            Thread.Sleep(wait);
                //        }
                //        d.Disposable = action(this, state);
                //    }
                //});

                return d;
            }
        }
    }
}

#endif