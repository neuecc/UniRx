#pragma warning disable 0414

using UnityEngine;
using UniRx.UI;
using System.Collections;
using UniRx;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Text;
using UniRx.Diagnostics;
#if !(UNITY_METRO || UNITY_WP8) && (UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6)
    // Fallback for Unity versions below 4.5
    using Hash = System.Collections.Hashtable;
    using HashEntry = System.Collections.DictionaryEntry;    
#else
using Hash = System.Collections.Generic.Dictionary<string, string>;
using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
#endif

namespace UniRx.ObjectTest
{
    // test sandbox
    public class UniRxTestSandbox : ObservableMonoBehaviour
    {
        readonly static Logger logger = new Logger("UniRx.Test.NewBehaviour");

        StringBuilder logtext = new StringBuilder();

        [ThreadStatic]
        static object threadstaticobj;

        public override void Awake()
        {
            Debug.Log("Awake");

            ObservableLogger.Listener.LogToUnityDebug();

            MainThreadDispatcher.Initialize();
            threadstaticobj = new object();

            ObservableLogger.Listener.ObserveOnMainThread().Subscribe(x =>
            {
                logtext.AppendLine(x.Message);
            });

            base.Awake();
        }

        public override void Start()
        {
            Debug.Log("Start");

            // DoubleCLick Sample of
            // The introduction to Reactive Programming you've been missing
            // https://gist.github.com/staltz/868e7e9bc2a7b8c1f754

            //var clickStream = Observable.EveryUpdate()
            //    .Where(_ => Input.GetMouseButtonDown(0));

            //clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
            //    .Where(xs => xs.Count >= 2)
            //    .Subscribe(xs => Debug.Log("DoubleClick Detected! Count:" + xs.Count));

            base.Start();
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }

        IDisposable yieldCancel = null;

        Subscriber subscriber = new Subscriber();


        public void OnGUI()
        {
            var xpos = 0;
            var ypos = 0;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Clear"))
            {
                logtext.Length = 0;
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Now"))
            {
                logger.Debug(DateTime.Now.ToString());
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "TimeScale-1"))
            {
                Time.timeScale -= 1f;
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "TimeScale+1"))
            {
                Time.timeScale += 1f;
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "TimeScale=0"))
            {
                Time.timeScale = 0;
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "TimeScale=100"))
            {
                Time.timeScale = 100;
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Scheduler0"))
            {
                logger.Debug("run");
                Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(5000), () =>
                {
                    logger.Debug(DateTime.Now);
                });
            }

            xpos += 100;
            ypos = 0;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Scheduler1"))
            {
                logger.Debug("Before Start");
                Scheduler.MainThread.Schedule(() => logger.Debug("immediate"));
                Scheduler.MainThread.Schedule(TimeSpan.Zero, () => logger.Debug("zero span"));
                Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(1), () => logger.Debug("0.1 span"));
                logger.Debug("After Start");
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Scheduler2"))
            {
                logger.Debug("M:Before Start");
                Scheduler.MainThread.Schedule(TimeSpan.FromSeconds(5), () => logger.Debug("M:after 5 minutes"));
                Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(5500), () => logger.Debug("M:after 5.5 minutes"));
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Realtime"))
            {
                logger.Debug("R:Before Start");
                Scheduler.MainThreadIgnoreTimeScale.Schedule(TimeSpan.FromSeconds(5), () => logger.Debug("R:after 5 minutes"));
                Scheduler.MainThreadIgnoreTimeScale.Schedule(TimeSpan.FromMilliseconds(5500), () => logger.Debug("R:after 5.5 minutes"));
            }

#if !UNITY_METRO

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ManagedThreadId"))
            {
                logger.Debug("Current:" + Thread.CurrentThread.ManagedThreadId);
                new Thread(_ => logger.Debug("NewThread:" + Thread.CurrentThread.ManagedThreadId)).Start();
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    logger.Debug("ThraedPool:" + Thread.CurrentThread.ManagedThreadId);
                    this.transform.position = new Vector3(0, 0, 0); // exception
                });
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ThreadStatic"))
            {
                logger.Debug(threadstaticobj != null);
                new Thread(_ => logger.Debug(threadstaticobj != null)).Start();
                ThreadPool.QueueUserWorkItem(_ => logger.Debug(threadstaticobj != null));
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Log"))
            {
                logger.Debug("test", this);
                ThreadPool.QueueUserWorkItem(_ => logger.Debug("test2", this));
            }

#endif

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "POST"))
            {
                var form = new WWWForm();
                form.AddField("test", "abcdefg");
                ObservableWWW.PostWWW("http://localhost:53395/Handler1.ashx", form, new Hash
            {
                {"aaaa", "bbb"},
                {"User-Agent", "HugaHuga"}
            })
                .Subscribe(x => logger.Debug(x.text));
            }

            xpos += 100;
            ypos = 0;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Yield"))
            {
                yieldCancel = Observable.FromCoroutineValue<string>(StringYield, false)
                    .Subscribe(x => logger.Debug(x), ex => logger.Debug("E-x:" + ex));
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "YieldCancel"))
            {
                yieldCancel.Dispose();
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ThreadPool"))
            {
                Observable.Timer(TimeSpan.FromMilliseconds(400), Scheduler.ThreadPool)
                    .ObserveOnMainThread()
                    .Subscribe(x => logger.Debug(x));
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Subscribe"))
            {
                subscriber.InitSubscriptions();
                logger.Debug("Subscribe++ : " + subscriber.SubscriptionCount);
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Push"))
            {
                Publisher.foo();
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Unsubscriber"))
            {
                subscriber.RemoveSubscriptions();
                logger.Debug("UnsubscribeAll : " + subscriber.SubscriptionCount);
            }

            ypos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "DistinctUntilChanged"))
            {
                new[] { "hoge", null, null, "huga", "huga", "hoge" }
                    .ToObservable()
                    .DistinctUntilChanged()
                    .Subscribe(x => logger.Debug(x));
            }

            // Time

            var sb = new StringBuilder();
            sb.AppendLine("CaptureFramerate:" + Time.captureFramerate);
            sb.AppendLine("deltaTime:" + Time.deltaTime);
            sb.AppendLine("fixedDeltaTime:" + Time.fixedDeltaTime);
            sb.AppendLine("fixedTime:" + Time.fixedTime);
            sb.AppendLine("frameCount:" + Time.frameCount);
            sb.AppendLine("maximumDeltaTime:" + Time.maximumDeltaTime);
            sb.AppendLine("realtimeSinceStartup:" + Time.realtimeSinceStartup);
            sb.AppendLine("renderedFrameCount:" + Time.renderedFrameCount);
            sb.AppendLine("smoothDeltaTime:" + Time.smoothDeltaTime);
            sb.AppendLine("time:" + Time.time);
            sb.AppendLine("timeScale:" + Time.timeScale);
            sb.AppendLine("timeSinceLevelLoad:" + Time.timeSinceLevelLoad);
            sb.AppendLine("unscaledDeltaTime:" + Time.unscaledDeltaTime);
            sb.AppendLine("unscaledTime:" + Time.unscaledTime);

            GUI.Box(new Rect(Screen.width - 300, Screen.height - 300, 300, 300), "Time");
            GUI.Label(new Rect(Screen.width - 290, Screen.height - 290, 290, 290), sb.ToString());

            // Log
            GUI.Box(new Rect(Screen.width - 300, 0, 300, 300), "Log");
            GUI.Label(new Rect(Screen.width - 290, 10, 290, 290), logtext.ToString());
        }

        IEnumerator StringYield()
        {
            try
            {
                yield return "aaa";
                yield return "bbb";
                yield return new WaitForSeconds(5);
                yield return "ccc";
                yield return null;
                throw new Exception("ex!!!");
            }
            finally
            {
                logger.Debug("finally!");
            }
        }

        IEnumerator Work()
        {
            var t1 = Observable.Interval(TimeSpan.FromSeconds(1)).Take(4).ToLazyTask();
            var t2 = Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => x * x).Take(4).ToLazyTask();
            var t3 = Observable.Throw<Unit>(new Exception()).ToLazyTask();

            yield return LazyTask.WhenAll(t1, t2, t3);

            logger.Debug(t1.Result + ":" + t2.Result);
            logger.Debug(t3.Exception);
        }

        IEnumerator Test()
        {
            logger.Debug("first");
            yield return 1000;
            logger.Debug("second");
        }

        // Question from UnityForum  #45

        public static class Publisher
        {
            private static readonly object _Lock = new object();
            private static Subject<bool> item = new UniRx.Subject<bool>();

            public static IObservable<bool> Item
            {
                get
                {
                    return item; // no needs lock
                }
            }

            public static void foo()
            {
                item.OnNext(true);
            }
        }

        public class Subscriber
        {
            private CompositeDisposable m_Subscriptions = new CompositeDisposable();

            public int SubscriptionCount { get { return m_Subscriptions.Count; } }

            public void InitSubscriptions()
            {
                m_Subscriptions.Add(Publisher.Item.Subscribe(UniRx.Observer.Create<bool>(result => this.HandleItem(result), ex => this.HandleError(ex), () => { })));
            }

            void HandleItem(bool args)
            {
                logger.Debug("Received Item: " + args);
            }

            void HandleError(Exception ex)
            {
                logger.Debug("Exception: " + ex.Message);
            }

            public void RemoveSubscriptions()
            {
                m_Subscriptions.Clear();
            }
        }
    }
}

#pragma warning restore 0414