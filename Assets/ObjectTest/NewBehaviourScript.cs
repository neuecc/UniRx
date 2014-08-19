#pragma warning disable 0414

using UnityEngine;
using UniRx.UI;
using System.Collections;
using UniRx;
using System.Threading;
using System;
using System.Text;
using UniRx.Diagnostics;

#if !(UNITY_METRO || UNITY_WP8)
using Hash = System.Collections.Hashtable;
using HashEntry = System.Collections.DictionaryEntry;
#else
using Hash = System.Collections.Generic.Dictionary<string, string>;
using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
#endif

// test sandbox
public class NewBehaviourScript : ObservableMonoBehaviour
{
    readonly static Logger logger = new Logger("UniRx.Test.NewBehaviour");

    GameObject text;

    [ThreadStatic]
    static object threadstaticobj;

    public override void Awake()
    {
        ObservableLogger.Listener.LogToUnityDebug();

        text = GameObject.Find("myGuiText");
        MainThreadDispatcher.Initialize();
        threadstaticobj = new object();

        ObservableLogger.Listener.ObserveOnMainThread().Subscribe(x =>
        {
            text.guiText.text = x.ToString();
        });

        base.Awake();
    }

    public override void Start()
    {
        // DoubleCLick Sample of
        // The introduction to Reactive Programming you've been missing
        // https://gist.github.com/staltz/868e7e9bc2a7b8c1f754
        //OnMouseDownAsObservable().Buffer(OnMouseDownAsObservable().Throttle(TimeSpan.FromMilliseconds(250)))
        //    .Where(xs =>
        //    {
        //        logger.Debug(xs.Count);
        //        return xs.Count >= 2;
        //    })
        //    .Subscribe(_ => logger.Debug("Double Click Detected"));

        base.Start();
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }

    public void OnGUI()
    {
        var xpos = 0;
        var ypos = 0;

        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "LazyTask"))
        {
            StartCoroutine(Work());
        }

        ypos += 100;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Observe"))
        {
            logger.Debug(DateTime.Now.ToString());
            Observable.Timer(TimeSpan.FromSeconds(3))
                .ObserveOnMainThread() // comment out this line, get_guiText can only be called from the main thread.
                .Subscribe(x => text.guiText.text = DateTime.Now.ToString());
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
}

#pragma warning restore 0414