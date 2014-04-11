using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace UnityRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler CurrentThread = new CurrentThreadScheduler();

        class CurrentThreadScheduler : IScheduler
        {
            [ThreadStatic]
            static SchedulingPriorityQueue threadStaticQueue;

            SchedulingPriorityQueue GetQueue()
            {
                if (threadStaticQueue == null)
                {
                    threadStaticQueue = new SchedulingPriorityQueue();
                }
                return threadStaticQueue;
            }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
            {
                return Schedule<TState>(state, Now, action);
            }
            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                return Schedule(state, Now + dueTime, action);
            }

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                var queue = GetQueue();

                if (queue.Count > 0)
                {
                    var b = new BooleanDisposable();
                    queue.Enqueue(() =>
                    {
                        if (!b.IsDisposed)
                        {
                            action(this, state);
                        }
                    }, dueTime);
                    return b;
                }

                queue.Enqueue(() => action(this, state), dueTime);

                while (queue.Count > 0)
                {
                    Action act;
                    DateTimeOffset dt;
                    using (queue.Dequeue(out act, out dt))
                    {
                        var wait = Scheduler.Normalize(dt - Now);
                        if (wait.Ticks > 0)
                        {
                            Thread.Sleep(wait);
                        }
                        act();
                    }
                }

                return Disposable.Empty;
            }
        }

        // Pseudo PriorityQueue, Cheap implementation
        class SchedulingPriorityQueue
        {
            List<Action> actions = new List<Action>();
            List<DateTimeOffset> priorities = new List<DateTimeOffset>();

            int runninngCount;

            public int Count { get { return runninngCount; } }

            public void Enqueue(Action action, DateTimeOffset time)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    if (priorities[i] > time)
                    {
                        actions.Insert(i, action);
                        priorities.Insert(i, time);
                        runninngCount++;
                        return;
                    }
                }

                actions.Add(action);
                priorities.Add(time);
                runninngCount++;
            }

            public IDisposable Dequeue(out Action action, out DateTimeOffset time)
            {
                if (actions.Count == 0) throw new InvalidOperationException();

                action = actions[0];
                time = priorities[0];
                actions.RemoveAt(0);
                priorities.RemoveAt(0);

                return Disposable.Create(() => runninngCount--);
            }
        }
    }
}