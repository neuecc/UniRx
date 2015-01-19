#if !UNITY_METRO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UniRx
{
    public static partial class Scheduler
    {
        public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

        class ThreadPoolScheduler : IScheduler
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
                    _action = () => { }; // NOP

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
        }
    }
}

#endif