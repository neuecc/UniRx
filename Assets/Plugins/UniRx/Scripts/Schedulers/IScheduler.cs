using System;

namespace UniRx
{
    public interface IScheduler
    {
        DateTimeOffset Now { get; }

        // simple path
        IDisposable Schedule(Action action);
        IDisposable Schedule(TimeSpan dueTime, Action action);

        // memory optimize path
        //IDisposable Schedule<TState>(TState state, Action<TState> action);
        //IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Action action);
    }

    public interface ISchedulerPeriodic
    {
        IDisposable SchedulePeriodic<T>(T state, TimeSpan period, Action<T> action);
    }

    // currently does not use this.
    // public interface ISchedulerLongRunning
    // {
    //     IDisposable ScheduleLongRunning(Action<ICancelable> action);
    // }

    public interface ISchedulerQueueing
    {
        void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action);
    }
}