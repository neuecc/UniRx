// Adapted from https://github.com/Reactive-Extensions/Rx.NET/blob/ba98e6254c9a2f841cbc4169bf38590b133c8064/Rx.NET/Source/src/System.Reactive/Concurrency/VirtualTimeScheduler.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file at https://github.com/Reactive-Extensions/Rx.NET/blob/fa1629a1e12a8fc21c95aeff7863425c2485defd/LICENSE for more information. 

using System;
using UniRx.InternalUtil;

namespace UniRx
{
    /// <summary>
    /// Base class for virtual time schedulers.
    /// </summary>
    public abstract class VirtualTimeSchedulerBase : IScheduler
    {
        /// <summary>
        /// Creates a new virtual time scheduler with the default value of TAbsolute as the initial clock value.
        /// </summary>
        protected VirtualTimeSchedulerBase()
        {
        }

        /// <summary>
        /// Creates a new virtual time scheduler with the specified initial clock value and absolute time comparer.
        /// </summary>
        /// <param name="initialClock">Initial value for the clock.</param>
        protected VirtualTimeSchedulerBase(long initialClock)
        {
            Clock = initialClock;
        }

        /// <summary>
        /// Gets whether the scheduler is enabled to run work.
        /// </summary>
        public bool IsEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Schedules an action to be executed at dueTime.
        /// </summary>
        /// <param name="dueTime">Absolute time at which to execute the action.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        public abstract IDisposable ScheduleAbsolute(long dueTime, Action action);

        /// <summary>
        /// Schedules an action to be executed at dueTime.
        /// </summary>
        /// <param name="dueTime">Relative time after which to execute the action.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        public IDisposable Schedule(long dueTime, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var runAt = Clock + dueTime;

            return ScheduleAbsolute(runAt, action);
        }

        public DateTimeOffset Now
        {
            get
            {
                return DateTimeOffset.MinValue + TimeSpan.FromTicks(Clock);
            }
        }

        /// <summary>
        /// Schedules an action to be executed.
        /// </summary>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public IDisposable Schedule(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return ScheduleAbsolute(Clock, action);
        }

        /// <summary>
        /// Schedules an action to be executed after dueTime.
        /// </summary>
        /// <param name="dueTime">Relative time after which to execute the action.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public IDisposable Schedule(TimeSpan dueTime, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return Schedule(dueTime.Ticks, action);
        }

        /// <summary>
        /// Starts the virtual time scheduler.
        /// </summary>
        public void Start()
        {
            if (!IsEnabled)
            {
                IsEnabled = true;
                do
                {
                    var next = GetNext();
                    if (next != null)
                    {
                        if (next.DueTime.Ticks > Clock)
                            Clock = next.DueTime.Ticks;
                        next.Invoke();
                    }
                    else
                        IsEnabled = false;
                } while (IsEnabled);
            }
        }

        /// <summary>
        /// Stops the virtual time scheduler.
        /// </summary>
        public void Stop()
        {
            IsEnabled = false;
        }

        /// <summary>
        /// Advances the scheduler's clock to the specified time, running all work till that point.
        /// </summary>
        /// <param name="time">Absolute time to advance the scheduler's clock to.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is in the past.</exception>
        /// <exception cref="InvalidOperationException">The scheduler is already running. VirtualTimeScheduler doesn't support running nested work dispatch loops. To simulate time slippage while running work on the scheduler, use <see cref="Sleep"/>.</exception>
        public void AdvanceTo(long time)
        {
            if (time < Clock)
                throw new ArgumentOutOfRangeException(nameof(time));

            if (time == Clock)
                return;

            if (!IsEnabled)
            {
                IsEnabled = true;
                do
                {
                    var next = GetNext();
                    if (next != null && next.DueTime.Ticks <= time)
                    {
                        if (next.DueTime.Ticks > Clock)
                            Clock = next.DueTime.Ticks;
                        next.Invoke();
                    }
                    else
                        IsEnabled = false;
                } while (IsEnabled);

                Clock = time;
            }
            else
            {
                throw new InvalidOperationException("Can't advance while running");
            }
        }

        /// <summary>
        /// Advances the scheduler's clock by the specified relative time, running all work scheduled for that timespan.
        /// </summary>
        /// <param name="time">Relative time to advance the scheduler's clock by.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is negative.</exception>
        /// <exception cref="InvalidOperationException">The scheduler is already running. VirtualTimeScheduler doesn't support running nested work dispatch loops. To simulate time slippage while running work on the scheduler, use <see cref="Sleep"/>.</exception>
        public void AdvanceBy(long time)
        {
            var dt = Clock + time;

            if (dt < Clock)
                throw new ArgumentOutOfRangeException(nameof(time));

            if (dt == Clock)
                return;

            if (!IsEnabled)
            {
                AdvanceTo(dt);
            }
            else
            {
                throw new InvalidOperationException("Can't advance while running");
            }
        }

        /// <summary>
        /// Advances the scheduler's clock by the specified relative time.
        /// </summary>
        /// <param name="time">Relative time to advance the scheduler's clock by.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is negative.</exception>
        public void Sleep(long time)
        {
            var dt = Clock + time;

            if (dt < Clock)
                throw new ArgumentOutOfRangeException(nameof(time));

            Clock = dt;
        }

        /// <summary>
        /// Gets the scheduler's absolute time clock value.
        /// </summary>
        public long Clock
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the next scheduled item to be executed.
        /// </summary>
        /// <returns>The next scheduled item.</returns>
        internal abstract ScheduledItem GetNext();
    }

    /// <summary>
    /// Base class for virtual time schedulers using a priority queue for scheduled items.
    /// </summary>
    public abstract class VirtualTimeScheduler : VirtualTimeSchedulerBase
    {
        private readonly SchedulerQueue queue = new SchedulerQueue();

        /// <summary>
        /// Creates a new virtual time scheduler with the default value of TAbsolute as the initial clock value.
        /// </summary>
        protected VirtualTimeScheduler()
        {
        }

        /// <summary>
        /// Creates a new virtual time scheduler.
        /// </summary>
        /// <param name="initialClock">Initial value for the clock.</param>
        protected VirtualTimeScheduler(long initialClock) : base(initialClock)
        {
        }

        /// <summary>
        /// Gets the next scheduled item to be executed.
        /// </summary>
        /// <returns>The next scheduled item.</returns>
        internal override ScheduledItem GetNext()
        {
            lock (queue)
            {
                while (queue.Count > 0)
                {
                    var next = queue.Peek();
                    if (next.IsCanceled)
                        queue.Dequeue();
                    else
                        return next;
                }
            }

            return null;
        }

        /// <summary>
        /// Schedules an action to be executed at dueTime.
        /// </summary>
        /// <param name="action">Action to be executed.</param>
        /// <param name="dueTime">Absolute time at which to execute the action.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public override IDisposable ScheduleAbsolute(long dueTime, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var si = default(ScheduledItem);

            si = new ScheduledItem(() =>
            {
                lock (queue)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    queue.Remove(si);
                }

                action();
            }, TimeSpan.FromTicks(dueTime));

            lock (queue)
            {
                queue.Enqueue(si);
            }

            return Disposable.Create(() =>
            {
                lock (queue)
                {
                    queue.Remove(si);
                }
            });
        }
    }
}