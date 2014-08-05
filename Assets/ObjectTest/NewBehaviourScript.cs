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

    public override void Awake()
    {
        text = GameObject.Find("myGuiText");
        MainThreadDispatcher.Initialize();
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
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Scheduler"))
        {
            Debug.Log("run");
            Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(5000), () =>
            {
                Debug.Log(DateTime.Now);
            });
        }

        xpos += 100;
        ypos = 0;
        if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Coroutine"))
        {
            Debug.Log("Before Start");
            //StartCoroutine(t);
            Scheduler.MainThread.Schedule(() => Debug.Log("immediate"));
            Scheduler.MainThread.Schedule(TimeSpan.Zero, () => Debug.Log("zero span"));
            Scheduler.MainThread.Schedule(TimeSpan.FromMilliseconds(1), () => Debug.Log("0.1 span"));
            Debug.Log("After Start");
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
