#if !UNITY_METRO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
            {
                var d = new SingleAssignmentDisposable();

                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    if (!d.IsDisposed)
                    {
                        d.Disposable = action(this, state);
                    }
                });

                return d;
            }

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                return Schedule(state, dueTime - Now, action);
            }

            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                return new Timer<TState>(this, state, dueTime, action);
            }

            // timer was borrwed from Rx Official

            sealed class Timer<TState> : IDisposable
            {
                static readonly HashSet<System.Threading.Timer> s_timers = new HashSet<System.Threading.Timer>();

                private readonly MultipleAssignmentDisposable _disposable;

                private readonly IScheduler _parent;
                private readonly TState _state;

                private Func<IScheduler, TState, IDisposable> _action;
                private System.Threading.Timer _timer;

                private bool _hasAdded;
                private bool _hasRemoved;

                public Timer(IScheduler parent, TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
                {
                    _disposable = new MultipleAssignmentDisposable();
                    _disposable.Disposable = Disposable.Create(Unroot);

                    _parent = parent;
                    _state = state;

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
                        _disposable.Disposable = _action(_parent, _state);
                    }
                    finally
                    {
                        Unroot();
                    }
                }

                private void Unroot()
                {
                    _action = Nop;

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

                private IDisposable Nop(IScheduler scheduler, TState state)
                {
                    return Disposable.Empty;
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