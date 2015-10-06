using System;

#if SystemReactive
namespace System.Reactive.Concurrency
#else
namespace UniRx
#endif
{
    public interface IScheduler
    {
        DateTimeOffset Now { get; }

        // Interface is changed from official Rx for avoid iOS AOT problem (state is dangerous).

        IDisposable Schedule(Action action);

        IDisposable Schedule(TimeSpan dueTime, Action action);
    }
}
