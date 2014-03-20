using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnityRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler Immediate = new ImmediateScheduler();
        
        class ImmediateScheduler : IScheduler
        {
            public ImmediateScheduler()
            {
            }

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
                Thread.Sleep(Scheduler.Normalize(dueTime));
                return action(this, state);
            }
        }
    }
}