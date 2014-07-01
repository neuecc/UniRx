using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UniRx
{
    public static partial class Scheduler
    {
        static IScheduler mainThread;

        public static IScheduler MainThread
        {
            get
            {
                return mainThread ?? (mainThread = new MainThreadScheduler());
            }
        }

        class MainThreadScheduler : IScheduler
        {
            public MainThreadScheduler()
            {
                MainThreadDispatcher.Initialize();
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action)
            {
                if (dueTime == TimeSpan.Zero)
                {
                    MainThreadDispatcher.Post(action);
                }
                else if (dueTime.TotalMilliseconds % 1000 == 0)
                {
                    yield return new WaitForSeconds((float)dueTime.TotalSeconds);
                    MainThreadDispatcher.Post(action);
                }
                else
                {
                    var startTime = DateTime.Now;
                    while (true)
                    {
                        yield return null;
                        var now = DateTime.Now;
                        if ((now - startTime) >= dueTime)
                        {
                            MainThreadDispatcher.Post(action);
                            break;
                        }
                    }
                }
            }

            public DateTimeOffset Now
            {
                get { return DateTimeOffset.Now; }
            }

            public IDisposable Schedule(Action action)
            {
                var d = new BooleanDisposable();
                MainThreadDispatcher.Post(() =>
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
                var d = new BooleanDisposable();
                var time = Normalize(dueTime);

                MainThreadDispatcher.StartCoroutine(DelayAction(time, () =>
                {
                    if (!d.IsDisposed)
                    {
                        action();
                    }
                }));

                return d;
            }
        }
    }
}