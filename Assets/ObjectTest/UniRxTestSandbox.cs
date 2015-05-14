#pragma warning disable 0414

using UnityEngine;
using System.Collections;
using UniRx;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Text;
using UniRx.Triggers;
using UniRx.Diagnostics;
using System.Net;
using System.IO;
#if !(UNITY_METRO || UNITY_WP8) && (UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6)
    // Fallback for Unity versions below 4.5
    using Hash = System.Collections.Hashtable;
    using HashEntry = System.Collections.DictionaryEntry;    
#else
using Hash = System.Collections.Generic.Dictionary<string, string>;
using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
using UniRx.InternalUtil;
#endif

namespace UniRx.ObjectTest
{
    public enum Mikan
    {
        Ringo = 30,
        Tako = 40
    }

    public class LogCallback
    {
        public string Condition;
        public string StackTrace;
        public UnityEngine.LogType LogType;
    }

    static class LogHelper
    {
        // If static register callback, use Subject for event branching.

#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6)
            static Subject<LogCallback> subject;

            public static IObservable<LogCallback> LogCallbackAsObservable()
            {
                if (subject == null)
                {
                    subject = new Subject<LogCallback>();

                    // Publish to Subject in callback


                    UnityEngine.Application.RegisterLogCallback((condition, stackTrace, type) =>
                    {
                        subject.OnNext(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type });
                    });
                }

                return subject.AsObservable();
            }

#else
        // If standard evetns, you can use Observable.FromEvent.

        public static IObservable<LogCallback> LogCallbackAsObservable()
        {
            return Observable.FromEvent<Application.LogCallback, LogCallback>(
                h => (condition, stackTrace, type) => h(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type }),
                h => Application.logMessageReceived += h, h => Application.logMessageReceived -= h);
        }
#endif
    }

    [Serializable]
    public struct MySuperStruct
    {
        public int A;
        public int B;
        public int C;
        public int D;
        public int E;
        public int F;
        public int G;
        public int X;
        public int Y;
        public int Z;

        public override string ToString()
        {
            return A + ":" + B + ":" + C + ":" + D + ":" + E + ":" + F + ":" + G + ":" + X + ":" + Y + ":" + Z;
        }
    }

    public enum Fruit
    {
        Apple, Grape
    }

    [Serializable]
    public class FruitReactiveProperty : ReactiveProperty<Fruit>
    {
        public FruitReactiveProperty()
        {

        }

        public FruitReactiveProperty(Fruit initialValue)
            : base(initialValue)
        {

        }
    }


    [Serializable]
    public class MikanReactiveProperty : ReactiveProperty<Mikan>
    {
        public MikanReactiveProperty()
        {

        }

        public MikanReactiveProperty(Mikan mikan)
            : base(mikan)
        {

        }
    }

    [Serializable]
    public class MyContainerClass
    {
        public int X;
        public int Y;
    }


    [Serializable]
    public class MySuperStructReactiveProperty : ReactiveProperty<MySuperStruct>
    {
        public MySuperStructReactiveProperty()
        {

        }
    }


    [Serializable]
    public class MyContainerReactiveProperty : ReactiveProperty<MyContainerClass>
    {
        public MyContainerReactiveProperty()
        {

        }
    }

    [Serializable]
    public class UShortReactiveProeprty : ReactiveProperty<ushort>
    {

    }


#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(UShortReactiveProeprty))]
    [UnityEditor.CustomPropertyDrawer(typeof(MikanReactiveProperty))]
    [UnityEditor.CustomPropertyDrawer(typeof(MySuperStructReactiveProperty))]
    public class ExtendInspectorDisplayDrawer : InspectorDisplayDrawer
    {
    }

#endif

    // test sandbox
    [Serializable]
    public class UniRxTestSandbox : MonoBehaviour
    {
        readonly static Logger logger = new Logger("UniRx.Test.NewBehaviour");

        // public UnityEvent<int> SimpleEvent;

        public Vector2ReactiveProperty V2R;

        StringBuilder logtext = new StringBuilder();

        GameObject cube;
        Clicker clicker;

        //[ThreadStatic]
        static object threadstaticobj;

        public DateTime DateTimeSonomono;


        public IntReactiveProperty Intxxx;
        public LongReactiveProperty LongxXXX;
        public BoolReactiveProperty Booxxx;
        public FloatReactiveProperty FloAAX;
        public DoubleReactiveProperty DuAAX;
        public MikanReactiveProperty MikanRP;
        public StringReactiveProperty Strrrrr;

        public UShortReactiveProeprty USHHHH;

        //[InspectorDisplay]
        //public MyContainerReactiveProperty MCCCC;

        //public Vector4 V4;
        //public Rect REEEECT;
        //public AnimationCurve ACCCCCCC;
        //public Bounds BOO;
        //public Quaternion ZOOOM;

        public Vector2ReactiveProperty AAA;
        public Vector3ReactiveProperty BBB;
        public Vector4ReactiveProperty CCC;
        public ColorReactiveProperty DDD;
        public RectReactiveProperty EEE;

        // public Slider MySlider;

        public MySuperStructReactiveProperty SUPER_Rx;

        public AnimationCurveReactiveProperty FFF;
        public BoundsReactiveProperty GGG;
        public QuaternionReactiveProperty HHH;

        //public Matrix4x4 MXX;

        public void Awake()
        {
            Debug.Log("Awake");
            LogHelper.LogCallbackAsObservable()
                .ObserveOnMainThread()
                .Subscribe(x => logtext.AppendLine(x.ToString()));


            ObservableLogger.Listener.LogToUnityDebug();

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clicker = cube.AddComponent<Clicker>();



            MainThreadDispatcher.Initialize();
            threadstaticobj = new object();


            ObservableLogger.Listener.ObserveOnMainThread().Subscribe(x =>
            {
                logtext.AppendLine(x.Message);
            });


            // Intxxx.Subscribe(x => Debug.Log(x));
            LongxXXX.Subscribe(x => Debug.Log(x));
            DuAAX.Subscribe(x => Debug.Log(x));
            FloAAX.Subscribe(x => Debug.Log(x));
            Booxxx.Subscribe(x => Debug.Log(x));
            MikanRP.Subscribe(x => Debug.Log(x));
            Strrrrr.Subscribe(x => Debug.Log(x));

            USHHHH.Subscribe(x => Debug.Log(x));

            AAA.Subscribe(x => Debug.Log(x));
            BBB.Subscribe(x => Debug.Log(x));
            CCC.Subscribe(x => Debug.Log(x));
            DDD.Subscribe(x => Debug.Log(x));
            EEE.Subscribe(x => Debug.Log(x));
            FFF.Subscribe(x => Debug.Log(x));
            GGG.Subscribe(x => Debug.Log(x));
            HHH.Subscribe(x => Debug.Log(x));

            SUPER_Rx.Subscribe(x => Debug.Log(x.ToString()));
        }

        public void Start()
        {
            Debug.Log("Start");

            // DoubleCLick Sample of
            // The introduction to Reactive Programming you've been missing
            // https://gist.github.com/staltz/868e7e9bc2a7b8c1f754

            /*
            var clickStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0));

            clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
                .Where(xs => xs.Count >= 2)
                .Subscribe(xs => Debug.Log("DoubleClick Detected! Count:" + xs.Count));
             */

            MainThreadDispatcher.Initialize();

        }

        GameObject primitive = null;

        public void Update()
        {

        }

        public void OnDestroy()
        {
            Debug.Log("Destroy");
        }


        IDisposable yieldCancel = null;

        // Subscriber subscriber = new Subscriber();

        CompositeDisposable disposables = new CompositeDisposable();


        Tadanoiremono iremono = null;

        IEnumerator Hoge()
        {
            while (true)
            {
                // logtext.AppendLine(Time.frameCount.ToString());
                yield return null;
            }
        }

        Subject<Unit> throttleSubject = new Subject<Unit>();
        Func<bool> isNull = null;
        ReactiveProperty<int> fromNeverRxProp = null;

        public Func<bool> IsNull<T>(T source)
        {
            return () => source == null;
        }

        public bool IsNullNano<T>(T source)
        {
            return source == null;
        }

        public void OnGUI()
        {
            //var xpos = 0;
            //var ypos = 0;

            if (GUILayout.Button("Clear"))
            {
                logtext.Length = 0;
                disposables.Clear();
            }

            if (GUILayout.Button("RxProp1"))
            {
                Intxxx.Subscribe(x => Debug.Log(x));
            }

            if (GUILayout.Button("RxProp2"))
            {
                fromNeverRxProp = Observable.Never<int>().ToReactiveProperty();
                fromNeverRxProp.Subscribe(x => Debug.Log(x));
            }

            if (GUILayout.Button("RxProp2Push"))
            {
                fromNeverRxProp.Value = 20;
            }

            if (GUILayout.Button("DelayFrameEmpty"))
            {
                logtext.AppendLine("StartFrame:" + Time.frameCount);
                Observable.Empty<int>()
                    .DelayFrame(3)
                    .Subscribe(x => logtext.AppendLine(x.ToString() + ":" + Time.frameCount), () => logtext.AppendLine("completed" + ":" + Time.frameCount));
            }

            if (GUILayout.Button("NextFrame"))
            {
                logtext.AppendLine("StartFrame:" + Time.frameCount);
                Observable.NextFrame()
                    .Subscribe(x => logtext.AppendLine(x.ToString() + ":" + Time.frameCount), () => logtext.AppendLine("completed" + Time.frameCount))
                    .AddTo(disposables);
            }

            if (GUILayout.Button("IntervalFrame"))
            {
                logtext.AppendLine("StartFrame:" + Time.frameCount);
                Observable.IntervalFrame(3)
                    .Subscribe(x => logtext.AppendLine(x.ToString() + ":" + Time.frameCount), () => logtext.AppendLine("completed" + Time.frameCount))
                    .AddTo(disposables);
            }

            if (GUILayout.Button("TimerFrame1"))
            {
                logtext.AppendLine("StartFrame:" + Time.frameCount);
                Observable.TimerFrame(3)
                    .Subscribe(x => logtext.AppendLine(x.ToString() + ":" + Time.frameCount), () => logtext.AppendLine("completed" + Time.frameCount))
                    .AddTo(disposables);
            }

            if (GUILayout.Button("TimerFrame2"))
            {
                logtext.AppendLine("StartFrame:" + Time.frameCount);
                Observable.TimerFrame(5, 3)
                    .Subscribe(x => logtext.AppendLine(x.ToString() + ":" + Time.frameCount), () => logtext.AppendLine("completed" + Time.frameCount))
                    .AddTo(disposables);
            }

            if (GUILayout.Button("TimeScaleZero"))
            {
                logtext.AppendLine("StartFrame:" + Time.frameCount);
                Time.timeScale = 0f;
                Scheduler.MainThreadIgnoreTimeScale.Schedule(TimeSpan.FromSeconds(3), () =>
                {
                    logtext.AppendLine(Time.frameCount.ToString());
                });
            }

            if (GUILayout.Button("SampleFrame"))
            {
                logtext.AppendLine("SampleFrame:" + Time.frameCount);
                Observable.IntervalFrame(10)
                    .SampleFrame(25)
                    .Take(6)
                    .Subscribe(x =>
                    {
                        logtext.AppendLine("Sample:" + Time.frameCount.ToString());
                    }, () =>
                    {
                        logtext.AppendLine("Complete:" + Time.frameCount.ToString());
                    })
                    .AddTo(disposables);
            }

            if (GUILayout.Button("ThrottleClick"))
            {
                logtext.AppendLine("ClickFrame:" + Time.frameCount);
                throttleSubject.OnNext(Unit.Default);
            }

            if (GUILayout.Button("ThrottleFrame"))
            {
                logtext.AppendLine("ThrottleFrame:" + Time.frameCount);
                throttleSubject
                    .ThrottleFrame(60)
                    .Subscribe(x =>
                    {
                        logtext.AppendLine("Throttle:" + Time.frameCount.ToString());
                    }, () =>
                    {
                        logtext.AppendLine("Complete:" + Time.frameCount.ToString());
                    })
                    .AddTo(disposables);
            }

            if (GUILayout.Button("ThrottleFirstFrame"))
            {
                logtext.AppendLine("ThrottleFirstFrame:" + Time.frameCount);
                throttleSubject
                    .ThrottleFirstFrame(60)
                    .Subscribe(x =>
                    {
                        logtext.AppendLine("ThrottleFirst:" + Time.frameCount.ToString());
                    }, () =>
                    {
                        logtext.AppendLine("Complete:" + Time.frameCount.ToString());
                    })
                    .AddTo(disposables);
            }

            if (GUILayout.Button("TimeoutFrame"))
            {
                logtext.AppendLine("TimeoutFrame:" + Time.frameCount);
                throttleSubject
                    .TimeoutFrame(60)
                    .Subscribe(x =>
                    {
                        logtext.AppendLine("Throttle:" + Time.frameCount.ToString());
                    }, ex =>
                        {
                            logtext.AppendLine("Timeout:" + ex.ToString());
                        }, () =>
                    {
                        logtext.AppendLine("Complete:" + Time.frameCount.ToString());
                    })
                    .AddTo(disposables);
            }

            if (GUILayout.Button("ReactiveProperty"))
            {
                var enemy = new Enemy(1000);
                enemy.CurrentHp.Subscribe(x => logtext.AppendLine(x.ToString())).AddTo(disposables);
                enemy.CurrentHp.Value -= 900;

                var person = new Person("hoge", "huga");
                person.FullName.Subscribe(x => logtext.AppendLine(x)).AddTo(disposables);

                person.GivenName.Value = "aiueo";
                person.FamilyName.Value = "kakikukeko";
            }

            if (GUILayout.Button("RxProp2"))
            {
                var p = new ReactiveProperty<Enemy>();
                p.Skip(1).Subscribe(x => Debug.Log("set" + x.CurrentHp.Value));

                p.Value = new Enemy(1000);
                p.Value = new Enemy(43000);
                p.Value = new Enemy(43000);
            }

            if (GUILayout.Button("Log"))
            {
                logger.DebugFormat("debug{0}format{1}", "-", "!");
                logger.ErrorFormat("error{0}format{1}", "-", "!");
                logger.WarningFormat("warning{0}format{1}", "-", "!");
                logger.LogFormat("log{0}format{1}", "-", "!");
            }

            if (GUILayout.Button("Cancel"))
            {
                var dispose = Scheduler.MainThread.Schedule(TimeSpan.FromSeconds(3), () =>
                {
                    Debug.Log("MainThreadSchedule");
                });
                dispose.Dispose();
            }

            if (GUILayout.Button("Observe(UnityObject)"))
            {
                Debug.Log("before");
                clicker.transform.ObserveEveryValueChanged(x => x.position.x)
                    .Subscribe(x => Debug.Log(x), Debug.LogException, () => Debug.Log("comp"));
                Debug.Log("after");
            }

            if (GUILayout.Button("DestroyCube"))
            {
                GameObject.Destroy(cube);
            }

            if (GUILayout.Button("NextScene"))
            {
                Application.LoadLevel("NextSandbox");
            }

            if (GUILayout.Button("Create POCO"))
            {
                iremono = new Tadanoiremono();
            }

            if (GUILayout.Button("Observe POCO"))
            {
                iremono.ObserveEveryValueChanged(x => x.Hoge)
                    .Subscribe(x => Debug.Log(x), Debug.LogException, () => Debug.Log("comp"))
                    .AddTo(disposables);
            }

            if (GUILayout.Button("Add POCO"))
            {
                iremono.Hoge += 100;
            }

            if (GUILayout.Button("POCO Null"))
            {
                iremono = null;
            }

            if (GUILayout.Button("GC"))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            if (GUILayout.Button("Trigger"))
            {
                primitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                var trigger = primitive.AddComponent<ObservableUpdateTrigger>();
                trigger.UpdateAsObservable()
                    .Subscribe(x => Debug.Log(x), () => Debug.Log("Comp!!!"));
            }

            if (GUILayout.Button("Destroy Primitive"))
            {
                if (primitive != null) GameObject.Destroy(primitive);
            }

            if (GUILayout.Button("Cube Update"))
            {
                clicker.UpdateAsObservable()
                    .Finally(() => Debug.Log("f"))
                    .Subscribe(x => Debug.Log(x));
            }

            if (GUILayout.Button("TimeoutCheck"))
            {
                Application.targetFrameRate = 60;
                Observable.Concat(
                        Observable.Timer(TimeSpan.FromSeconds(3)).Select(_ => "a"),
                        Observable.Timer(TimeSpan.FromSeconds(6)).Select(_ => "b"))
                    .TimeoutFrame(60 * 8)
                    .Subscribe(x => Debug.Log("timer complete:" + x), ex => Debug.Log(ex.ToString()), () => Debug.Log("comp"));

            }

            if (GUILayout.Button("FromAsyncPattern"))
            {
                var req = WebRequest.Create("http://unity3d.com/");
                req.GetResponseAsObservable()
                    .ObserveOnMainThread()
                    .Subscribe(x =>
                    {
                        using (var stream = x.GetResponseStream())
                        using (var sr = new StreamReader(stream))
                        {
                            var txt = sr.ReadToEnd();
                            Debug.Log(txt);
                        }
                    });
            }

            GUILayout.BeginArea(new Rect(200, 0, 100, 200));
            {
                if (GUILayout.Button("Simple FromEvent"))
                {
                    try
                    {
                        // MySlider.OnValueChangedAsObservable().Subscribe(x => logger.Debug(x), ex => logger.Exception(ex));

                    }
                    //Observable.FromEvent<UnityEvent>(h => h.Invoke, 
                    catch (Exception ex)
                    {
                        logger.Exception(ex);
                    }
                }
                GUILayout.EndArea();


                //if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Clear"))
                //{
                //    logtext.Length = 0;
                //    disposables.Clear();
                //}
                //ypos += 100;
                //if (GUI.Button(new Rect(xpos, ypos, 100, 100), "CurrentThreadScheduler"))
                //{
                //    try
                //    {
                //        Scheduler.CurrentThread.Schedule(() =>
                //        {
                //            try
                //            {
                //                logtext.AppendLine("test threadscheduler");
                //            }
                //            catch (Exception ex)
                //            {
                //                logtext.AppendLine("innner ex" + ex.ToString());
                //            }
                //        });
                //    }
                //    catch (Exception ex)
                //    {
                //        logtext.AppendLine("outer ex" + ex.ToString());
                //    }
                //}
                //ypos += 100;
                //if (GUI.Button(new Rect(xpos, ypos, 100, 100), "EveryUpdate"))
                //{
                //    Observable.EveryUpdate()
                //        .Subscribe(x => logtext.AppendLine(x.ToString()), ex => logtext.AppendLine("ex:" + ex.ToString()))
                //        .AddTo(disposables);
                //}
                //ypos += 100;
                //if (GUI.Button(new Rect(xpos, ypos, 100, 100), "FromCoroutinePure"))
                //{
                //    Observable.Create<Unit>(observer =>
                //    {
                //        var cancel = new BooleanDisposable();

                //        MainThreadDispatcher.StartCoroutine(Hoge(observer));

                //        return cancel;
                //    })
                //    .Subscribe(x => logtext.AppendLine(x.ToString()), ex => logtext.AppendLine("ex:" + ex.ToString()));
                //}
                //ypos += 100;
                //if (GUI.Button(new Rect(xpos, ypos, 100, 100), "FromCoroutine"))
                //{
                //    Observable.FromCoroutine<Unit>(Hoge)
                //    .Subscribe(x => logtext.AppendLine(x.ToString()), ex => logtext.AppendLine("ex:" + ex.ToString()));
                //}


                /*

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
                 * */

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

                //GUI.Box(new Rect(Screen.width - 300, Screen.height - 300, 300, 300), "Time");
                //GUI.Label(new Rect(Screen.width - 290, Screen.height - 290, 290, 290), sb.ToString());

                // logtext only
                GUI.Box(new Rect(Screen.width - 300, Screen.height - 300, 300, 300), "logtext");
                GUI.Label(new Rect(Screen.width - 290, Screen.height - 290, 290, 290), logtext.ToString());

                // Log
                //GUI.Box(new Rect(Screen.width - 300, 0, 300, 300), "Log");
                //GUI.Label(new Rect(Screen.width - 290, 10, 290, 290), logtext.ToString());
            }
            /*
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
             * */
        }



        public class Enemy
        {
            public ReactiveProperty<long> CurrentHp { get; private set; }

            public ReactiveProperty<bool> IsDead { get; private set; }

            public Enemy(int initialHp)
            {
                CurrentHp = new ReactiveProperty<long>(initialHp);
                IsDead = CurrentHp.Select(x => x <= 0).ToReactiveProperty();
            }
        }

        public class Person
        {
            public ReactiveProperty<string> GivenName { get; private set; }
            public ReactiveProperty<string> FamilyName { get; private set; }
            public ReadOnlyReactiveProperty<string> FullName { get; private set; }

            public Person(string givenName, string familyName)
            {
                GivenName = new ReactiveProperty<string>(givenName);
                FamilyName = new ReactiveProperty<string>(familyName);
                // If change the givenName or familyName, notify with fullName!
                FullName = GivenName.CombineLatest(FamilyName, (x, y) => x + " " + y).ToReadOnlyReactiveProperty();
            }
        }

        public class Tadanoiremono
        {
            public int Hoge { get; set; }

            ~Tadanoiremono()
            {
                Debug.Log("Iremono Destructor!");
            }
        }
    }
}

#pragma warning restore 0414