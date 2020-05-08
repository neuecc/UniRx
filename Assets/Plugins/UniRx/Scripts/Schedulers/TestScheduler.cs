// Adapted from https://github.com/Reactive-Extensions/Rx.NET/blob/fa1629a1e12a8fc21c95aeff7863425c2485defd/Rx.NET/Source/src/Microsoft.Reactive.Testing/TestScheduler.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file at https://github.com/Reactive-Extensions/Rx.NET/blob/fa1629a1e12a8fc21c95aeff7863425c2485defd/LICENSE for more information. 

using System;

namespace UniRx
{
    /// <summary>
    /// Virtual time scheduler used for testing applications and libraries built using Reactive Extensions.
    /// </summary>
    public class TestScheduler : VirtualTimeScheduler
    {
        /// <summary>
        /// Schedules an action to be executed at the specified virtual time.
        /// </summary>
        /// <param name="action">Action to be executed.</param>
        /// <param name="dueTime">Absolute virtual time at which to execute the action.</param>
        /// <returns>Disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public override IDisposable ScheduleAbsolute(long dueTime, Action action)
        {
            if (dueTime < Clock)
                dueTime = Clock;

            return base.ScheduleAbsolute(dueTime, action);
        }
    }
}
