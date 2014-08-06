#pragma warning disable 0414

using UnityEngine;
using UniRx.UI;
using System.Collections;
using UniRx;
using System.Threading;
using System;
using System.Text;

// test sandbox
public class NewBehaviourScript : ObservableMonoBehaviour
{
    GameObject text;

    [ThreadStatic]
    static object threadstaticobj;

    public override void Awake()
    {
        text = GameObject.Find("myGuiText");
        MainThreadDispatcher.Initialize();
        threadstaticobj = new object();
        base.Awake();
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
            Debug.Log(DateTime.Now.ToString());
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
            Debug.Log("run");
            Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(5000), () =>
            {
                Debug.Log(DateTime.Now);
            });
        }

        xpos += 100;
        ypos = 0;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Scheduler1"))
        {
            Debug.Log("Before Start");
            Scheduler.MainThread.Schedule(() => Debug.Log("immediate"));
            Scheduler.MainThread.Schedule(TimeSpan.Zero, () => Debug.Log("zero span"));
            Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(1), () => Debug.Log("0.1 span"));
            Debug.Log("After Start");
        }

        ypos += 100;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Scheduler2"))
        {
            Debug.Log("M:Before Start");
            Scheduler.MainThread.Schedule(TimeSpan.FromSeconds(5), () => Debug.Log("M:after 5 minutes"));
            Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(5500), () => Debug.Log("M:after 5.5 minutes"));
        }

        ypos += 100;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Realtime"))
        {
            Debug.Log("R:Before Start");
            Scheduler.MainThreadRealTime.Schedule(TimeSpan.FromSeconds(5), () => Debug.Log("R:after 5 minutes"));
            Scheduler.MainThreadRealTime.Schedule(TimeSpan.FromMilliseconds(5500), () => Debug.Log("R:after 5.5 minutes"));
        }

        ypos += 100;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ManagedThreadId"))
        {
            Debug.Log("Current:" + Thread.CurrentThread.ManagedThreadId);
            new Thread(_ => Debug.Log("NewThread:" + Thread.CurrentThread.ManagedThreadId)).Start();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Debug.Log("ThraedPool:" + Thread.CurrentThread.ManagedThreadId);
                this.transform.position = new Vector3(0, 0, 0); // exception
            });
        }

        ypos += 100;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ThreadStatic"))
        {
            Debug.Log(threadstaticobj != null);
            new Thread(_ => Debug.Log(threadstaticobj != null)).Start();
            ThreadPool.QueueUserWorkItem(_ => Debug.Log(threadstaticobj != null));
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

        Debug.Log(t1.Result + ":" + t2.Result);
        Debug.Log(t3.Exception);
    }

    IEnumerator Test()
    {
        Debug.Log("first");
        yield return 1000;
        Debug.Log("second");
    }
}

#pragma warning restore 0414