using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IScheduler))]
[assembly: TypeForwardedTo(typeof(Scheduler))]
[assembly: TypeForwardedTo(typeof(CurrentThreadScheduler))]
[assembly: TypeForwardedTo(typeof(ThreadPoolScheduler))]
[assembly: TypeForwardedTo(typeof(ImmediateScheduler))]
