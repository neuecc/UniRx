#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define SupportCustomYieldInstruction
#endif
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
using System.Linq;

#if !(UNITY_METRO || UNITY_WP8) && (UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6)
    // Fallback for Unity versions below 4.5
    using Hash = System.Collections.Hashtable;
    using HashEntry = System.Collections.DictionaryEntry;    
#else
using Hash = System.Collections.Generic.Dictionary<string, string>;
using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
using UniRx.InternalUtil;
using UniRx.Toolkit;
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

        public override string ToString()
        {
            return Condition + " " + StackTrace;
        }
    }

    static class LogHelper
    {
        // If static register callback, use Subject for event branching.

#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7)
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


    [Serializable]
    public class StringReactiveCollection : ReactiveCollection<string>
    {
    }

#if UNITY_5_3

    public class CustomYield : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                Debug.Log("FromCustomYield:" + Time.frameCount);
                return (Time.frameCount >= 300) ? false : true;
            }
        }
    }

    public class TadanoIEnumerator : IEnumerator
    {
        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            Debug.Log("TadanoIEnumerator:" + Time.frameCount);
            return (Time.frameCount >= 300) ? false : true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

#endif

    public class MyObjectPool : AsyncObjectPool<Clicker>
    {
        Transform parent;

        public MyObjectPool(Transform parent)
        {
            this.parent = parent;
        }

        protected override IObservable<Clicker> CreateInstanceAsync()
        {
            return Observable.Start(() =>
            {
                var p = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                p.transform.SetParent(parent);
                return p.AddComponent<Clicker>();
            }, TimeSpan.FromSeconds(1), Scheduler.MainThread);
        }

        protected override void OnBeforeRent(Clicker instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnBeforeReturn(Clicker instance)
        {
            instance.gameObject.SetActive(false);
        }
    }


    // test sandbox
    [Serializable]
    public class UniRxTestSandbox : MonoBehaviour
    {


        readonly static UniRx.Diagnostics.Logger logger = new UniRx.Diagnostics.Logger("UniRx.Test.NewBehaviour");

        // public UnityEvent<int> SimpleEvent;

        public Vector2ReactiveProperty V2R;

        StringBuilder logtext = new StringBuilder();

        //GameObject cube;
        //Clicker clicker;

        //[ThreadStatic]
        //static object threadstaticobj;

        public DateTime DateTimeSonomono;

        public List<string> arrayC;

        public StringReactiveCollection SerializableRxc;

        [Range(1, 10)]
        public float TadanoRange;

        [RangeReactivePropertyAttribute(1, 10)]
        public IntReactiveProperty Intxxx;
        public LongReactiveProperty LongxXXX;
        public BoolReactiveProperty Booxxx;

        [RangeReactivePropertyAttribute(1, 10)]
        public FloatReactiveProperty FloAAX;

        [RangeReactivePropertyAttribute(1, 10)]
        public DoubleReactiveProperty DuAAX;
        public MikanReactiveProperty MikanRP;
        [RangeReactivePropertyAttribute(1, 10)]
        public StringReactiveProperty Strrrrr;

        [Multiline]
        public StringReactiveProperty MultilineStrrrr;

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

        MyObjectPool objectPoolTest;

        public void Awake()
        {
            MainThreadDispatcher.Initialize();


            Application.targetFrameRate = -1;

            //LogHelper.LogCallbackAsObservable()
            //    .ObserveOnMainThread()
            //    .Where(x => x.LogType == LogType.Exception)
            //    .Subscribe(x => logtext.AppendLine(x.ToString()));

            ObservableLogger.Listener.LogToUnityDebug();
            ObservableLogger.Listener.ObserveOnMainThread().Subscribe(x =>
            {
                logtext.AppendLine(x.Message);
            });


            objectPoolTest = new ObjectTest.MyObjectPool(this.transform);

#if UNITY_5_3

            //button.OnClickAsObservable().Subscribe(_ =>
            //{
            //    UnityEngine.Debug.Log("---");
            //    Observable.EveryGameObjectUpdate().Subscribe(x => Debug.Log("EveryGameObjectUpdate" + x));
            //    Observable.EveryUpdate().Subscribe(x => Debug.Log("EveryUpdate:" + x));
            //    // Observable.EveryAfterUpdate().Subscribe(x => Debug.Log("EveryAfterUpdate:" + x));
            //    Observable.EveryLateUpdate().Subscribe(x => Debug.Log("EveryLateUpdate:" + x));
            //    Observable.EveryEndOfFrame().Subscribe(x => Debug.Log("EveryEndOfFrame:" + x));
            //    UnityEngine.Debug.Log("---");
            //});
#endif

            FloAAX.Subscribe(x => Debug.Log("FloAAX:" + x));

        }

        CompositeDisposable disposables = new CompositeDisposable();
        Subject<int> Source = new Subject<int>();


        IEnumerator TestEnumerator()
        {
            while (true)
            {
                yield return null;
            }
        }

        IEnumerator MoreTestEnumerator()
        {
            for (var i = 0; i < 1;)
            {
                yield return null;
            }
        }

        IEnumerator GetResult()
        {
            long xxx = -1;
            yield return Observable.Timer(TimeSpan.FromSeconds(1)).Select(x => 999).StartAsCoroutine(x =>
            {
                xxx = x;
            });

            Debug.Log(xxx);

            //var yi = Observable.Timer(TimeSpan.FromSeconds(1)).Select(x => 999).ToYieldInstruction();
            //yield return yi;

            //Debug.Log(yi.Result);
        }


        Subject<long> subj;
        object gate = new object();

        Clicker rentInstance;

        int counter;
        IEnumerator IncrCounter()
        {
            while (true)
            {
                yield return null;
                counter++;
            }
        }

        IDisposable cancelCoroutine;

        IEnumerator TestCoroutine(IObserver<int> observer)
        {
            var i = 0;
            while (true)
            {
                Debug.Log(i++);
                observer.OnNext(i);
                yield return null;
            }
        }


        public void OnGUI()
        {
            var xpos = 0;
            var ypos = 0;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Clear"))
            {
                logtext.Length = 0;
                disposables.Clear();
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "StartCo"))
            {
                stopwatch.Stop();
                elapsed.Clear();
                for (int i = 0; i < 10000; i++)
                {
                    Observable.FromCoroutine(IncrCounter).Subscribe().AddTo(disposables);
                }
                logger.Debug("StartCo");
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "StartMicro"))
            {
                stopwatch.Stop();
                elapsed.Clear();
                for (int i = 0; i < 10000; i++)
                {
                    Observable.FromMicroCoroutine(IncrCounter).Subscribe().AddTo(disposables);
                }
                logger.Debug("StartMicro");
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Average"))
            {
                var avg = elapsed.Average(x => x.TotalMilliseconds) + "ms";
                logger.Debug("Average:" + avg);
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ToYield"))
            {
                // StartCoroutine_Auto(ToYield());
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Every"))
            {
                this.UpdateAsObservable().Take(1).Subscribe(__ =>
                {
                    UnityEngine.Debug.Log("Register Start");
                    Observable.EveryGameObjectUpdate().Subscribe(x => Debug.Log("EveryGameObjectUpdate" + x));
                    Observable.EveryUpdate().Subscribe(x => Debug.Log("EveryUpdate:" + x));
                    Observable.EveryLateUpdate().Subscribe(x => Debug.Log("EveryLateUpdate:" + x));
                    Observable.EveryEndOfFrame().Subscribe(x => Debug.Log("EveryEndOfFrame:" + x));
                    Observable.EveryFixedUpdate().Subscribe(x => Debug.Log("EveryFixedUpdate:" + x));

                    UnityEngine.Debug.Log("Register End");
                });
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Delay"))
            {
                Observable.Return(100).DelayFrame(25, FrameCountType.Update).Subscribe(x => Debug.Log("delayed"));
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "SubscribePerf1"))
            {
                var list = Enumerable.Range(1, 10000).Select(x => new ReactiveProperty<int>(x)).ToArray();

                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var item in list)
                    {
                        item.Subscribe();
                    }
                    sw.Stop();
                    logger.Debug("Direct Subscribe:" + sw.Elapsed.TotalMilliseconds + "ms");
                }

                {
                    var r = UnityEngine.Random.Range(1, 100);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var item in list)
                    {
                        item.Value = r;
                    }
                    sw.Stop();
                    logger.Debug("Push Direct Perf:" + sw.Elapsed.TotalMilliseconds + "ms");
                }
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "SubscribePerf2"))
            {
                var list = Enumerable.Range(1, 10000).Select(x => new ReactiveProperty<int>(x)).ToArray();

                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var item in list)
                    {
                        item.Select(x => x).Subscribe();
                    }
                    sw.Stop();
                    logger.Debug("Select.Subscribe:" + sw.Elapsed.TotalMilliseconds + "ms");
                }

                {
                    var r = UnityEngine.Random.Range(1, 100);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var item in list)
                    {
                        item.Value = r;
                    }
                    sw.Stop();
                    logger.Debug("Push Select Perf:" + sw.Elapsed.TotalMilliseconds + "ms");
                }
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Subscribe Perf:"))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                var sw = System.Diagnostics.Stopwatch.StartNew();

                var list = Enumerable.Range(1, 10000).Select(x => new Subject<int>()).ToArray();
                foreach (var item in list)
                {
                    item.Where(x => x % 2 == 0).Select(x => x * x).Take(10).Subscribe();
                }

                sw.Stop();
                logger.Debug("Subscribe Perf:" + sw.Elapsed.TotalMilliseconds + "ms");
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "OnNext Perf"))
            {
                var list = Enumerable.Range(1, 10000).Select(x => new ReactiveProperty<int>(0)).ToArray();

                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var item in list)
                    {
                        item.Where(x => x % 2 == 0).Select(x => x * x).Take(10).Subscribe();
                    }
                    sw.Stop();
                    logger.Debug("Subscribe + OnNext Perf:" + sw.Elapsed.TotalMilliseconds + "ms");
                }

                {
                    var r = UnityEngine.Random.Range(1, 100);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var item in list)
                    {
                        item.Value = r;
                    }
                    sw.Stop();
                    logger.Debug("OnNext Perf:" + sw.Elapsed.TotalMilliseconds + "ms");
                }
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Check Compare Perf"))
            {
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    for (int i = 0; i < 10000; i++)
                    {
                        var s = new Subject<long>();
                        Interlocked.CompareExchange(ref subj, s, null);
                    }

                    logger.Debug("Interlocked:" + sw.Elapsed.TotalMilliseconds + "ms");
                }
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    for (int i = 0; i < 10000; i++)
                    {
                        lock (gate)
                        {
                            if (subj == null)
                            {
                                subj = new Subject<long>();
                            }
                        }
                        subj = null;
                    }

                    logger.Debug("Lock:" + sw.Elapsed.TotalMilliseconds + "ms");
                }
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    for (int i = 0; i < 10000; i++)
                    {
                        if (subj == null)
                        {
                            subj = new Subject<long>();
                        }
                        subj = null;
                    }

                    logger.Debug("Plain:" + sw.Elapsed.TotalMilliseconds + "ms");
                }
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "ObserveOn Check:"))
            {
                var s = new Subject<int>();

                Debug.Log("Before ObserveOn:" + Time.frameCount);
                s.ObserveOnMainThread()
                    .Subscribe(x =>
                    {
                        Debug.Log("Subscribe:" + Time.frameCount + "|" + x);
                    });

                s.OnNext(997);
                s.OnNext(998);
                s.OnNext(999);
                Debug.Log("After OnNext:" + Time.frameCount);
            }
            ypos += 100;

            ypos = 0;
            xpos += 100;
            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Preload"))
            {
                objectPoolTest.PreloadAsync(100, 20).Subscribe(_ => Debug.Log("preload complete"));
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "Run"))
            {
                cancelCoroutine = Observable.FromCoroutine<int>(observer => TestCoroutine(observer))
                    .Subscribe(x => Debug.Log("Subs " + x));
            }
            ypos += 100;

            if (GUI.Button(new Rect(xpos, ypos, 100, 100), "return"))
            {
                cancelCoroutine.Dispose();
            }
            ypos += 100;


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



        List<TimeSpan> elapsed = new List<TimeSpan>();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        void Update()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        void LateUpdate()
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
                elapsed.Add(stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }
    }
}

#pragma warning restore 0414
