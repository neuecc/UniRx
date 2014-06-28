using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace UniRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler CurrentThread = new CurrentThreadScheduler();

        public static bool IsCurrentThreadSchedulerScheduleRequired { get { return (CurrentThread as CurrentThreadScheduler).IsScheduleRequired; } }

        class CurrentThreadScheduler : IScheduler
        {
            [ThreadStatic]
            static SchedulingPriorityQueue threadStaticQueue;

            public bool IsScheduleRequired { get { return threadStaticQueue == null; } }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action)
            {
                return Schedule(TimeSpan.Zero, action);
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action)
            {
                var queue = threadStaticQueue;
                if (queue == null)
                {
                    queue = threadStaticQueue = new SchedulingPriorityQueue();
                }

                if (queue.Count > 0)
                {
                    var d = new BooleanDisposable();
                    queue.Enqueue(() =>
                    {
                        if (!d.IsDisposed)
                        {
                            action();
                        }
                    }, Now + dueTime, d);
                    return d;
                }

                var rootCancel = new BooleanDisposable();
                queue.Enqueue(action, Now + dueTime, rootCancel);

                while (queue.Count > 0)
                {
                    Action act;
                    DateTimeOffset dt;
                    ICancelable cancel;
                    using (queue.Dequeue(out act, out dt, out cancel))
                    {
                        if (!cancel.IsDisposed)
                        {
                            var wait = Scheduler.Normalize(dt - Now);
                            if (wait.Ticks > 0)
                            {
                                Thread.Sleep(wait);
                            }
                            act();
                        }
                    }
                }

                threadStaticQueue = null;

                return rootCancel;
            }
        }

        // Pseudo PriorityQueue, Cheap implementation
        class SchedulingPriorityQueue
        {
            // make ScheduleItem
            List<Action> actions = new List<Action>();
            List<DateTimeOffset> priorities = new List<DateTimeOffset>();
            List<ICancelable> cancels = new List<ICancelable>();

            int runninngCount;

            public int Count { get { return runninngCount; } }

            public void Enqueue(Action action, DateTimeOffset time, ICancelable cancel)
            {
                // should use reverse for?
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