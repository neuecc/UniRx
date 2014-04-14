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

        public static bool IsCurrentThreadSchedulerScheduleRqequired { get { return (CurrentThread as CurrentThreadScheduler).IsScheduleRequired; } }

        class CurrentThreadScheduler : IScheduler
        {
            [ThreadStatic]
            static SchedulingPriorityQueue threadStaticQueue;

            // TODO:Queue should set to null(guard memory leak)
            SchedulingPriorityQueue GetQueue()
            {
                if (threadStaticQueue == null)
                {
                    threadStaticQueue = new SchedulingPriorityQueue();
                }
                return threadStaticQueue;
            }

            public bool IsScheduleRequired { get { return GetQueue().Count == 0; } }

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
                    var d = new SingleAssignmentDisposable();
                    queue.Enqueue(() =>
                    {
                        if (!d.IsDisposed)
                        {
                            d.Disposable = action(this, state);
                        }
                    }, dueTime, d);
                    return d;
                }

                var rootCancel = new BooleanDisposable();
                queue.Enqueue(() => action(this, state), dueTime, rootCancel);

                while (queue.Count > 0)
                {
                    Action act;
                    DateTimeOffset dt;
                    ICancelable cancel;
                    using (queue.Dequeue(out act, out dt, out cancel))
                    {
                        var wait = Scheduler.Normalize(dt - Now);
                        if (wait.Ticks > 0)
                        {
                            Thread.Sleep(wait);
                        }
                        act();
                    }
                }

                return rootCancel;
            }
        }

        // Pseudo PriorityQueue, Cheap implementation
        class SchedulingPriorityQueue
        {
            // TODO:make ScheduleItem
            List<Action> actions = new List<Action>();
            List<DateTimeOffset> priorities = new List<DateTimeOffset>();
            List<ICancelable> cancels = new List<ICancelable>();

            int runninngCount;

            public int Count { get { return runninngCount; } }

            public void Enqueue(Action action, DateTimeOffset time, ICancelable cancel)
            {
                // TODO:reverse for?
                for (int i = 0; i < actions.Count; i++)
                {
                    if (priorities[i] > time)
                    {
                        actions.Insert(i, action);
                        priorities.Insert(i, time);
                        cancels.Insert(i, cancel);
                        runninngCount++;
                        return;
                    }
                }

                actions.Add(action);
                priorities.Add(time);
                cancels.Add(cancel);
                runninngCount++;
            }

            public IDisposable Dequeue(out Action action, out DateTimeOffset time, out ICancelable cancel)
            {
                if (actions.Count == 0) throw new InvalidOperationException();

                action = actions[0];
                time = priorities[0];
                cancel = cancels[0];
                actions.RemoveAt(0);
                priorities.RemoveAt(0);
                cancels.RemoveAt(0);

                return Disposable.Create(() => runninngCount--);
            }
        }
    }
}