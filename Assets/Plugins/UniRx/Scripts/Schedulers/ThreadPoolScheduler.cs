#if !UNITY_METRO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniRx.InternalUtil;

namespace UniRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

        class ThreadPoolScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
        {
            public ThreadPoolScheduler()
            {
            }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action)
            {
                var d = new BooleanDisposable();

                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    if (!d.IsDisposed)
                    {
                        action();
                    }
                });

                return d;
            }

            public IDisposable Schedule(DateTimeOffset dueTime, Action action)
            {
                return Schedule(dueTime - Now, action);
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action)
            {
                return new Timer(dueTime, action);
            }

            public IDisposable SchedulePeriodic<T>(T state, TimeSpan period, Action<T> action)
            {
                return new PeriodicTimer<T>(state, period, action);
            }

            public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(callBackState =>
                {
                    if (!cancel.IsDisposed)
                    {
                        action((T)callBackState);
                    }
                }, state);
            }

            // timer was borrwed from Rx Official

            sealed class Timer : IDisposable
            {
                static readonly HashSet<System.Threading.Timer> s_timers = new HashSet<System.Threading.Timer>();

                private readonly SingleAssignmentDisposable _disposable;

                private Action _action;
                private System.Threading.Timer _timer;

                private bool _hasAdded;
                private bool _hasRemoved;

                public Timer(TimeSpan dueTime, Action action)
                {
                    _disposable = new SingleAssignmentDisposable();
                    _disposable.Disposable = Disposable.Create(Unroot);

                    _action = action;
                    _timer = new System.Threading.Timer(Tick, null, dueTime, TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite));

                    lock (s_timers)
                    {
                        if (!_hasRemoved)
                        {
                            s_timers.Add(_timer);

                            _hasAdded = true;
                        }
                    }
                }

                private void Tick(object state)
                {
                    try
                    {
                        if (!_disposable.IsDisposed)
                        {
                            _action();
                        }
                    }
                    finally
                    {
                        Unroot();
                    }
                }

                private void Unroot()
                {
                    _action = Stubs.Nop;

                    var timer = default(System.Threading.Timer);

                    lock (s_timers)
                    {
                        if (!_hasRemoved)
                        {
                            timer = _timer;
                            _timer = null;

                            if (_hasAdded && timer != null)
                                s_timers.Remove(timer);

                            _hasRemoved = true;
                        }
                    }

                    if (timer != null)
                        timer.Dispose();
                }

                public void Dispose()
                {
                    _disposable.Dispose();
                }
            }

            static class RunningTimers
            {
                public static readonly HashSet<System.Threading.Timer> Periodic = new HashSet<System.Threading.Timer>();
            }

            sealed class PeriodicTimer<T> : IDisposable
            {
                private T _state;
                private Action<T> _action;
                private System.Threading.Timer _timer;
                private readonly AsyncLock<Tuple<Action<T>, T>> _gate;

                public PeriodicTimer(T state, TimeSpan period, Action<T> action)
                {
                    this._state = state;
                    this._action = action;
                    this._timer = new System.Threading.Timer(Tick, null, period, period);
                    this._gate = new AsyncLock<Tuple<Action<T>, T>>();

                    lock (RunningTimers.Periodic)
                    {
                        RunningTimers.Periodic.Add(_timer);
                    }
                }

                private void Tick(object state)
                {
                    _gate.Wait(Tuple.Create(_action, _state), t =>
                    {
                        t.Item1.Invoke(t.Item2);
                    });
                }

                public void Dispose()
                {
                    var timer = default(System.Threading.Timer);

                    lock (RunningTimers.Periodic)
                    {
                        timer = _timer;
                        _timer = null;

                        if (timer != null)
                            RunningTimers.Periodic.Remove(timer);
                    }

                    if (timer != null)
                    {
                        timer.Dispose();
                        _action = Stubs<T>.Ignore;
                    }
                }
            }
        }
    }
}

#endif