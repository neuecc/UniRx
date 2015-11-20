using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UniRx
{
#if UniRxLibrary
    public static partial class SchedulerUnity
    {
#else
    public static partial class Scheduler
    {
        public static void SetDefaultForUnity()
        {
            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = MainThread;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }
#endif
        static IScheduler mainThread;

        /// <summary>
        /// Unity native MainThread Queue Scheduler. Run on mainthread and delayed on coroutine update loop, elapsed time is calculated based on Time.time.
        /// </summary>
        public static IScheduler MainThread
        {
            get
            {
                return mainThread ?? (mainThread = new MainThreadScheduler());
            }
        }

        static IScheduler mainThreadIgnoreTimeScale;

        /// <summary>
        /// Another MainThread scheduler, delay elapsed time is calculated based on Time.realtimeSinceStartup.
        /// </summary>
        public static IScheduler MainThreadIgnoreTimeScale
        {
            get
            {
                return mainThreadIgnoreTimeScale ?? (mainThreadIgnoreTimeScale = new IgnoreTimeScaleMainThreadScheduler());
            }
        }

        class MainThreadScheduler : IScheduler
        {
            public MainThreadScheduler()
            {
                MainThreadDispatcher.Initialize();
            }

            // delay action is run in StartCoroutine
            // Okay to action run synchronous and guaranteed run on MainThread
            IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
            {
                if (!ScenePlaybackDetectorStub.IsPlaying)
                {
                    var startTime = DateTimeOffset.UtcNow;
                    while (true)
                    {
                        yield return null;
                        if (cancellation.IsDisposed) break;

                        var elapsed = DateTimeOffset.UtcNow - startTime;
                        if (elapsed >= dueTime)
                        {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    };
                    yield break;
                }
                
                if (dueTime == TimeSpan.Zero)
                {
                    yield return null; // not immediately, run next frame
                    if (cancellation.IsDisposed) yield break;

                    MainThreadDispatcher.UnsafeSend(action);
                }
                else if (dueTime.TotalMilliseconds % 1000 == 0)
                {
                    yield return new WaitForSeconds((float)dueTime.TotalSeconds);
                    if (cancellation.IsDisposed) yield break;

                    MainThreadDispatcher.UnsafeSend(action);
                }
                else
                {
                    var startTime = Time.time;
                    var dt = (float)dueTime.TotalSeconds;
                    while (true)
                    {
                        yield return null;
                        if (cancellation.IsDisposed) break;

                        var elapsed = Time.time - startTime;
                        if (elapsed >= dt)
                        {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    }
                }
            }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
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
                var time = Scheduler.Normalize(dueTime);

                MainThreadDispatcher.SendStartCoroutine(DelayAction(time, () =>
                {
                    if (!d.IsDisposed)
                    {
                        action();
                    }
                }, d));

                return d;
            }
        }

        class IgnoreTimeScaleMainThreadScheduler : IScheduler
        {
            public IgnoreTimeScaleMainThreadScheduler()
            {
                MainThreadDispatcher.Initialize();
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
            {
                if (!ScenePlaybackDetectorStub.IsPlaying)
                {
                    var startTime = DateTimeOffset.UtcNow;
                    while (true)
                    {
                        yield return null;
                        if (cancellation.IsDisposed) break;

                        var elapsed = DateTimeOffset.UtcNow - startTime;
                        if (elapsed >= dueTime)
                        {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    };
                    yield break;
                }

                if (dueTime == TimeSpan.Zero)
                {
                    yield return null;
                    if (cancellation.IsDisposed) yield break;

                    MainThreadDispatcher.UnsafeSend(action);
                }
                else
                {
                    var startTime = Time.realtimeSinceStartup; // this is difference
                    var dt = (float)dueTime.TotalSeconds;
                    while (true)
                    {
                        yield return null;
                        if (cancellation.IsDisposed) break;

                        var elapsed = Time.realtimeSinceStartup - startTime;
                        if (elapsed >= dt)
                        {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    }
                }
            }

            public DateTimeOffset Now
            {
                get { return Scheduler.Now; }
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
                var time = Scheduler.Normalize(dueTime);

                MainThreadDispatcher.SendStartCoroutine(DelayAction(time, () =>
                {
                    if (!d.IsDisposed)
                    {
                        action();
                    }
                }, d));

                return d;
            }
        }
    }
}