// this code is borrowed from RxOfficial(rx.codeplex.com) and modified

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace UniRx.InternalUtil
{
    /// <summary>
    /// Represents a work item that has been scheduled.
    /// </summary>
    /// <typeparam name="TAbsolute">Absolute time representation type.</typeparam>
    public interface IScheduledItem
    {
        /// <summary>
        /// Gets the absolute time at which the item is due for invocation.
        /// </summary>
        TimeSpan DueTime { get; }

        /// <summary>
        /// Invokes the work item.
        /// </summary>
        void Invoke();
    }

    /// <summary>
    /// Abstract base class for scheduled work items.
    /// </summary>
    /// <typeparam name="TAbsolute">Absolute time representation type.</typeparam>
    public abstract class ScheduledItem : IScheduledItem, IComparable<ScheduledItem>
    {
        private readonly SingleAssignmentDisposable _disposable = new SingleAssignmentDisposable();
        private readonly TimeSpan _dueTime;

        /// <summary>
        /// Creates a new scheduled work item to run at the specified time.
        /// </summary>
        /// <param name="dueTime">Absolute time at which the work item has to be executed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
        protected ScheduledItem(TimeSpan dueTime)
        {
            _dueTime = dueTime;
        }

        /// <summary>
        /// Gets the absolute time at which the item is due for invocation.
        /// </summary>
        public TimeSpan DueTime
        {
            get { return _dueTime; }
        }

        /// <summary>
        /// Invokes the work item.
        /// </summary>
        public void Invoke()
        {
            if (!_disposable.IsDisposed)
                _disposable.Disposable = InvokeCore();
        }

        /// <summary>
        /// Implement this method to perform the work item invocation, returning a disposable object for deep cancellation.
        /// </summary>
        /// <returns>Disposable object used to cancel the work item and/or derived work items.</returns>
        protected abstract IDisposable InvokeCore();

        #region Inequality

        /// <summary>
        /// Compares the work item with another work item based on absolute time values.
        /// </summary>
        /// <param name="other">Work item to compare the current work item to.</param>
        /// <returns>Relative ordering between this and the specified work item.</returns>
        /// <remarks>The inequality operators are overloaded to provide results consistent with the IComparable implementation. Equality operators implement traditional reference equality semantics.</remarks>
        public int CompareTo(ScheduledItem other)
        {
            // MSDN: By definition, any object compares greater than null, and two null references compare equal to each other. 
            if (object.ReferenceEquals(other, null))
                return 1;

            return DueTime.CompareTo(other.DueTime);
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due before a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is earlier than the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator <(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due before or at the same of a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is earlier than or simultaneous with the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator <=(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due after a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is later than the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator >(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due after or at the same time of a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is later than or simultaneous with the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator >=(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two specified ScheduledItem&lt;TAbsolute, TValue&gt; objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if both ScheduledItem&lt;TAbsolute, TValue&gt; are equal; otherwise, false.</returns>
        /// <remarks>This operator does not provide results consistent with the IComparable implementation. Instead, it implements reference equality.</remarks>
        public static bool operator ==(ScheduledItem left, ScheduledItem right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <summary>
        /// Determines whether two specified ScheduledItem&lt;TAbsolute, TValue&gt; objects are inequal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if both ScheduledItem&lt;TAbsolute, TValue&gt; are inequal; otherwise, false.</returns>
        /// <remarks>This operator does not provide results consistent with the IComparable implementation. Instead, it implements reference equality.</remarks>
        public static bool operator !=(ScheduledItem left, ScheduledItem right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether a ScheduledItem&lt;TAbsolute&gt; object is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare to the current ScheduledItem&lt;TAbsolute&gt; object.</param>
        /// <returns>true if the obj parameter is a ScheduledItem&lt;TAbsolute&gt; object and is equal to the current ScheduledItem&lt;TAbsolute&gt; object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Returns the hash code for the current ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Cancels the work item by disposing the resource returned by InvokeCore as soon as possible.
        /// </summary>
        public void Cancel()
        {
            _disposable.Dispose();
        }

        /// <summary>
        /// Gets whether the work item has received a cancellation request.
        /// </summary>
        public bool IsCanceled
        {
            get { return _disposable.IsDisposed; }
        }
    }

    /// <summary>
    /// Represents a scheduled work item based on the materialization of an IScheduler.Schedule method call.
    /// </summary>
    /// <typeparam name="TAbsolute">Absolute time representation type.</typeparam>
    /// <typeparam name="TValue">Type of the state passed to the scheduled action.</typeparam>
    public sealed class ScheduledItemImpl : ScheduledItem
    {
        private readonly IScheduler _scheduler;
        private readonly Action _action;

        /// <summary>
        /// Creates a materialized work item.
        /// </summary>
        /// <param name="scheduler">Recursive scheduler to invoke the scheduled action with.</param>
        /// <param name="action">Scheduled action.</param>
        /// <param name="dueTime">Time at which to run the scheduled action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="scheduler"/> or <paramref name="action"/> or <paramref name="comparer"/> is null.</exception>
        public ScheduledItemImpl(IScheduler scheduler, Action action, TimeSpan dueTime)
            : base(dueTime)
        {
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");
            if (action == null)
                throw new ArgumentNullException("action");

            _scheduler = scheduler;
            _action = action;
        }

        /// <summary>
        /// Invokes the scheduled action with the supplied recursive scheduler and state.
        /// </summary>
        /// <returns>Cancellation resource returned by the scheduled action.</returns>
        protected override IDisposable InvokeCore()
        {
            _action();
            return Disposable.Empty;
        }
    }

    /// <summary>
    /// Efficient scheduler queue that maintains scheduled items sorted by absolute time.
    /// </summary>
    /// <typeparam name="TAbsolute">Absolute time representation type.</typeparam>
    /// <remarks>This type is not thread safe; users should ensure proper synchronization.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "But it *is* a queue!")]
    public class SchedulerQueue
    {
        private readonly PriorityQueue<ScheduledItem> _queue;

        /// <summary>
        /// Creates a new scheduler queue with a default initial capacity.
        /// </summary>
        public SchedulerQueue()
            : this(1024)
        {
        }

        /// <summary>
        /// Creats a new scheduler queue with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity of the scheduler queue.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public SchedulerQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            _queue = new PriorityQueue<ScheduledItem>(capacity);
        }

        /// <summary>
        /// Gets the number of scheduled items in the scheduler queue.
        /// </summary>
        public int Count
        {
            get
            {
                return _queue.Count;
            }
        }

        /// <summary>
        /// Enqueues the specified work item to be scheduled.
        /// </summary>
        /// <param name="scheduledItem">Work item to be scheduled.</param>
        public void Enqueue(ScheduledItem scheduledItem)
        {
            _queue.Enqueue(scheduledItem);
        }

        /// <summary>
        /// Removes the specified work item from the scheduler queue.
        /// </summary>
        /// <param name="scheduledItem">Work item to be removed from the scheduler queue.</param>
        /// <returns>true if the item was found; false otherwise.</returns>
        public bool Remove(ScheduledItem scheduledItem)
        {
            return _queue.Remove(scheduledItem);
        }

        /// <summary>
        /// Dequeues the next work item from the scheduler queue.
        /// </summary>
        /// <returns>Next work item in the scheduler queue (removed).</returns>
        public ScheduledItem Dequeue()
        {
            return _queue.Dequeue();
        }

        /// <summary>
        /// Peeks the next work item in the scheduler queue.
        /// </summary>
        /// <returns>Next work item in the scheduler queue (not removed).</returns>
        public ScheduledItem Peek()
        {
            return _queue.Peek();
        }
    }
}