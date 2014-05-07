using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UniRx
{
    public interface IScheduler
    {
        DateTimeOffset Now { get; }
        IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action);
        IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action);
        IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action);
    }

    // Scheduler Extension
    public static partial class Scheduler
    {
        // utils

        public static DateTimeOffset Now
        {
            get { return DateTimeOffset.Now; }
        }

        public static TimeSpan Normalize(TimeSpan timeSpan)
        {
            return timeSpan >= TimeSpan.Zero ? timeSpan : TimeSpan.Zero;
        }

        // schduler is too complex especially recursive.
        // the code borrow from official rx.

        static IDisposable Invoke(IScheduler scheduler, Action action)
        {
            action();
            return Disposable.Empty;
        }

        public static IDisposable Schedule(this IScheduler scheduler, Action action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(action, Invoke);
        }

        public static IDisposable Schedule(this IScheduler scheduler, TimeSpan dueTime, Action action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(action, dueTime, Invoke);
        }
        public static IDisposable Schedule(this IScheduler scheduler, DateTimeOffset dueTime, Action action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(action, dueTime, Invoke);
        }

        public static IDisposable Schedule(this IScheduler scheduler, Action<Action> action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(action, (Action<Action> _action, Action<Action<Action>> self) => _action(() => self(_action)));
        }

        public static IDisposable Schedule<TState>(this IScheduler scheduler, TState state, Action<TState, Action<TState>> action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(new Pair<TState, Action<TState, Action<TState>>> { First = state, Second = action }, InvokeRec1);
        }

        static IDisposable InvokeRec1<TState>(IScheduler scheduler, Pair<TState, Action<TState, Action<TState>>> pair)
        {
            var group = new CompositeDisposable(1);
            var gate = new object();
            var state = pair.First;
            var action = pair.Second;

            Action<TState> recursiveAction = null;
            recursiveAction = state1 => action(state1, state2 =>
            {
                var isAdded = false;
                var isDone = false;
                var d = default(IDisposable);
                d = scheduler.Schedule(state2, (scheduler1, state3) =>
                {
                    lock (gate)
                    {
                        if (isAdded)
                            group.Remove(d);
                        else
                            isDone = true;
                    }
                    recursiveAction(state3);
                    return Disposable.Empty;
                });

                lock (gate)
                {
                    if (!isDone)
                    {
                        group.Add(d);
                        isAdded = true;
                    }
                }
            });

            recursiveAction(state);

            return group;
        }

        public static IDisposable Schedule(this IScheduler scheduler, TimeSpan dueTime, Action<Action<TimeSpan>> action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(action, dueTime, (Action<Action<TimeSpan>> _action, Action<Action<Action<TimeSpan>>, TimeSpan> self) => _action(dt => self(_action, dt)));
        }

        public static IDisposable Schedule<TState>(this IScheduler scheduler, TState state, TimeSpan dueTime, Action<TState, Action<TState, TimeSpan>> action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(new Pair<TState, Action<TState, Action<TState, TimeSpan>>> { First = state, Second = action }, dueTime, InvokeRec2);
        }

        static IDisposable InvokeRec2<TState>(IScheduler scheduler, Pair<TState, Action<TState, Action<TState, TimeSpan>>> pair)
        {
            var group = new CompositeDisposable(1);
            var gate = new object();
            var state = pair.First;
            var action = pair.Second;

            Action<TState> recursiveAction = null;
            recursiveAction = state1 => action(state1, (state2, dueTime1) =>
            {
                var isAdded = false;
                var isDone = false;
                var d = default(IDisposable);
                d = scheduler.Schedule(state2, dueTime1, (scheduler1, state3) =>
                {
                    lock (gate)
                    {
                        if (isAdded)
                            group.Remove(d);
                        else
                            isDone = true;
                    }
                    recursiveAction(state3);
                    return Disposable.Empty;
                });

                lock (gate)
                {
                    if (!isDone)
                    {
                        group.Add(d);
                        isAdded = true;
                    }
                }
            });

            recursiveAction(state);

            return group;
        }

        public static IDisposable Schedule(this IScheduler scheduler, DateTimeOffset dueTime, Action<Action<DateTimeOffset>> action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(action, dueTime, (Action<Action<DateTimeOffset>> _action, Action<Action<Action<DateTimeOffset>>, DateTimeOffset> self) => _action(dt => self(_action, dt)));
        }

        public static IDisposable Schedule<TState>(this IScheduler scheduler, TState state, DateTimeOffset dueTime, Action<TState, Action<TState, DateTimeOffset>> action)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");
            if (action == null) throw new ArgumentNullException("action");

            return scheduler.Schedule(new Pair<TState, Action<TState, Action<TState, DateTimeOffset>>> { First = state, Second = action }, dueTime, InvokeRec3);
        }

        static IDisposable InvokeRec3<TState>(IScheduler scheduler, Pair<TState, Action<TState, Action<TState, DateTimeOffset>>> pair)
        {
            var group = new CompositeDisposable(1);
            var gate = new object();
            var state = pair.First;
            var action = pair.Second;

            Action<TState> recursiveAction = null;
            recursiveAction = state1 => action(state1, (state2, dueTime1) =>
            {
                var isAdded = false;
                var isDone = false;
                var d = default(IDisposable);
                d = scheduler.Schedule(state2, dueTime1, (scheduler1, state3) =>
                {
                    lock (gate)
                    {
                        if (isAdded)
                            group.Remove(d);
                        else
                            isDone = true;
                    }
                    recursiveAction(state3);
                    return Disposable.Empty;
                });

                lock (gate)
                {
                    if (!isDone)
                    {
                        group.Add(d);
                        isAdded = true;
                    }
                }
            });

            recursiveAction(state);

            return group;
        }

        [Serializable]
        struct Pair<T1, T2>
        {
            public T1 First;
            public T2 Second;
        }
    }
}