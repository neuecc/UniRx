
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx.Tests
{

    public static class UnitTests
    {
        public static void Clear(GameObject resultVertical)
        {
            foreach (Transform child in resultVertical.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void SetButtons(Button buttonPrefab, GameObject buttonVertical, Result resultPrefab, GameObject resultVertical)
        {
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservableConcurrencyTest(3)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservableConcurrencyTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ConversionTest(5)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ConversionTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "DoTest(7)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(DoTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "DurabilityTest(4)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(DurabilityTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ToTest(2)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ToTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "WhenAllTest(3)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(WhenAllTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "SelectMany(8)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(SelectMany.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "AggregateTest(2)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(AggregateTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "TakeTest(1)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(TakeTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "RangeTest(1)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(RangeTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "QueueWorkerTest(1)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(QueueWorkerTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "DisposableTest(4)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(DisposableTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ErrorHandlingTest(3)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ErrorHandlingTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservableEventsTest(2)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservableEventsTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservablePagingTest(24)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservablePagingTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservableTimeTest(9)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservableTimeTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservableConcatTest(19)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservableConcatTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservableTest(13)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservableTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "SchedulerTest(4)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(SchedulerTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ReactriveDictionaryTest(1)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ReactriveDictionaryTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "SubjectTests(6)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(SubjectTests.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
            {
                var button = GameObject.Instantiate(buttonPrefab);
                button.GetComponentInChildren<Text>().text = "ObservableGeneratorTest(8)";
                button.OnClickAsObservable().Subscribe(_ =>
                {
                    Clear(resultVertical);
                    MainThreadDispatcher.StartCoroutine(ObservableGeneratorTest.Run(resultPrefab, resultVertical));
                });
                button.transform.SetParent(buttonVertical.transform);
            }
        }
    }



    public partial class ObservableConcurrencyTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservableConcurrencyTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ObserveOnTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ObserveOnTest();
                    r.Message.Value = "ObserveOnTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ObserveOnTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ObserveOnTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "AmbTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.AmbTest();
                    r.Message.Value = "AmbTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "AmbTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "AmbTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "AmbMultiTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.AmbMultiTest();
                    r.Message.Value = "AmbMultiTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "AmbMultiTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "AmbMultiTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void ObserveOnTest()
        {
            var xs = Observable.Range(1, 10)
                .ObserveOn(Scheduler.ThreadPool)
                .ToArrayWait();

            xs.OrderBy(x => x).IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

            var s = new Subject<int>();

            var list = new List<Notification<int>>();
            s.ObserveOn(Scheduler.Immediate).Materialize().Subscribe(list.Add);

            s.OnError(new Exception());

            list[0].Kind.Is(NotificationKind.OnError);

            s = new Subject<int>();
            s.ObserveOn(Scheduler.Immediate).Materialize().Subscribe(list.Add);

            s.OnCompleted();
            list[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void AmbTest()
        {
            var xs = Observable.Return(10).Delay(TimeSpan.FromSeconds(1)).Concat(Observable.Range(1, 3));

            var xss = Observable.Return(10).Concat(Observable.Range(1, 3));
            xss.ToArray().Wait();
            xss.ToArray().Wait();
            xss.ToArray().Wait();


            var ys = Observable.Return(30).Delay(TimeSpan.FromSeconds(2)).Concat(Observable.Range(5, 3));

            // win left
            var result = xs.Amb(ys).ToArray().Wait();

            result[0].Is(10);
            result[1].Is(1);
            result[2].Is(2);
            result[3].Is(3);

            // win right
            result = ys.Amb(xs).ToArray().Wait();

            result[0].Is(10);
            result[1].Is(1);
            result[2].Is(2);
            result[3].Is(3);
        }




        [TestMethod]
        public void AmbMultiTest()
        {
            var xs = Observable.Return(10).Delay(TimeSpan.FromSeconds(5)).Concat(Observable.Range(1, 3));
            var ys = Observable.Return(30).Delay(TimeSpan.FromSeconds(1)).Concat(Observable.Range(5, 3));
            var zs = Observable.Return(50).Delay(TimeSpan.FromSeconds(3)).Concat(Observable.Range(9, 3));

            // win center
            var result = Observable.Amb(xs, ys, zs).ToArray().Wait();

            result[0].Is(30);
            result[1].Is(5);
            result[2].Is(6);
            result[3].Is(7);

            // win first
            result = Observable.Amb(new[] { ys, xs, zs }.AsEnumerable()).ToArray().Wait();

            result[0].Is(30);
            result[1].Is(5);
            result[2].Is(6);
            result[3].Is(7);

            // win last
            result = Observable.Amb(new[] { zs, xs, ys }.AsEnumerable()).ToArray().Wait();

            result[0].Is(30);
            result[1].Is(5);
            result[2].Is(6);
            result[3].Is(7);
        }



    }


    public partial class ConversionTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ConversionTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "AsObservable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.AsObservable();
                    r.Message.Value = "AsObservable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "AsObservable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "AsObservable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "AsUnitObservable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.AsUnitObservable();
                    r.Message.Value = "AsUnitObservable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "AsUnitObservable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "AsUnitObservable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ToObservable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ToObservable();
                    r.Message.Value = "ToObservable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ToObservable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ToObservable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Cast";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Cast();
                    r.Message.Value = "Cast OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Cast NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Cast NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "OfType";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.OfType();
                    r.Message.Value = "OfType OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "OfType NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "OfType NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void AsObservable()
        {
            Observable.Range(1, 10).AsObservable().ToArrayWait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }




        [TestMethod]
        public void AsUnitObservable()
        {
            Observable.Range(1, 3).AsUnitObservable().ToArrayWait().IsCollection(Unit.Default, Unit.Default, Unit.Default);
        }




        [TestMethod]
        public void ToObservable()
        {
            Enumerable.Range(1, 3).ToObservable(Scheduler.CurrentThread).ToArrayWait().IsCollection(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.ThreadPool).ToArrayWait().IsCollection(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.Immediate).ToArrayWait().IsCollection(1, 2, 3);
        }




        [TestMethod]
        public void Cast()
        {
            Observable.Range(1, 3).Cast<int, object>().ToArrayWait().IsCollection(1, 2, 3);
        }




        [TestMethod]
        public void OfType()
        {
            var subject = new Subject<object>();

            var list = new List<int>();
            subject.OfType(default(int)).Subscribe(x => list.Add(x));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("hogehoge");
            subject.OnNext(3);

            list.IsCollection(1, 2, 3);
        }



    }


    public partial class DoTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new DoTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Do";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Do();
                    r.Message.Value = "Do OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Do NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Do NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DoObserver";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DoObserver();
                    r.Message.Value = "DoObserver OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DoObserver NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DoObserver NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DoOnError";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DoOnError();
                    r.Message.Value = "DoOnError OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DoOnError NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DoOnError NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DoOnCompleted";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DoOnCompleted();
                    r.Message.Value = "DoOnCompleted OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DoOnCompleted NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DoOnCompleted NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DoOnTerminate";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DoOnTerminate();
                    r.Message.Value = "DoOnTerminate OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DoOnTerminate NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DoOnTerminate NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DoOnSubscribe";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DoOnSubscribe();
                    r.Message.Value = "DoOnSubscribe OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DoOnSubscribe NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DoOnSubscribe NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DoOnCancel";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DoOnCancel();
                    r.Message.Value = "DoOnCancel OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DoOnCancel NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DoOnCancel NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }




        [TestMethod]
        public void Do()
        {
            var list = new List<int>();
            Observable.Empty<int>().Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).DefaultIfEmpty().Wait();
            list.IsCollection(1000);

            list.Clear();
            Observable.Range(1, 5).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Wait();
            list.IsCollection(1, 2, 3, 4, 5, 1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(1, 2, 3, 4, 5, 100);
        }




        [TestMethod]
        public void DoObserver()
        {
            var observer = new ListObserver();
            Observable.Empty<int>().Do(observer).DefaultIfEmpty().Wait();
            observer.list.IsCollection(1000);

            observer = new ListObserver();
            Observable.Range(1, 5).Do(observer).Wait();
            observer.list.IsCollection(1, 2, 3, 4, 5, 1000);

            observer = new ListObserver();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(observer).Subscribe(_ => { }, _ => { }, () => { });
            observer.list.IsCollection(1, 2, 3, 4, 5, 100);
        }




        [TestMethod]
        public void DoOnError()
        {
            var list = new List<int>();
            Observable.Empty<int>().DoOnError(_ => list.Add(100)).DefaultIfEmpty().Wait();
            list.IsCollection();

            list.Clear();
            Observable.Range(1, 5).DoOnError(_ => list.Add(100)).Wait();
            list.IsCollection();

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnError(_ => list.Add(100)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(100);
        }




        [TestMethod]
        public void DoOnCompleted()
        {
            var list = new List<int>();
            Observable.Empty<int>().DoOnCompleted(() => list.Add(1000)).DefaultIfEmpty().Wait();
            list.IsCollection(1000);

            list.Clear();
            Observable.Range(1, 5).DoOnCompleted(() => list.Add(1000)).Wait();
            list.IsCollection(1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnCompleted(() => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection();
        }




        [TestMethod]
        public void DoOnTerminate()
        {
            var list = new List<int>();
            Observable.Empty<int>().DoOnTerminate(() => list.Add(1000)).DefaultIfEmpty().Wait();
            list.IsCollection(1000);

            list.Clear();
            Observable.Range(1, 5).DoOnTerminate(() => list.Add(1000)).Wait();
            list.IsCollection(1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnTerminate(() => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(1000);
        }




        [TestMethod]
        public void DoOnSubscribe()
        {
            var list = new List<int>();
            Observable.Empty<int>()
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnSubscribe(() => list.Add(10000)).DefaultIfEmpty().Wait();
            list.IsCollection(10000, 1000);

            list.Clear();
            Observable.Range(1, 5)
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnSubscribe(() => list.Add(10000)).Wait();
            list.IsCollection(10000, 1, 2, 3, 4, 5, 1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception()))
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnSubscribe(() => list.Add(10000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(10000, 1, 2, 3, 4, 5, 100);
        }




        [TestMethod]
        public void DoOnCancel()
        {
            var list = new List<int>();
            Observable.Empty<int>()
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnCancel(() => list.Add(5000))
                .DoOnCancel(() => list.Add(10000))
                .DefaultIfEmpty()
                .Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(1000);

            list.Clear();
            Observable.Throw<int>(new Exception())
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnCancel(() => list.Add(5000))
                .DoOnCancel(() => list.Add(10000))
                .DefaultIfEmpty()
                .Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(100);

            list.Clear();
            var subscription = Observable.Timer(TimeSpan.FromMilliseconds(1000)).Select(x => (int)x)
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnCancel(() => list.Add(5000))
                .DoOnCancel(() => list.Add(10000))
                .Subscribe();
            subscription.Dispose();
            list.IsCollection(5000, 10000);
        }



    }


    public partial class DurabilityTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new DurabilityTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "FromEventPattern";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.FromEventPattern();
                    r.Message.Value = "FromEventPattern OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "FromEventPattern NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "FromEventPattern NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "FromEventUnityLike";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.FromEventUnityLike();
                    r.Message.Value = "FromEventUnityLike OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "FromEventUnityLike NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "FromEventUnityLike NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "FromEventUnity";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.FromEventUnity();
                    r.Message.Value = "FromEventUnity OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "FromEventUnity NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "FromEventUnity NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Durability";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Durability();
                    r.Message.Value = "Durability OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Durability NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Durability NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }




        [TestMethod]
        public void FromEventPattern()
        {
            var tester = new EventTester();

            {
                var list = new List<int>();
                var d = Observable.FromEventPattern<EventHandler<MyEventArgs>, MyEventArgs>((h) => h.Invoke, h => tester.genericEventHandler += h, h => tester.genericEventHandler -= h)
                    .Subscribe(xx =>
                    {
                        list.Add(xx.EventArgs.MyProperty);
                        Observable.Return(xx.EventArgs.MyProperty)
                            .Do(x => { if (x == 1) throw new Exception(); })
                            .Subscribe(x => list.Add(x));
                    });

                try { tester.Fire(5); } catch { }
                try { tester.Fire(1); } catch { }
                try { tester.Fire(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                tester.Fire(1000);
                list.Count.Is(5);
            }
        }




        [TestMethod]
        public void FromEventUnityLike()
        {
            var tester = new EventTester();

            {
                var list = new List<int>();
                var d = Observable.FromEvent<LikeUnityAction<int>, int>(h => new LikeUnityAction<int>(h), h => tester.intUnityAction += h, h => tester.intUnityAction -= h)
                    .Subscribe(xx =>
                    {
                        list.Add(xx);
                        Observable.Return(xx)
                            .Do(x => { if (x == 1) throw new Exception(); })
                            .Subscribe(x => list.Add(x));
                    });

                try { tester.Fire(5); } catch { }
                try { tester.Fire(1); } catch { }
                try { tester.Fire(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                tester.Fire(1000);
                list.Count.Is(5);
            }

            {
                var i = 0;
                var list = new List<int>();
                var d = Observable.FromEvent<LikeUnityAction>(h => new LikeUnityAction(h), h => tester.unityAction += h, h => tester.unityAction -= h)
                    .Subscribe(xx =>
                    {
                        list.Add(i);
                        Observable.Return(i)
                            .Do(x => { if (x == 1) throw new Exception(); })
                            .Subscribe(x => list.Add(i));
                    });

                try { i = 5; tester.Fire(5); } catch { }
                try { i = 1; tester.Fire(1); } catch { }
                try { i = 10; tester.Fire(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                tester.Fire(1000);
                list.Count.Is(5);
            }
        }




        [TestMethod]
        public void FromEventUnity()
        {
            var tester = new EventTester();

            {
                var list = new List<int>();
                var d = Observable.FromEvent<int>(h => tester.intAction += h, h => tester.intAction -= h)
                    .Subscribe(xx =>
                    {
                        list.Add(xx);
                        Observable.Return(xx)
                            .Do(x => { if (x == 1) throw new Exception(); })
                            .Subscribe(x => list.Add(x));
                    });

                try { tester.Fire(5); } catch { }
                try { tester.Fire(1); } catch { }
                try { tester.Fire(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                tester.Fire(1000);
                list.Count.Is(5);
            }

            {
                var i = 0;
                var list = new List<int>();
                var d = Observable.FromEvent(h => tester.unitAction += h, h => tester.unitAction -= h)
                    .Subscribe(xx =>
                    {
                        list.Add(i);
                        Observable.Return(i)
                            .Do(x => { if (x == 1) throw new Exception(); })
                            .Subscribe(x => list.Add(i));
                    });

                try { i = 5; tester.Fire(5); } catch { }
                try { i = 1; tester.Fire(1); } catch { }
                try { i = 10; tester.Fire(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                tester.Fire(1000);
                list.Count.Is(5);
            }
        }




        [TestMethod]
        public void Durability()
        {
            {
                var s1 = new Subject<int>();

                var list = new List<int>();
                var d = s1.Subscribe(xx =>
                {
                    list.Add(xx);
                    Observable.Return(xx)
                        .Do(x => { if (x == 1) throw new Exception(); })
                        .Subscribe(x => list.Add(x));
                });

                try { s1.OnNext(5); } catch { }
                try { s1.OnNext(1); } catch { }
                try { s1.OnNext(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                s1.OnNext(1000);
                list.Count.Is(5);
            }
            {
                var s1 = new Subject<int>();

                var list = new List<int>();
                var d = s1.Select(x => x).Take(1000).Subscribe(xx =>
                {
                    list.Add(xx);
                    Observable.Return(xx)
                        .Do(x => { if (x == 1) throw new Exception(); })
                        .Subscribe(x => list.Add(x));
                });

                try { s1.OnNext(5); } catch { }
                try { s1.OnNext(1); } catch { }
                try { s1.OnNext(10); } catch { }

                list.IsCollection(5, 5, 1, 10, 10);
                d.Dispose();
                s1.OnNext(1000);
                list.Count.Is(5);
            }
        }



    }


    public partial class ToTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ToTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ToArray";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ToArray();
                    r.Message.Value = "ToArray OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ToArray NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ToArray NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ToList";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ToList();
                    r.Message.Value = "ToList OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ToList NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ToList NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }




        [TestMethod]
        public void ToArray()
        {
            Observable.Empty<int>().ToArray().Wait().IsCollection();
            Observable.Return(10).ToArray().Wait().IsCollection(10);
            Observable.Range(1, 10).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }




        [TestMethod]
        public void ToList()
        {
            Observable.Empty<int>().ToList().Wait().IsCollection();
            Observable.Return(10).ToList().Wait().IsCollection(10);
            Observable.Range(1, 10).ToList().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }



    }


    public partial class WhenAllTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new WhenAllTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "WhenAllEmpty";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.WhenAllEmpty();
                    r.Message.Value = "WhenAllEmpty OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "WhenAllEmpty NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "WhenAllEmpty NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "WhenAll";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.WhenAll();
                    r.Message.Value = "WhenAll OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "WhenAll NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "WhenAll NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "WhenAllEnumerable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.WhenAllEnumerable();
                    r.Message.Value = "WhenAllEnumerable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "WhenAllEnumerable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "WhenAllEnumerable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void WhenAllEmpty()
        {
            var xs = Observable.WhenAll(new IObservable<int>[0]).Wait();
            xs.Length.Is(0);

            var xs2 = Observable.WhenAll(Enumerable.Empty<IObservable<int>>().Select(x => x)).Wait();
            xs2.Length.Is(0);
        }




        [TestMethod]
        public void WhenAll()
        {
            var xs = Observable.WhenAll(
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4))
                .Wait();

            xs.IsCollection(100, 5, 4);
        }




        [TestMethod]
        public void WhenAllEnumerable()
        {
            var xs = new[] {
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4)
            }.Select(x => x).WhenAll().Wait();
                
            xs.IsCollection(100, 5, 4);
        }



    }


    public partial class SelectMany
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new SelectMany();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Selector";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Selector();
                    r.Message.Value = "Selector OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Selector NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Selector NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SelectorWithIndex";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SelectorWithIndex();
                    r.Message.Value = "SelectorWithIndex OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SelectorWithIndex NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SelectorWithIndex NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SelectorEnumerable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SelectorEnumerable();
                    r.Message.Value = "SelectorEnumerable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SelectorEnumerable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SelectorEnumerable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SelectorEnumerableWithIndex";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SelectorEnumerableWithIndex();
                    r.Message.Value = "SelectorEnumerableWithIndex OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SelectorEnumerableWithIndex NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SelectorEnumerableWithIndex NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ResultSelector";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ResultSelector();
                    r.Message.Value = "ResultSelector OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ResultSelector NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ResultSelector NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ResultSelectorWithIndex";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ResultSelectorWithIndex();
                    r.Message.Value = "ResultSelectorWithIndex OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ResultSelectorWithIndex NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ResultSelectorWithIndex NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ResultSelectorEnumerable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ResultSelectorEnumerable();
                    r.Message.Value = "ResultSelectorEnumerable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ResultSelectorEnumerable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ResultSelectorEnumerable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ResultSelectorEnumerableWithIndex";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ResultSelectorEnumerableWithIndex();
                    r.Message.Value = "ResultSelectorEnumerableWithIndex OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ResultSelectorEnumerableWithIndex NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ResultSelectorEnumerableWithIndex NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Selector()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext(200)", "OnNext(300)", "OnNext(400)", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext(200)", "OnNext(300)", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext(200)", "OnNext(300)", "OnError(System.Exception)");
            }
            {
                // OnCompleted Case2
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c)
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                b.OnCompleted();
                c.OnCompleted();

                // check
                list.IsCollection("OnNext(200)", "OnNext(300)");

                a.OnCompleted();

                list.IsCollection("OnNext(200)", "OnNext(300)", "OnCompleted()");
            }
        }




        [TestMethod]
        public void SelectorWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((200, 0))", "OnNext((300, 1))", "OnNext((400, 1))", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((200, 0))", "OnNext((300, 1))", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((200, 0))", "OnNext((300, 1))", "OnError(System.Exception)");
            }
        }




        [TestMethod]
        public void SelectorEnumerable()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.IsCollection("OnNext(1)", "OnNext(2)", "OnNext(3)", "OnNext(10)", "OnNext(10)", "OnNext(10)", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.IsCollection("OnNext(1)", "OnNext(2)", "OnNext(3)", "OnNext(10)", "OnNext(10)", "OnNext(10)", "OnError(System.Exception)");
            }
        }




        [TestMethod]
        public void SelectorEnumerableWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.IsCollection("OnNext(0)", "OnNext(1)", "OnNext(2)", "OnNext(1)", "OnNext(1)", "OnNext(1)", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.IsCollection("OnNext(0)", "OnNext(1)", "OnNext(2)", "OnNext(1)", "OnNext(1)", "OnNext(1)", "OnError(System.Exception)");
            }
        }






        [TestMethod]
        public void ResultSelector()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c, (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 200))", "OnNext((100, 300))", "OnNext((100, 400))", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c, (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 200))", "OnNext((100, 300))", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? b : c, (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 200))", "OnNext((100, 300))", "OnError(System.Exception)");
            }
        }




        [TestMethod]
        public void ResultSelectorWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)), (x, i1, y, i2) => Tuple.Create(x, i1, y.Item1, y.Item2, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 0, 200, 0, 0))", "OnNext((100, 1, 300, 1, 0))", "OnNext((100, 1, 400, 1, 1))", "OnCompleted()");
            }
            {
                // OnError A
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)), (x, i1, y, i2) => Tuple.Create(x, i1, y.Item1, y.Item2, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnError(new Exception());
                b.OnCompleted();
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 0, 200, 0, 0))", "OnNext((100, 1, 300, 1, 0))", "OnError(System.Exception)");
            }
            {
                // OnError B
                var a = new Subject<int>();
                var b = new Subject<int>();
                var c = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? b.Select(y => Tuple.Create(y, i)) : c.Select(y => Tuple.Create(y, i)), (x, i1, y, i2) => Tuple.Create(x, i1, y.Item1, y.Item2, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do

                b.OnNext(200);
                c.OnNext(300);
                a.OnCompleted();
                b.OnError(new Exception());
                c.OnNext(400);
                c.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 0, 200, 0, 0))", "OnNext((100, 1, 300, 1, 0))", "OnError(System.Exception)");
            }
        }




        [TestMethod]
        public void ResultSelectorEnumerable()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3), (x, y) => Tuple.Create(x, y))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 1))", "OnNext((10, 2))", "OnNext((10, 3))", "OnNext((100, 10))", "OnNext((100, 10))", "OnNext((100, 10))", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany(x => (x == 10) ? Enumerable.Range(1, 3) : Enumerable.Repeat(10, 3), (x, y) => Tuple.Create(x, y))
                 .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.IsCollection("OnNext((10, 1))", "OnNext((10, 2))", "OnNext((10, 3))", "OnNext((100, 10))", "OnNext((100, 10))", "OnNext((100, 10))", "OnError(System.Exception)");
            }
        }




        [TestMethod]
        public void ResultSelectorEnumerableWithIndex()
        {
            {
                // OnCompleted Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3), (x, i1, y, i2) => Tuple.Create(x, i1, y, i2))
                 .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnCompleted();

                // check
                list.IsCollection("OnNext((10, 0, 0, 0))", "OnNext((10, 0, 1, 1))", "OnNext((10, 0, 2, 2))", "OnNext((100, 1, 1, 0))", "OnNext((100, 1, 1, 1))", "OnNext((100, 1, 1, 2))", "OnCompleted()");
            }
            {
                // OnError Case
                var a = new Subject<int>();
                var list = new List<string>();
                a.SelectMany((x, i) => (x == 10) ? Enumerable.Range(i, 3) : Enumerable.Repeat(i, 3), (x, i1, y, i2) => Tuple.Create(x, i1, y, i2))
                    .Materialize().Select(x => x.ToString()).Subscribe(x => list.Add(x));
                a.OnNext(10);
                a.OnNext(100);

                // do
                a.OnError(new Exception());

                // check
                list.IsCollection("OnNext((10, 0, 0, 0))", "OnNext((10, 0, 1, 1))", "OnNext((10, 0, 2, 2))", "OnNext((100, 1, 1, 0))", "OnNext((100, 1, 1, 1))", "OnNext((100, 1, 1, 2))", "OnError(System.Exception)");
            }
        }



    }


    public partial class AggregateTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new AggregateTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Scan";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Scan();
                    r.Message.Value = "Scan OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Scan NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Scan NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Aggregate";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Aggregate();
                    r.Message.Value = "Aggregate OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Aggregate NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Aggregate NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Scan()
        {
            var range = Observable.Range(1, 5);

            range.Scan((x, y) => x + y).ToArrayWait().IsCollection(1, 3, 6, 10, 15);
            range.Scan(100, (x, y) => x + y).ToArrayWait().IsCollection(101, 103, 106, 110, 115);

            Observable.Empty<int>().Scan((x, y) => x + y).ToArrayWait().IsCollection();
            Observable.Empty<int>().Scan(100, (x, y) => x + y).ToArrayWait().IsCollection();
        }




        [TestMethod]
        public void Aggregate()
        {
            AssertEx.Throws<InvalidOperationException>(() => Observable.Empty<int>().Aggregate((x, y) => x + y).Wait());
            Observable.Range(1, 5).Aggregate((x, y) => x + y).Wait().Is(15);

            Observable.Empty<int>().Aggregate(100, (x, y) => x + y).Wait().Is(100);
            Observable.Range(1, 5).Aggregate(100, (x, y) => x + y).Wait().Is(115);

            Observable.Empty<int>().Aggregate(100, (x, y) => x + y, x => x + x).Wait().Is(200);
            Observable.Range(1, 5).Aggregate(100, (x, y) => x + y, x => x + x).Wait().Is(230);
        }



    }


    public partial class TakeTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new TakeTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TakeCount";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TakeCount();
                    r.Message.Value = "TakeCount OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TakeCount NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TakeCount NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void TakeCount()
        {
            var range = Observable.Range(1, 10);

            AssertEx.Throws<ArgumentOutOfRangeException>(() => range.Take(-1));

            range.Take(0).ToArray().Wait().Length.Is(0);

            range.Take(3).ToArrayWait().IsCollection(1, 2, 3);
            range.Take(15).ToArrayWait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }



    }


    public partial class RangeTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new RangeTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Range";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Range();
                    r.Message.Value = "Range OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Range NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Range NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Range()
        {
            AssertEx.Throws<ArgumentOutOfRangeException>(() => Observable.Range(1, -1).ToArray().Wait());

            Observable.Range(1, 0).ToArray().Wait().Length.Is(0);
            Observable.Range(1, 10).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }



    }


    public partial class QueueWorkerTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new QueueWorkerTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Enq";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Enq();
                    r.Message.Value = "Enq OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Enq NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Enq NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Enq()
        {
            var q = new UniRx.InternalUtil.ThreadSafeQueueWorker();

            var l = new List<int>();
            q.Enqueue(x => l.Add((int)x), 1);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -1);
            q.Enqueue(x => l.Add((int)x), 2);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -2);
            q.Enqueue(x => l.Add((int)x), 3);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -3);
            q.Enqueue(x => l.Add((int)x), 4);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -4);
            q.Enqueue(x => l.Add((int)x), 5);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -5);
            q.Enqueue(x => l.Add((int)x), 6);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -6);
            q.Enqueue(x => l.Add((int)x), 7);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -7);
            q.Enqueue(x => l.Add((int)x), 8);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -8);
            q.Enqueue(x => l.Add((int)x), 9);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -9);
            q.Enqueue(x => l.Add((int)x), 10);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -10);
            q.Enqueue(x => l.Add((int)x), 11);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -11);
            q.Enqueue(x => l.Add((int)x), 12);

            q.ExecuteAll(ex => { });

            l.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.IsCollection(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Count.Is(0);

            q.Enqueue(x => l.Add((int)x), 1);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -1);
            q.Enqueue(x => l.Add((int)x), 2);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -2);
            q.Enqueue(x => l.Add((int)x), 3);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -3);
            q.Enqueue(x => l.Add((int)x), 4);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -4);
            q.Enqueue(x => l.Add((int)x), 5);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -5);
            q.Enqueue(x => l.Add((int)x), 6);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -6);
            q.Enqueue(x => l.Add((int)x), 7);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -7);
            q.Enqueue(x => l.Add((int)x), 8);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -8);
            q.Enqueue(x => l.Add((int)x), 9);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -9);
            q.Enqueue(x => l.Add((int)x), 10);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -10);
            q.Enqueue(x => l.Add((int)x), 11);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -11);
            q.Enqueue(x => l.Add((int)x), 12);

            q.ExecuteAll(ex => { });
            l.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.IsCollection(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Count.Is(0);
        }



    }


    public partial class DisposableTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new DisposableTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SingleAssignment";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SingleAssignment();
                    r.Message.Value = "SingleAssignment OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SingleAssignment NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SingleAssignment NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "MultipleAssignment";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.MultipleAssignment();
                    r.Message.Value = "MultipleAssignment OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "MultipleAssignment NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "MultipleAssignment NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Serial";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Serial();
                    r.Message.Value = "Serial OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Serial NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Serial NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Boolean";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Boolean();
                    r.Message.Value = "Boolean OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Boolean NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Boolean NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void SingleAssignment()
        {
            var d = new SingleAssignmentDisposable();
            d.IsDisposed.IsFalse();
            var id1 = new IdDisp(1);
            var id2 = new IdDisp(2);
            var id3 = new IdDisp(3);

            // dispose first
            d.Dispose();
            d.IsDisposed.IsTrue();

            d.Disposable = id1; id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // normal flow
            d = new SingleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);

            d.Disposable = id1; id1.IsDisposed.IsFalse();
            d.Dispose();
            id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // exception flow
            d = new SingleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);
            d.Disposable = id1;
            AssertEx.Catch<InvalidOperationException>(() => d.Disposable = id2);

            // null
            d = new SingleAssignmentDisposable();
            id1 = new IdDisp(1);
            d.Disposable = null;
            d.Dispose();
            d.Disposable = null;
        }




        [TestMethod]
        public void MultipleAssignment()
        {
            var d = new MultipleAssignmentDisposable();
            d.IsDisposed.IsFalse();
            var id1 = new IdDisp(1);
            var id2 = new IdDisp(2);
            var id3 = new IdDisp(3);

            // dispose first
            d.Dispose();
            d.IsDisposed.IsTrue();

            d.Disposable = id1; id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // normal flow
            d = new MultipleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);

            d.Disposable = id1; id1.IsDisposed.IsFalse();
            d.Dispose();
            id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // exception flow
            d = new MultipleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);
            d.Disposable = id1;
            d.Disposable = id2;
            d.Disposable = id3;
            d.Dispose();
            id1.IsDisposed.IsFalse();
            id2.IsDisposed.IsFalse();
            id3.IsDisposed.IsTrue();

            // null
            d = new MultipleAssignmentDisposable();
            id1 = new IdDisp(1);
            d.Disposable = null;
            d.Dispose();
            d.Disposable = null;
        }




        [TestMethod]
        public void Serial()
        {
            var d = new SerialDisposable();
            d.IsDisposed.IsFalse();
            var id1 = new IdDisp(1);
            var id2 = new IdDisp(2);
            var id3 = new IdDisp(3);

            // dispose first
            d.Dispose();
            d.IsDisposed.IsTrue();

            d.Disposable = id1; id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // normal flow
            d = new SerialDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);

            d.Disposable = id1; id1.IsDisposed.IsFalse();
            d.Dispose();
            id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // exception flow
            d = new SerialDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);
            d.Disposable = id1;
            id1.IsDisposed.IsFalse();
            d.Disposable = id2;
            id1.IsDisposed.IsTrue();
            id2.IsDisposed.IsFalse();
            d.Disposable = id3;
            id2.IsDisposed.IsTrue();
            id3.IsDisposed.IsFalse();

            d.Dispose();
            
            id3.IsDisposed.IsTrue();

            // null
            d = new SerialDisposable();
            id1 = new IdDisp(1);
            d.Disposable = null;
            d.Dispose();
            d.Disposable = null;
        }




        [TestMethod]
        public void Boolean()
        {
            var bd = new BooleanDisposable();
            bd.IsDisposed.IsFalse();
            bd.Dispose();
            bd.IsDisposed.IsTrue();
        }



    }


    public partial class ErrorHandlingTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ErrorHandlingTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Finally";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Finally();
                    r.Message.Value = "Finally OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Finally NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Finally NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Catch";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Catch();
                    r.Message.Value = "Catch OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Catch NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Catch NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CatchEnumerable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CatchEnumerable();
                    r.Message.Value = "CatchEnumerable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CatchEnumerable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CatchEnumerable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Finally()
        {
            var called = false;
            try
            {
                Observable.Range(1, 10, Scheduler.Immediate)
                    .Do(x => { throw new Exception(); })
                    .Finally(() => called = true)
                    .Subscribe();
            }
            catch
            {
            }
            finally
            {
                called.IsTrue();
            }
        }




        [TestMethod]
        public void Catch()
        {
            var xs = Observable.Return(2, Scheduler.ThreadPool).Concat(Observable.Throw<int>(new InvalidOperationException()))
                .Catch((InvalidOperationException ex) =>
                {
                    return Observable.Range(1, 3);
                })
                .ToArrayWait();

            xs.IsCollection(2, 1, 2, 3);
        }




        [TestMethod]
        public void CatchEnumerable()
        {
            {
                var xs = new[]
                {
                    Observable.Return(2).Concat(Observable.Throw<int>(new Exception())),
                    Observable.Return(99).Concat(Observable.Throw<int>(new Exception())),
                    Observable.Range(10,2)
                }
                .Catch()
                .Materialize()
                .ToArrayWait();

                xs[0].Value.Is(2);
                xs[1].Value.Is(99);
                xs[2].Value.Is(10);
                xs[3].Value.Is(11);
                xs[4].Kind.Is(NotificationKind.OnCompleted);
            }
            {
                var xs = new[]
                {
                    Observable.Return(2).Concat(Observable.Throw<int>(new Exception())),
                    Observable.Return(99).Concat(Observable.Throw<int>(new Exception()))
                }
                .Catch()
                .Materialize()
                .ToArrayWait();

                xs[0].Value.Is(2);
                xs[1].Value.Is(99);
                xs[2].Kind.Is(NotificationKind.OnError);
            }
        }



    }


    public partial class ObservableEventsTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservableEventsTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "FromEventPattern";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.FromEventPattern();
                    r.Message.Value = "FromEventPattern OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "FromEventPattern NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "FromEventPattern NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "FromEvent";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.FromEvent();
                    r.Message.Value = "FromEvent OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "FromEvent NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "FromEvent NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }





        [TestMethod]
        public void FromEventPattern()
        {
            var test = new EventTestesr();

            {
                var isRaised = false;
                var d = Observable.FromEventPattern<EventHandler, EventArgs>(
                    h => h.Invoke,
                    h => test.Event1 += h, h => test.Event1 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(1);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(1);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEventPattern<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => h.Invoke,
                    h => test.Event2 += h, h => test.Event2 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(2);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(2);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEventPattern<MyEventHandler, MyEventArgs>(
                    h => h.Invoke,
                    h => test.Event3 += h, h => test.Event3 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(3);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(3);
                isRaised.IsFalse();
            }
        }




        [TestMethod]
        public void FromEvent()
        {
            var test = new EventTestesr();

            {
                var isRaised = false;
                var d = Observable.FromEvent<EventHandler, EventArgs>(
                    h => (sender, e) => h.Invoke(e),
                    h => test.Event1 += h, h => test.Event1 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(1);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(1);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => (sender, e) => h.Invoke(e),
                    h => test.Event2 += h, h => test.Event2 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(2);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(2);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<MyEventHandler, MyEventArgs>(
                    h => (sender, e) => h.Invoke(e),
                    h => test.Event3 += h, h => test.Event3 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(3);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(3);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<Action, Unit>(
                    h => () => h(Unit.Default),
                    h => test.Event4 += h, h => test.Event4 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(4);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(4);
                isRaised.IsFalse();
            }

            // shortcut
            {
                var isRaised = false;
                var d = Observable.FromEvent(
                    h => test.Event4 += h, h => test.Event4 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(4);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(4);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<Action<int>, int>(
                    h => h,
                    h => test.Event5 += h, h => test.Event5 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(5);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(5);
                isRaised.IsFalse();
            }

            // shortcut
            {
                var isRaised = false;
                var d = Observable.FromEvent<int>(
                    h => test.Event5 += h, h => test.Event5 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(5);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(5);
                isRaised.IsFalse();
            }

            {
                var isRaised = false;
                var d = Observable.FromEvent<Action<int, string>, Tuple<int, string>>(
                    h => (x, y) => h(Tuple.Create(x, y)),
                    h => test.Event6 += h, h => test.Event6 -= h)
                    .Subscribe(x => isRaised = true);
                test.Fire(6);
                isRaised.IsTrue();
                isRaised = false;
                d.Dispose();
                test.Fire(6);
                isRaised.IsFalse();
            }
        }



    }


    public partial class ObservablePagingTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservablePagingTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Buffer";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Buffer();
                    r.Message.Value = "Buffer OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Buffer NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Buffer NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferTime";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferTime();
                    r.Message.Value = "BufferTime OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferTime NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferTime NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferTimeEmptyBuffer";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferTimeEmptyBuffer();
                    r.Message.Value = "BufferTimeEmptyBuffer OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferTimeEmptyBuffer NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferTimeEmptyBuffer NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferTimeComplete";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferTimeComplete();
                    r.Message.Value = "BufferTimeComplete OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferTimeComplete NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferTimeComplete NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferTimeEmptyComplete";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferTimeEmptyComplete();
                    r.Message.Value = "BufferTimeEmptyComplete OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferTimeEmptyComplete NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferTimeEmptyComplete NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferEmpty";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferEmpty();
                    r.Message.Value = "BufferEmpty OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferEmpty NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferEmpty NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferComplete2";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferComplete2();
                    r.Message.Value = "BufferComplete2 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferComplete2 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferComplete2 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Buffer3";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Buffer3();
                    r.Message.Value = "Buffer3 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Buffer3 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Buffer3 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferSkip";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferSkip();
                    r.Message.Value = "BufferSkip OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferSkip NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferSkip NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BufferTimeAndCount";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BufferTimeAndCount();
                    r.Message.Value = "BufferTimeAndCount OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BufferTimeAndCount NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BufferTimeAndCount NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "First";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.First();
                    r.Message.Value = "First OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "First NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "First NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "FirstOrDefault";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.FirstOrDefault();
                    r.Message.Value = "FirstOrDefault OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "FirstOrDefault NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "FirstOrDefault NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Last";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Last();
                    r.Message.Value = "Last OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Last NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Last NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "LastOrDefault";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.LastOrDefault();
                    r.Message.Value = "LastOrDefault OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "LastOrDefault NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "LastOrDefault NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Single";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Single();
                    r.Message.Value = "Single OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Single NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Single NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SingleOrDefault";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SingleOrDefault();
                    r.Message.Value = "SingleOrDefault OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SingleOrDefault NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SingleOrDefault NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TakeWhile";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TakeWhile();
                    r.Message.Value = "TakeWhile OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TakeWhile NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TakeWhile NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TakeUntil";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TakeUntil();
                    r.Message.Value = "TakeUntil OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TakeUntil NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TakeUntil NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Skip";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Skip();
                    r.Message.Value = "Skip OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Skip NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Skip NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SkipTime";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SkipTime();
                    r.Message.Value = "SkipTime OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SkipTime NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SkipTime NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SkipWhile";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SkipWhile();
                    r.Message.Value = "SkipWhile OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SkipWhile NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SkipWhile NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SkipWhileIndex";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SkipWhileIndex();
                    r.Message.Value = "SkipWhileIndex OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SkipWhileIndex NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SkipWhileIndex NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SkipUntil";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SkipUntil();
                    r.Message.Value = "SkipUntil OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SkipUntil NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SkipUntil NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Pairwise";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Pairwise();
                    r.Message.Value = "Pairwise OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Pairwise NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Pairwise NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Buffer()
        {
            var xs = Observable.Range(1, 10)
                .Buffer(3)
                .ToArray()
                .Wait();
            xs[0].IsCollection(1, 2, 3);
            xs[1].IsCollection(4, 5, 6);
            xs[2].IsCollection(7, 8, 9);
            xs[3].IsCollection(10);
        }




        [TestMethod]
        public void BufferTime()
        {
            var hoge = Observable.Return(1000).Delay(TimeSpan.FromSeconds(4));

            var xs = Observable.Range(1, 10)
                .Concat(hoge)
                .Buffer(TimeSpan.FromSeconds(3))
                .ToArray()
                .Wait();

            xs[0].IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            xs[1].IsCollection(1000);
        }




        [TestMethod]
        public void BufferTimeEmptyBuffer()
        {
            var xs = Observable.Return(10).Delay(TimeSpan.FromMilliseconds(3500))
                .Buffer(TimeSpan.FromSeconds(1))
                .ToArray()
                .Wait();

            xs.Length.Is(4);
            xs[0].Count.Is(0); // 1sec
            xs[1].Count.Is(0); // 2sec
            xs[2].Count.Is(0); // 3sec
            xs[3].Count.Is(1); // 4sec
        }




        [TestMethod]
        public void BufferTimeComplete()
        {
            // when complete, return empty array.
            var xs = Observable.Return(1).Delay(TimeSpan.FromSeconds(2))
                .Concat(Observable.Return(1).Delay(TimeSpan.FromSeconds(2)).Skip(1))
                .Buffer(TimeSpan.FromSeconds(3))
                .ToArray()
                .Wait();

            xs.Length.Is(2);
            xs[0].Count.Is(1);
            xs[1].Count.Is(0);
        }




        [TestMethod]
        public void BufferTimeEmptyComplete()
        {
            var xs = Observable.Empty<int>()
                .Buffer(TimeSpan.FromSeconds(1000))
                .ToArray()
                .Wait();

            xs.Length.Is(1);
        }




        [TestMethod]
        public void BufferEmpty()
        {
            var xs = Observable.Empty<int>()
                .Buffer(10)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }





        [TestMethod]
        public void BufferComplete2()
        {
            var xs = Observable.Range(1, 2)
                .Buffer(2)
                .ToArray()
                .Wait();

            xs.Length.Is(1);
            xs[0].IsCollection(1, 2);
        }




        [TestMethod]
        public void Buffer3()
        {
            var xs = Observable.Empty<int>()
                .Buffer(1, 2)
                .ToArray()
                .Wait();

            xs.Length.Is(0);
        }




        [TestMethod]
        public void BufferSkip()
        {
            // count > skip
            {
                var xs = Observable.Range(1, 10).Buffer(3, 1)
                    .ToArray()
                    .Wait();

                xs[0].IsCollection(1, 2, 3);
                xs[1].IsCollection(2, 3, 4);
                xs[2].IsCollection(3, 4, 5);
                xs[3].IsCollection(4, 5, 6);
                xs[4].IsCollection(5, 6, 7);
                xs[5].IsCollection(6, 7, 8);
                xs[6].IsCollection(7, 8, 9);
                xs[7].IsCollection(8, 9, 10);
                xs[8].IsCollection(9, 10);
                xs[9].IsCollection(10);
            }

            // count == skip
            {
                var xs = Observable.Range(1, 10).Buffer(3, 3)
                    .ToArray()
                    .Wait();

                xs[0].IsCollection(1, 2, 3);
                xs[1].IsCollection(4, 5, 6);
                xs[2].IsCollection(7, 8, 9);
                xs[3].IsCollection(10);
            }

            // count < skip
            {
                var xs = Observable.Range(1, 20).Buffer(3, 5)
                    .ToArray()
                    .Wait();

                xs[0].IsCollection(1, 2, 3);
                xs[1].IsCollection(6, 7, 8);
                xs[2].IsCollection(11, 12, 13);
                xs[3].IsCollection(16, 17, 18);
            }
        }




        [TestMethod]
        public void BufferTimeAndCount()
        {
            // time...count...complete
            {
                var xs = Observable.Return(1000)
                    .Concat(Observable.Return(99).Delay(TimeSpan.FromMilliseconds(1500)))
                    .Concat(Observable.Range(1, 7))
                    .Buffer(TimeSpan.FromSeconds(1), 5)
                    .ToArray()
                    .Wait();

                xs.Length.Is(3);
                xs[0].IsCollection(1000); // 1sec
                xs[1].IsCollection(99, 1, 2, 3, 4); // 1.5sec -> buffer
                xs[2].IsCollection(5, 6, 7); // next 1sec
            }
            // time...count...time
            {
                var xs = Observable.Return(1000)
                    .Concat(Observable.Return(99).Delay(TimeSpan.FromMilliseconds(1500)))
                    .Concat(Observable.Range(1, 7))
                    .Concat(Observable.Never<int>())
                    .Buffer(TimeSpan.FromSeconds(1), 5)
                    .Take(3)
                    .ToArray()
                    .Wait();

                xs.Length.Is(3);
                xs[0].IsCollection(1000); // 1sec
                xs[1].IsCollection(99, 1, 2, 3, 4); // 1.5sec -> buffer
                xs[2].IsCollection(5, 6, 7); // next 1sec
            }
            // time(before is canceled)
            {
                var start = DateTime.Now;
                var result = Observable.Return(10).Delay(TimeSpan.FromSeconds(2))
                    .Concat(Observable.Range(1, 2))
                    .Concat(Observable.Return(1000).Delay(TimeSpan.FromSeconds(2)))
                    .Concat(Observable.Never<int>())
                    .Buffer(TimeSpan.FromSeconds(3), 3)
                    .Take(2)
                    .Select(xs =>
                    {
                        var currentSpan = DateTime.Now - start;
                        return new { currentSpan, xs };
                    })
                    .ToArray()
                    .Wait();

                // after 2 seconds, buffer is flush by count
                result[0].xs.IsCollection(10, 1, 2);
                result[0].currentSpan.Is(x => TimeSpan.FromMilliseconds(1800) <= x && x <= TimeSpan.FromMilliseconds(2200));

                // after 3 seconds, buffer is flush by time
                result[1].xs.IsCollection(1000);
                result[1].currentSpan.Is(x => TimeSpan.FromMilliseconds(4800) <= x && x <= TimeSpan.FromMilliseconds(5200));
            }
        }




        [TestMethod]
        public void First()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.First().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First().Materialize().Subscribe(l.Add);

                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnError(new Exception());
                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);
                s.OnError(new Exception());
                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.First(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();
                l[0].Kind.Is(NotificationKind.OnError);
            }
        }




        [TestMethod]
        public void FirstOrDefault()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.FirstOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault().Materialize().Subscribe(l.Add);

                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.FirstOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }




        [TestMethod]
        public void Last()
        {
            var s = new Subject<int>();
            var l = new List<Notification<int>>();
            {
                s.Last().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnCompleted();

                l[0].Value.Is(30);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.Last(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(20);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(20);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Last(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(5);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }




        [TestMethod]
        public void LastOrDefault()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.LastOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnCompleted();

                l[0].Value.Is(30);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.LastOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(20);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(20);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.LastOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(5);
                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }




        [TestMethod]
        public void Single()
        {
            var s = new Subject<int>();
            var l = new List<Notification<int>>();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(10);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.Single(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Kind.Is(NotificationKind.OnError);
            }
        }




        [TestMethod]
        public void SingleOrDefault()
        {
            var s = new Subject<int>();

            var l = new List<Notification<int>>();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault().Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnNext(10);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l = new List<Notification<int>>();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(10);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(10);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(20);
                s.OnNext(30);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnNext(10);
                s.OnError(new Exception());

                l[0].Kind.Is(NotificationKind.OnError);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);

                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }

            s = new Subject<int>();
            l.Clear();
            {
                s.SingleOrDefault(x => x % 2 == 0).Materialize().Subscribe(l.Add);
                s.OnNext(9);
                s.OnCompleted();

                l[0].Value.Is(0);
                l[1].Kind.Is(NotificationKind.OnCompleted);
            }
        }




        [TestMethod]
        public void TakeWhile()
        {
            Observable.Range(1, 10)
                .TakeWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .IsCollection(1, 2, 3, 4, 5);
        }




        [TestMethod]
        public void TakeUntil()
        {
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.TakeUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnNext(1000);
                l.Count.Is(3);
                l[0].Value.Is(1);
                l[1].Value.Is(10);
                l[2].Kind.Is(NotificationKind.OnCompleted);
            }
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.TakeUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnCompleted();
                l.Count.Is(2);

                b.OnNext(1000);
                l.Count.Is(2);

                a.OnNext(100);
                l.Count.Is(3);
                l[0].Value.Is(1);
                l[1].Value.Is(10);
                l[2].Value.Is(100);
            }
        }






        [TestMethod]
        public void Skip()
        {
            Observable.Range(1, 10)
                .Skip(3)
                .ToArray()
                .Wait()
                .IsCollection(4, 5, 6, 7, 8, 9, 10);

            {
                var range = Observable.Range(1, 10);

                AssertEx.Throws<ArgumentOutOfRangeException>(() => range.Skip(-1));

                range.Skip(10).ToArray().Wait().Length.Is(0);

                range.Skip(3).ToArrayWait().IsCollection(4, 5, 6, 7, 8, 9, 10);
                range.Skip(3).Skip(2).ToArrayWait().IsCollection(6, 7, 8, 9, 10);
            }
        }




        [TestMethod]
        public void SkipTime()
        {
            {
                var now = DateTime.Now;
                var timer = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(10))
                    .Timestamp();

                var v = timer.Skip(TimeSpan.FromMilliseconds(300))
                    .First()
                    .Wait();

                (v.Timestamp - now).Is(x => (TimeSpan.FromMilliseconds(250) <= x && x <= TimeSpan.FromMilliseconds(350)));
            }

            {
                var now = DateTime.Now;
                var timer = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(10))
                    .Timestamp();

                var v = timer
                    .Skip(TimeSpan.FromMilliseconds(100))
                    .Skip(TimeSpan.FromMilliseconds(300))
                    .First()
                    .Wait();

                (v.Timestamp - now).Is(x => (TimeSpan.FromMilliseconds(250) <= x && x <= TimeSpan.FromMilliseconds(350)));
            }
        }




        [TestMethod]
        public void SkipWhile()
        {
            Observable.Range(1, 10)
                .SkipWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .IsCollection(6, 7, 8, 9, 10);
        }




        [TestMethod]
        public void SkipWhileIndex()
        {
            Observable.Range(1, 10)
                .SkipWhile((x, i) => i <= 5)
                .ToArray()
                .Wait()
                .IsCollection(7, 8, 9, 10);
        }





        [TestMethod]
        public void SkipUntil()
        {
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.SkipUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnNext(1000);

                l.Count.Is(0);

                b.OnNext(99999);
                b.HasObservers.IsFalse();

                a.OnNext(1);
                a.OnNext(10);

                l[0].Value.Is(1);
                l[1].Value.Is(10);
            }
            {
                var a = new Subject<int>();
                var b = new Subject<int>();

                var l = new List<Notification<int>>();

                a.SkipUntil(b).Materialize().Subscribe(l.Add);

                a.OnNext(1);
                a.OnNext(10);
                b.OnCompleted();
                l.Count.Is(0);

                a.OnNext(100);
                l.Count.Is(0);
            }
        }




        [TestMethod]
        public void Pairwise()
        {
            var xs = Observable.Range(1, 5).Pairwise((x, y) => x + ":" + y).ToArrayWait();
            xs.IsCollection("1:2", "2:3", "3:4", "4:5");
        }



    }


    public partial class ObservableTimeTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservableTimeTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TimerTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TimerTest();
                    r.Message.Value = "TimerTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TimerTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TimerTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DelayTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DelayTest();
                    r.Message.Value = "DelayTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DelayTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DelayTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SampleTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SampleTest();
                    r.Message.Value = "SampleTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SampleTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SampleTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TimeoutTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TimeoutTest();
                    r.Message.Value = "TimeoutTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TimeoutTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TimeoutTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TimeoutTestOffset";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TimeoutTestOffset();
                    r.Message.Value = "TimeoutTestOffset OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TimeoutTestOffset NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TimeoutTestOffset NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ThrottleTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ThrottleTest();
                    r.Message.Value = "ThrottleTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ThrottleTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ThrottleTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ThrottleFirstTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ThrottleFirstTest();
                    r.Message.Value = "ThrottleFirstTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ThrottleFirstTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ThrottleFirstTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Timestamp";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Timestamp();
                    r.Message.Value = "Timestamp OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Timestamp NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Timestamp NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "TimeInterval";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.TimeInterval();
                    r.Message.Value = "TimeInterval OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "TimeInterval NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "TimeInterval NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void TimerTest()
        {
            // periodic(Observable.Interval)
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Timestamp()
                    .ToArray()
                    .Wait();

                xs[0].Value.Is(0L);
                (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

                xs[1].Value.Is(1L);
                (now.AddMilliseconds(1800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(2200)).IsTrue();

                xs[2].Value.Is(2L);
                (now.AddMilliseconds(2800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(3200)).IsTrue();
            }

            // dueTime + periodic
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
                xs[1].Is(3);
                xs[2].Is(4);
            }

            // dueTime(DateTimeOffset)
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(now.AddSeconds(2), TimeSpan.FromSeconds(1))
                    .Take(3)
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
                xs[1].Is(3);
                xs[2].Is(4);
            }

            // onetime
            {
                var now = Scheduler.ThreadPool.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(2))
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
            }

            // non periodic scheduler
            {
                var now = Scheduler.CurrentThread.Now;
                var xs = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), Scheduler.CurrentThread)
                    .Take(3)
                    .Timestamp()
                    .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                    .ToArray()
                    .Wait();

                xs[0].Is(2);
                xs[1].Is(3);
                xs[2].Is(4);
            }
        }




        [TestMethod]
        public void DelayTest()
        {
            var now = Scheduler.ThreadPool.Now;

            var xs = Observable.Range(1, 3)
                .Delay(TimeSpan.FromSeconds(1))
                .Timestamp()
                .ToArray()
                .Wait();

            xs[0].Value.Is(1);
            (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[1].Value.Is(2);
            (now.AddMilliseconds(800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[2].Value.Is(3);
            (now.AddMilliseconds(800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(1200)).IsTrue();
        }




        [TestMethod]
        public void SampleTest()
        {
            // 2400, 4800, 7200, 9600
            var xs = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                .Take(10)
                .Sample(TimeSpan.FromMilliseconds(2400), Scheduler.CurrentThread)
                .Timestamp()
                .ToArray()
                .Wait();

            xs[0].Value.Is(2);
            xs[1].Value.Is(4);
            xs[2].Value.Is(7);
            xs[3].Value.Is(9);
        }




        [TestMethod]
        public void TimeoutTest()
        {
            var xs = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(9).Delay(TimeSpan.FromSeconds(3))
                )
                .Timestamp()
                .Timeout(TimeSpan.FromMilliseconds(1500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(2);
            xs[0].Value.Value.Is(1);
            xs[1].Exception.IsInstanceOf<TimeoutException>();
        }




        [TestMethod]
        public void TimeoutTestOffset()
        {
            var now = Scheduler.ThreadPool.Now;
            var xs = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(9).Delay(TimeSpan.FromSeconds(3))
                )
                .Timestamp()
                .Timeout(now.AddMilliseconds(3500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(3);
            xs[0].Value.Value.Is(1);
            xs[1].Value.Value.Is(5);
            xs[2].Exception.IsInstanceOf<TimeoutException>();
        }




        [TestMethod]
        public void ThrottleTest()
        {
            var xs = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(4).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(2)),
                    Observable.Return(6).Delay(TimeSpan.FromSeconds(3)), // over 2500
                    Observable.Return(7).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(8).Delay(TimeSpan.FromSeconds(1)) // with onCompleted
                )
                .Timestamp()
                .Throttle(TimeSpan.FromMilliseconds(2500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(3);
            xs[0].Value.Value.Is(5);
            xs[1].Value.Value.Is(8);
            xs[2].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void ThrottleFirstTest()
        {
            var xs = Observable.Concat(
                    Observable.Return(1),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(4).Delay(TimeSpan.FromSeconds(0.4)),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(0.2)), // over 2500
                    Observable.Return(6).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(7).Delay(TimeSpan.FromSeconds(1)),
                    Observable.Return(8).Delay(TimeSpan.FromSeconds(1)), // over 2500
                    Observable.Return(9) // withCompleted
                )
                .Timestamp()
                .ThrottleFirst(TimeSpan.FromMilliseconds(2500))
                .Materialize()
                .ToArray()
                .Wait();

            xs.Length.Is(4);
            xs[0].Value.Value.Is(1);
            xs[1].Value.Value.Is(5);
            xs[2].Value.Value.Is(8);
            xs[3].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void Timestamp()
        {
            var xs = Observable.Concat(
                Observable.Return(1),
                Observable.Return(2).Delay(TimeSpan.FromSeconds(1)),
                Observable.Return(3).Delay(TimeSpan.FromSeconds(1)),
                Observable.Return(4).Delay(TimeSpan.FromSeconds(0.4)),
                Observable.Return(5).Delay(TimeSpan.FromSeconds(0.2)) // over 2500
            ).Timestamp()
            .ToArray()
            .Wait();

            var now = DateTime.Now;

            xs[0].Value.Is(1); (now - xs[0].Timestamp).TotalSeconds.Is(x => 2.5 <= x && x <= 3.0);
            xs[1].Value.Is(2); (now - xs[1].Timestamp).TotalSeconds.Is(x => 1.4 <= x && x <= 1.8);
            xs[2].Value.Is(3); (now - xs[2].Timestamp).TotalSeconds.Is(x => 0.5 <= x && x <= 0.8);
            xs[3].Value.Is(4); (now - xs[3].Timestamp).TotalSeconds.Is(x => 0.18 <= x && x <= 0.3);
            xs[4].Value.Is(5); (now - xs[4].Timestamp).TotalSeconds.Is(x => 0 <= x && x <= 0.1);
        }




        [TestMethod]
        public void TimeInterval()
        {
            var xs = Observable.Concat(
                Observable.Return(1),
                Observable.Return(2).Delay(TimeSpan.FromSeconds(1)),
                Observable.Return(3).Delay(TimeSpan.FromSeconds(1)),
                Observable.Return(4).Delay(TimeSpan.FromSeconds(0.4)),
                Observable.Return(5).Delay(TimeSpan.FromSeconds(0.2)) // over 2500
            ).TimeInterval()
            .ToArray()
            .Wait();

            xs[0].Value.Is(1); xs[0].Interval.TotalSeconds.Is(x => 0.0 <= x && x <= 0.1);
            xs[1].Value.Is(2); xs[1].Interval.TotalSeconds.Is(x => 0.9 <= x && x <= 1.1);
            xs[2].Value.Is(3); xs[2].Interval.TotalSeconds.Is(x => 0.9 <= x && x <= 1.1);
            xs[3].Value.Is(4); xs[3].Interval.TotalSeconds.Is(x => 0.3 <= x && x <= 0.5);
            xs[4].Value.Is(5); xs[4].Interval.TotalSeconds.Is(x => 0.15 <= x && x <= 0.25);
        }



    }


    public partial class ObservableConcatTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservableConcatTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Concat";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Concat();
                    r.Message.Value = "Concat OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Concat NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Concat NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Zip";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Zip();
                    r.Message.Value = "Zip OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Zip NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Zip NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Zip2";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Zip2();
                    r.Message.Value = "Zip2 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Zip2 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Zip2 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ZipMulti";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ZipMulti();
                    r.Message.Value = "ZipMulti OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ZipMulti NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ZipMulti NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ZipMulti2";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ZipMulti2();
                    r.Message.Value = "ZipMulti2 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ZipMulti2 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ZipMulti2 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ZipNth";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ZipNth();
                    r.Message.Value = "ZipNth OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ZipNth NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ZipNth NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "WhenAll";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.WhenAll();
                    r.Message.Value = "WhenAll OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "WhenAll NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "WhenAll NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatest();
                    r.Message.Value = "CombineLatest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatest2";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatest2();
                    r.Message.Value = "CombineLatest2 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatest2 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatest2 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatest3";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatest3();
                    r.Message.Value = "CombineLatest3 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatest3 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatest3 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatest4";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatest4();
                    r.Message.Value = "CombineLatest4 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatest4 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatest4 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatestMulti";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatestMulti();
                    r.Message.Value = "CombineLatestMulti OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatestMulti NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatestMulti NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatestMulti2";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatestMulti2();
                    r.Message.Value = "CombineLatestMulti2 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatestMulti2 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatestMulti2 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatestMulti3";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatestMulti3();
                    r.Message.Value = "CombineLatestMulti3 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatestMulti3 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatestMulti3 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CombineLatestMulti4";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CombineLatestMulti4();
                    r.Message.Value = "CombineLatestMulti4 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CombineLatestMulti4 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CombineLatestMulti4 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "StartWith";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.StartWith();
                    r.Message.Value = "StartWith OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "StartWith NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "StartWith NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Merge";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Merge();
                    r.Message.Value = "Merge OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Merge NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Merge NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "MergeConcurrent";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.MergeConcurrent();
                    r.Message.Value = "MergeConcurrent OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "MergeConcurrent NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "MergeConcurrent NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Switch";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Switch();
                    r.Message.Value = "Switch OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Switch NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Switch NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Concat()
        {
            var a = Observable.Range(1, 5, Scheduler.ThreadPool);
            var b = Observable.Range(10, 3, Scheduler.ThreadPool);
            var c = Observable.Return(300, Scheduler.ThreadPool);

            Observable.Concat(a, b, c).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 10, 11, 12, 300);
        }




        [TestMethod]
        public void Zip()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.Zip(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            a.OnNext(1001);
            l.Count.Is(1);

            b.OnNext(5);
            l.Count.Is(2); // Completed!

            l[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void Zip2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.Zip(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            b.OnCompleted(); // Completed!

            l.Count.Is(2); // Completed!
            l[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void ZipMulti()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.Zip(a, b).Select(xs => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));


            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            a.OnNext(1001);
            l.Count.Is(1);

            b.OnNext(5);
            l.Count.Is(2); // Completed!

            l[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void ZipMulti2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.Zip(a, b).Select(xs => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(1); // OnNext

            b.OnCompleted(); // Completed!

            l.Count.Is(2); // Completed!
            l[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void ZipNth()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();
            var c = new Subject<int>();

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0, z = 0 })).ToList();
            Observable.Zip(a, b, c, (x, y, z) => new { x, y, z }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);
            c.OnNext(3000);

            l.Count.Is(1); // OnNext

            a.OnCompleted();

            b.OnNext(1001);
            l.Count.Is(1);

            b.OnCompleted();
            l.Count.Is(1);

            c.OnCompleted();
            l.Count.Is(2); // Completed!

            l[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void WhenAll()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.WhenAll(a, b).Select(xs => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            a.OnNext(1500);
            b.OnNext(2000);

            a.OnCompleted();

            l.Count.Is(0);

            a.OnNext(1001);
            l.Count.Is(0);

            b.OnNext(5);
            b.OnCompleted();

            l.Count.Is(2); // Completed!

            l[0].Value.x.Is(1500);
            l[0].Value.y.Is(5);
            l[1].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatest()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });

            b.OnNext(3000);
            l[1].Value.Is(new { x = 1000, y = 3000 });

            a.OnNext(5000);
            l[2].Value.Is(new { x = 5000, y = 3000 });

            a.OnCompleted();
            l.Count.Is(3);

            a.OnNext(1001);
            l.Count.Is(3);

            b.OnNext(5);
            l[3].Value.Is(new { x = 5000, y = 5 });
            b.OnNext(500);
            l[4].Value.Is(new { x = 5000, y = 500 });

            b.OnCompleted();
            l[5].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatest2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            b.OnNext(2000);
            a.OnCompleted();

            l.Count.Is(0);

            b.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatest3()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(2000);
            b.OnCompleted();

            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatest4()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.CombineLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));

            b.OnCompleted();
            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatestMulti()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });

            b.OnNext(3000);
            l[1].Value.Is(new { x = 1000, y = 3000 });

            a.OnNext(5000);
            l[2].Value.Is(new { x = 5000, y = 3000 });

            a.OnCompleted();
            l.Count.Is(3);

            a.OnNext(1001);
            l.Count.Is(3);

            b.OnNext(5);
            l[3].Value.Is(new { x = 5000, y = 5 });
            b.OnNext(500);
            l[4].Value.Is(new { x = 5000, y = 500 });

            b.OnCompleted();
            l[5].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatestMulti2()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            b.OnNext(2000);
            a.OnCompleted();

            l.Count.Is(0);

            b.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatestMulti3()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            a.OnNext(2000);
            b.OnCompleted();

            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void CombineLatestMulti4()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            a.OnNext(10);
            b.OnNext(20);

            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.CombineLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));

            b.OnCompleted();
            l.Count.Is(0);

            a.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void StartWith()
        {
            var seq = Observable.Range(1, 5);

            seq.StartWith(100).ToArray().Wait().IsCollection(100, 1, 2, 3, 4, 5);
            seq.StartWith(() => 100).ToArray().Wait().IsCollection(100, 1, 2, 3, 4, 5);
            seq.StartWith(100, 1000, 10000).ToArray().Wait().IsCollection(100, 1000, 10000, 1, 2, 3, 4, 5);
            seq.StartWith(Enumerable.Range(100, 3)).ToArray().Wait().IsCollection(100, 101, 102, 1, 2, 3, 4, 5);

            seq.StartWith(Scheduler.ThreadPool, 100).ToArray().Wait().IsCollection(100, 1, 2, 3, 4, 5);
            seq.StartWith(Scheduler.ThreadPool, 100, 1000, 10000).ToArray().Wait().IsCollection(100, 1000, 10000, 1, 2, 3, 4, 5);
            seq.StartWith(Scheduler.ThreadPool, Enumerable.Range(100, 3)).ToArray().Wait().IsCollection(100, 101, 102, 1, 2, 3, 4, 5);
        }




        [TestMethod]
        public void Merge()
        {
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            var s3 = new Subject<int>();

            var list = new List<int>();
            var complete = false;
            Observable.Merge(s1, s2, s3).Subscribe(list.Add, () => complete = true);

            s1.OnNext(10);
            s1.OnNext(20);
            s3.OnNext(100);
            s2.OnNext(50);

            list.IsCollection(10, 20, 100, 50);

            list.Clear();

            s2.OnCompleted();

            s3.OnNext(500);
            complete.IsFalse();
            s1.OnCompleted();
            s3.OnCompleted();
            complete.IsTrue();
        }





        [TestMethod]
        public void MergeConcurrent()
        {
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            var s3 = new Subject<int>();

            var list = new List<int>();
            var complete = false;
            Observable.Merge(new[] { s1, s2, s3 }, maxConcurrent: 2).Subscribe(list.Add, () => complete = true);

            s1.OnNext(10);
            s1.OnNext(20);
            s3.OnNext(100); // in queue
            s2.OnNext(50);

            list.IsCollection(10, 20, 50);

            list.Clear();

            s2.OnCompleted();

            s3.OnNext(500); // dequeued
            list.IsCollection(500);
            complete.IsFalse();

            s1.OnCompleted();
            s3.OnCompleted();
            complete.IsTrue();
        }




        [TestMethod]
        public void Switch()
        {
            var source = new Subject<IObservable<int>>();

            var list = new List<int>();
            source.Switch().Subscribe(list.Add);

            var s1 = new Subject<int>();
            source.OnNext(s1);

            s1.OnNext(100);
            s1.OnNext(2000);

            var s2 = new Subject<int>();
            s1.HasObservers.IsTrue();
            source.OnNext(s2);
            s1.OnNext(3000); // do nothing
            s1.HasObservers.IsFalse();

            s2.OnNext(5000);

            source.OnCompleted();

            s2.OnNext(900000);

            list.IsCollection(100, 2000, 5000, 900000);
            s2.OnCompleted();
        }



    }


    public partial class ObservableTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservableTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ToArray";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ToArray();
                    r.Message.Value = "ToArray OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ToArray NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ToArray NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ToArray_Dispose";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ToArray_Dispose();
                    r.Message.Value = "ToArray_Dispose OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ToArray_Dispose NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ToArray_Dispose NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Wait";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Wait();
                    r.Message.Value = "Wait OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Wait NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Wait NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DistinctUntilChanged";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DistinctUntilChanged();
                    r.Message.Value = "DistinctUntilChanged OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DistinctUntilChanged NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DistinctUntilChanged NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Distinct";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Distinct();
                    r.Message.Value = "Distinct OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Distinct NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Distinct NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SelectMany";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SelectMany();
                    r.Message.Value = "SelectMany OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SelectMany NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SelectMany NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Select";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Select();
                    r.Message.Value = "Select OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Select NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Select NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Where";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Where();
                    r.Message.Value = "Where OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Where NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Where NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Materialize";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Materialize();
                    r.Message.Value = "Materialize OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Materialize NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Materialize NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Dematerialize";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Dematerialize();
                    r.Message.Value = "Dematerialize OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Dematerialize NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Dematerialize NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "DefaultIfEmpty";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.DefaultIfEmpty();
                    r.Message.Value = "DefaultIfEmpty OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "DefaultIfEmpty NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "DefaultIfEmpty NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "IgnoreElements";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.IgnoreElements();
                    r.Message.Value = "IgnoreElements OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "IgnoreElements NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "IgnoreElements NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ForEachAsync";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ForEachAsync();
                    r.Message.Value = "ForEachAsync OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ForEachAsync NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ForEachAsync NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void ToArray()
        {
            var subject = new Subject<int>();

            int[] array = null;
            subject.ToArray().Subscribe(xs => array = xs);

            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);
            subject.OnCompleted();

            array.IsCollection(1, 10, 100);
        }




        [TestMethod]
        public void ToArray_Dispose()
        {
            var subject = new Subject<int>();

            int[] array = null;
            var disp = subject.ToArray().Subscribe(xs => array = xs);

            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);

            disp.Dispose();

            subject.OnCompleted();

            array.IsNull();
        }




        [TestMethod]
        public void Wait()
        {
            var subject = new Subject<int>();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                subject.OnNext(100);
                subject.OnCompleted();
            });

            subject.Wait().Is(100);
        }




        [TestMethod]
        public void DistinctUntilChanged()
        {
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.DistinctUntilChanged().ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5 }, subject.OnNext);
                subject.OnCompleted();

                array.IsCollection(1, 10, 1, 100, 5);
            }
            {

                string[] array = null;
                new[] { "hoge", "huga", null, null, "huga", "huga", "hoge" }
                    .ToObservable()
                    .DistinctUntilChanged()
                    .ToArray()
                    .Subscribe(xs => array = xs);

                array.IsCollection("hoge", "huga", null, "huga", "hoge");
            }
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.DistinctUntilChanged(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5 }, subject.OnNext);
                subject.OnCompleted();

                array.IsCollection(1, 10, 1, 100, 5);
            }
        }




        [TestMethod]
        public void Distinct()
        {
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.Distinct().ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }, subject.OnNext);
                subject.OnCompleted();

                array.IsCollection(1, 10, 100, 5, 70, 7);
            }
            {
                var subject = new Subject<int>();

                int[] array = null;
                subject.Distinct(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);

                Array.ForEach(new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }, subject.OnNext);
                subject.OnCompleted();

                array.IsCollection(1, 10, 100, 5, 70, 7);
            }
        }




        [TestMethod]
        public void SelectMany()
        {
            var a = new Subject<int>();
            var b = new Subject<int>();

            var list = new List<int>();
            a.SelectMany(_ => b).Subscribe(x => list.Add(x));

            a.OnNext(10);
            a.OnCompleted();
            b.OnNext(100);

            list.Count.Is(1);
            list[0].Is(100);
        }




        [TestMethod]
        public void Select()
        {
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select(x => x * x).Subscribe(x => list.Add(x));

                a.OnNext(100);
                a.OnNext(200);
                a.OnNext(300);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(10000, 40000, 90000);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select((x, i) => x * i).Subscribe(x => list.Add(x));

                a.OnNext(100);
                a.OnNext(200);
                a.OnNext(300);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(0, 200, 600);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select(x => x * x).Select(x => x * x).Subscribe(x => list.Add(x));

                a.OnNext(2);
                a.OnNext(4);
                a.OnNext(8);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(16, 256, 4096);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Select((x, i) => x * i).Select(x => x * 10).Subscribe(x => list.Add(x));

                a.OnNext(100);
                a.OnNext(200);
                a.OnNext(300);
                a.OnCompleted();

                list.Count.Is(3);
                list.IsCollection(0, 2000, 6000);
            }
        }




        [TestMethod]
        public void Where()
        {
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where(x => x % 3 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3);
                a.OnNext(5);
                a.OnNext(7);
                a.OnNext(9);
                a.OnNext(300);
                a.OnCompleted();

                list.IsCollection(3, 9, 300);
            }

            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where((x, i) => (x + i) % 3 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3); // 3 + 0
                a.OnNext(5); // 5 + 1
                a.OnNext(7); // 7 + 2
                a.OnNext(9); // 9 + 3
                a.OnNext(300); // 300 + 4
                a.OnCompleted();

                list.IsCollection(3, 5, 7, 9);
            }
            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where(x => x % 3 == 0).Where(x => x % 5 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3);
                a.OnNext(5);
                a.OnNext(7);
                a.OnNext(9);
                a.OnNext(300);
                a.OnCompleted();

                list.IsCollection(300);
            }

            {
                var a = new Subject<int>();

                var list = new List<int>();
                a.Where((x, i) => (x + i) % 3 == 0).Where(x => x % 5 == 0).Subscribe(x => list.Add(x));

                a.OnNext(3); // 3 + 0
                a.OnNext(5); // 5 + 1
                a.OnNext(7); // 7 + 2
                a.OnNext(9); // 9 + 3
                a.OnNext(300); // 300 + 4
                a.OnCompleted();

                list.IsCollection(5);
            }
        }




        [TestMethod]
        public void Materialize()
        {
            var m = Observable.Empty<int>().Materialize().ToArrayWait();
            m[0].Kind.Is(NotificationKind.OnCompleted);

            m = Observable.Range(1, 3).Materialize().ToArrayWait();
            m[0].Value.Is(1);
            m[1].Value.Is(2);
            m[2].Value.Is(3);
            m[3].Kind.Is(NotificationKind.OnCompleted);

            m = Observable.Range(1, 3).Concat(Observable.Throw<int>(new Exception())).Materialize().ToArrayWait();
            m[0].Value.Is(1);
            m[1].Value.Is(2);
            m[2].Value.Is(3);
            m[3].Kind.Is(NotificationKind.OnError);
        }




        [TestMethod]
        public void Dematerialize()
        {
            var m = Observable.Empty<int>().Materialize().Dematerialize().ToArrayWait();
            m.IsCollection();

            m = Observable.Range(1, 3).Materialize().Dematerialize().ToArrayWait();
            m.IsCollection(1, 2, 3);

            var l = new List<int>();
            Observable.Range(1, 3).Concat(Observable.Throw<int>(new Exception())).Materialize()
                .Dematerialize()
                .Subscribe(x => l.Add(x), ex => l.Add(1000), () => l.Add(10000));
            l.IsCollection(1, 2, 3, 1000);
        }




        [TestMethod]
        public void DefaultIfEmpty()
        {
            Observable.Range(1, 3).DefaultIfEmpty(-1).ToArrayWait().IsCollection(1, 2, 3);
            Observable.Empty<int>().DefaultIfEmpty(-1).ToArrayWait().IsCollection(-1);
        }




        [TestMethod]
        public void IgnoreElements()
        {
            var xs = Observable.Range(1, 10).IgnoreElements().Materialize().ToArrayWait();
            xs[0].Kind.Is(NotificationKind.OnCompleted);
        }




        [TestMethod]
        public void ForEachAsync()
        {
            {
                var list = new List<int>();
                var xs = Observable.Range(1, 10).ForEachAsync(x => list.Add(x)).ToArray().Wait();
                list.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
                xs.Length.Is(1);
                xs[0].Is(Unit.Default);
            }

            {
                var list = new List<int>();
                var listI = new List<int>();
                var xs = Observable.Range(1, 10).ForEachAsync((x, i) =>
                {
                    list.Add(x);
                    listI.Add(i);
                }).ToArray().Wait();
                list.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
                listI.IsCollection(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                xs.Length.Is(1);
                xs[0].Is(Unit.Default);
            }
        }



    }


    public partial class SchedulerTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new SchedulerTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CurrentThread";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CurrentThread();
                    r.Message.Value = "CurrentThread OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CurrentThread NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CurrentThread NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CurrentThread2";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CurrentThread2();
                    r.Message.Value = "CurrentThread2 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CurrentThread2 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CurrentThread2 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "CurrentThread3";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.CurrentThread3();
                    r.Message.Value = "CurrentThread3 OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "CurrentThread3 NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "CurrentThread3 NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Immediate";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Immediate();
                    r.Message.Value = "Immediate OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Immediate NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Immediate NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }




        [TestMethod]
        public void CurrentThread()
        {
            var hoge = ScheduleTasks(Scheduler.CurrentThread);
            hoge.IsCollection("outer start.", "outer end.", "--innerAction start.", "--innerAction end.", "----leafAction.");
        }



        [TestMethod]
        public void CurrentThread2()
        {
            var scheduler = Scheduler.CurrentThread;

            var list = new List<string>();
            scheduler.Schedule(() =>
            {
                list.Add("one");

                scheduler.Schedule(TimeSpan.FromSeconds(3), () =>
                {
                    list.Add("after 3");
                });

                scheduler.Schedule(TimeSpan.FromSeconds(1), () =>
                {
                    list.Add("after 1");
                });
            });

            list.IsCollection("one", "after 1", "after 3");
        }




        [TestMethod]
        public void CurrentThread3()
        {
            var scheduler = Scheduler.CurrentThread;

            var list = new List<string>();
            scheduler.Schedule(() =>
            {
                list.Add("one");

                var cancel = scheduler.Schedule(TimeSpan.FromSeconds(3), () =>
                {
                    list.Add("after 3");
                });

                scheduler.Schedule(TimeSpan.FromSeconds(1), () =>
                {
                    list.Add("after 1");
                    cancel.Dispose();
                });
            });

            list.IsCollection("one", "after 1");
        }




        [TestMethod]
        public void Immediate()
        {
            var hoge = ScheduleTasks(Scheduler.Immediate);
            hoge.IsCollection("outer start.", "--innerAction start.", "----leafAction.", "--innerAction end.", "outer end.");
        }



    }


    public partial class ReactriveDictionaryTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ReactriveDictionaryTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "RxDictObserve";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.RxDictObserve();
                    r.Message.Value = "RxDictObserve OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "RxDictObserve NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "RxDictObserve NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void RxDictObserve()
        {
            var dict = new ReactiveDictionary<string, int>();

            var count = 0;
            DictionaryAddEvent<string, int> addE = null;
            DictionaryRemoveEvent<string, int> removeE = null;
            DictionaryReplaceEvent<string, int> replaceE = null;
            var resetCount = 0;

            dict.ObserveCountChanged().Subscribe(x => count = x);
            dict.ObserveAdd().Subscribe(x => addE = x);
            dict.ObserveRemove().Subscribe(x => removeE = x);
            dict.ObserveReplace().Subscribe(x => replaceE = x);
            dict.ObserveReset().Subscribe(x => resetCount += 1);

            dict.Add("a", 100);
            count.Is(1);
            addE.Key.Is("a"); addE.Value.Is(100);

            dict.Add("b", 200);
            count.Is(2);
            addE.Key.Is("b"); addE.Value.Is(200);

            count = -1;
            dict["a"] = 300;
            count.Is(-1); // not fired
            addE.Key.Is("b"); // not fired
            replaceE.Key.Is("a"); replaceE.OldValue.Is(100); replaceE.NewValue.Is(300);

            dict["c"] = 400;
            count.Is(3);
            replaceE.Key.Is("a"); // not fired
            addE.Key.Is("c"); addE.Value.Is(400);

            dict.Remove("b");
            count.Is(2);
            removeE.Key.Is("b"); removeE.Value.Is(200);

            count = -1;
            dict.Remove("z");
            count.Is(-1); // not fired
            removeE.Key.Is("b"); // not fired

            dict.Clear();
            count.Is(0);
            resetCount.Is(1);

            count = -1;
            dict.Clear();
            resetCount.Is(2);
            count.Is(-1); // not fired
        }



    }


    public partial class SubjectTests
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new SubjectTests();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Subject";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Subject();
                    r.Message.Value = "Subject OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Subject NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Subject NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "SubjectSubscribeTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.SubjectSubscribeTest();
                    r.Message.Value = "SubjectSubscribeTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "SubjectSubscribeTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "SubjectSubscribeTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "AsyncSubjectTest";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.AsyncSubjectTest();
                    r.Message.Value = "AsyncSubjectTest OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "AsyncSubjectTest NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "AsyncSubjectTest NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "BehaviorSubject";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.BehaviorSubject();
                    r.Message.Value = "BehaviorSubject OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "BehaviorSubject NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "BehaviorSubject NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ReplaySubject";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ReplaySubject();
                    r.Message.Value = "ReplaySubject OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ReplaySubject NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ReplaySubject NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ReplaySubjectWindowReplay";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ReplaySubjectWindowReplay();
                    r.Message.Value = "ReplaySubjectWindowReplay OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ReplaySubjectWindowReplay NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ReplaySubjectWindowReplay NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Subject()
        {
            // OnCompletedPattern
            {
                var subject = new Subject<int>();

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.IsCollection(1, 10, 100, 1000);

                subject.OnCompleted();
                onCompletedCallCount.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(4);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);

                // ++subscription
                onNext.Clear();
                onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Count.Is(0);
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);
            }

            // OnErrorPattern
            {
                var subject = new Subject<int>();

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.IsCollection(1, 10, 100, 1000);

                subject.OnError(new Exception());
                exception.Count.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(4);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);

                // ++subscription
                onNext.Clear();
                exception.Clear();
                onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Count.Is(0);
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);
            }
        }




        [TestMethod]
        public void SubjectSubscribeTest()
        {
            var subject = new Subject<int>();
            var listA = new List<int>();
            var listB = new List<int>();
            var listC = new List<int>();
            subject.HasObservers.IsFalse();

            var listASubscription = subject.Subscribe(x => listA.Add(x));
            subject.HasObservers.IsTrue();
            subject.OnNext(1);
            listA[0].Is(1);

            var listBSubscription = subject.Subscribe(x => listB.Add(x));
            subject.HasObservers.IsTrue();
            subject.OnNext(2);
            listA[1].Is(2);
            listB[0].Is(2);

            var listCSubscription = subject.Subscribe(x => listC.Add(x));
            subject.HasObservers.IsTrue();
            subject.OnNext(3);
            listA[2].Is(3);
            listB[1].Is(3);
            listC[0].Is(3);

            listASubscription.Dispose();
            subject.HasObservers.IsTrue();
            subject.OnNext(4);
            listA.Count.Is(3);
            listB[2].Is(4);
            listC[1].Is(4);

            listCSubscription.Dispose();
            subject.HasObservers.IsTrue();
            subject.OnNext(5);
            listC.Count.Is(2);
            listB[3].Is(5);

            listBSubscription.Dispose();
            subject.HasObservers.IsFalse();
            subject.OnNext(6);
            listB.Count.Is(4);

            var listD = new List<int>();
            var listE = new List<int>();

            subject.Subscribe(x => listD.Add(x));
            subject.HasObservers.IsTrue();
            subject.OnNext(1);
            listD[0].Is(1);

            subject.Subscribe(x => listE.Add(x));
            subject.HasObservers.IsTrue();
            subject.OnNext(2);
            listD[1].Is(2);
            listE[0].Is(2);

            subject.Dispose();

            AssertEx.Throws<ObjectDisposedException>(() => subject.OnNext(0));
            AssertEx.Throws<ObjectDisposedException>(() => subject.OnError(new Exception()));
            AssertEx.Throws<ObjectDisposedException>(() => subject.OnCompleted());
        }




        [TestMethod]
        public void AsyncSubjectTest()
        {
            // OnCompletedPattern
            {
                var subject = new AsyncSubject<int>();

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.Count.Is(0);

                subject.OnCompleted();
                onNext.IsCollection(1000);
                onCompletedCallCount.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(1);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);

                // ++subscription
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.IsCollection(1000, 1000);
                exception.Count.Is(0);
                onCompletedCallCount.Is(2);
            }

            // OnErrorPattern
            {
                var subject = new AsyncSubject<int>();

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.Count.Is(0);

                subject.OnError(new Exception());
                exception.Count.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(0);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);

                // ++subscription
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Count.Is(0);
                exception.Count.Is(2);
                onCompletedCallCount.Is(0);
            }
        }




        [TestMethod]
        public void BehaviorSubject()
        {
            // OnCompletedPattern
            {
                var subject = new BehaviorSubject<int>(3333);

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                var _ = subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                onNext.IsCollection(3333);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);

                onNext.IsCollection(3333, 1, 10, 100, 1000);

                // re subscription
                onNext.Clear();
                _.Dispose();
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.IsCollection(1000);

                subject.OnCompleted();
                onCompletedCallCount.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(1);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);

                // ++subscription
                onNext.Clear();
                onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Count.Is(0);
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);
            }

            // OnErrorPattern
            {
                var subject = new BehaviorSubject<int>(3333);

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.IsCollection(3333, 1, 10, 100, 1000);

                subject.OnError(new Exception());
                exception.Count.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(5);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);

                // ++subscription
                onNext.Clear();
                exception.Clear();
                onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Count.Is(0);
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);
            }
        }




        [TestMethod]
        public void ReplaySubject()
        {
            // OnCompletedPattern
            {
                var subject = new ReplaySubject<int>();

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                var _ = subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.IsCollection(1, 10, 100, 1000);

                // replay subscription
                onNext.Clear();
                _.Dispose();
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.IsCollection(1, 10, 100, 1000);

                subject.OnCompleted();
                onCompletedCallCount.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(4);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);

                // ++subscription
                onNext.Clear();
                onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.IsCollection(1, 10, 100, 1000);
                exception.Count.Is(0);
                onCompletedCallCount.Is(1);
            }

            // OnErrorPattern
            {
                var subject = new ReplaySubject<int>();

                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                onNext.IsCollection(1, 10, 100, 1000);

                subject.OnError(new Exception());
                exception.Count.Is(1);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                onNext.Count.Is(4);

                subject.OnCompleted();
                subject.OnError(new Exception());
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);

                // ++subscription
                onNext.Clear();
                exception.Clear();
                onCompletedCallCount = 0;
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.IsCollection(1, 10, 100, 1000);
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);
            }
        }




        [TestMethod]
        public void ReplaySubjectWindowReplay()
        {
            var subject = new ReplaySubject<int>(window: TimeSpan.FromMilliseconds(700));

            var onNext = new List<int>();
            var exception = new List<Exception>();
            int onCompletedCallCount = 0;
            var _ = subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

            subject.OnNext(1); // 0
            Thread.Sleep(TimeSpan.FromMilliseconds(300));

            subject.OnNext(10); // 300
            Thread.Sleep(TimeSpan.FromMilliseconds(300));

            subject.OnNext(100); // 600
            Thread.Sleep(TimeSpan.FromMilliseconds(300));

            _.Dispose();
            onNext.Clear();
            subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
            onNext.IsCollection(10, 100);

            subject.OnNext(1000); // 900
            Thread.Sleep(TimeSpan.FromMilliseconds(300));

            _.Dispose();
            onNext.Clear();
            subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
            onNext.IsCollection(100, 1000);

            subject.OnNext(10000); // 1200
            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            subject.OnNext(2); // 1500
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            subject.OnNext(20); // 1800
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            _.Dispose();
            onNext.Clear();
            subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
            onNext.IsCollection(10000, 2, 20);
        }



    }


    public partial class ObservableGeneratorTest
    {
        public static IEnumerator Run(Result resultPrefab, GameObject resultVertical)
        {
            var test = new ObservableGeneratorTest();
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Empty";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Empty();
                    r.Message.Value = "Empty OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Empty NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Empty NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Never";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Never();
                    r.Message.Value = "Never OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Never NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Never NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Return";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Return();
                    r.Message.Value = "Return OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Return NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Return NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Range";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Range();
                    r.Message.Value = "Range OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Range NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Range NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Repeat";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Repeat();
                    r.Message.Value = "Repeat OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Repeat NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Repeat NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "RepeatStatic";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.RepeatStatic();
                    r.Message.Value = "RepeatStatic OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "RepeatStatic NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "RepeatStatic NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "ToObservable";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.ToObservable();
                    r.Message.Value = "ToObservable OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "ToObservable NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "ToObservable NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
            {
                var r = GameObject.Instantiate(resultPrefab);
                r.ForceInitialize();
                r.gameObject.transform.SetParent(resultVertical.transform);
                r.Message.Value = "Throw";
                r.Color.Value = UnityEngine.Color.gray;
                yield return null;
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    test.Throw();
                    r.Message.Value = "Throw OK " + sw.Elapsed.TotalMilliseconds + "ms";
                    r.Color.Value = UnityEngine.Color.green;
                }
                catch (AssertFailedException ex)
                {
                    r.Message.Value = "Throw NG\r\n" + ex.Message;
                    r.Color.Value = UnityEngine.Color.red;
                }
                catch (Exception ex)
                {
                    r.Message.Value = "Throw NG\r\n" + ex.ToString();
                    r.Color.Value = UnityEngine.Color.red;
                }
            }
            yield return null;
        }



        [TestMethod]
        public void Empty()
        {
            var material = Observable.Empty<Unit>().Materialize().ToArray().Wait();
            material.IsCollection(Notification.CreateOnCompleted<Unit>());
        }




        [TestMethod]
        public void Never()
        {
            AssertEx.Catch<TimeoutException>(() =>
                Observable.Never<Unit>().Materialize().ToArray().Wait(TimeSpan.FromMilliseconds(10)));
        }




        [TestMethod]
        public void Return()
        {
            Observable.Return(100).Materialize().ToArray().Wait().IsCollection(Notification.CreateOnNext(100), Notification.CreateOnCompleted<int>());
        }




        [TestMethod]
        public void Range()
        {
            Observable.Range(1, 5).ToArray().Wait().IsCollection(1, 2, 3, 4, 5);
        }




        [TestMethod]
        public void Repeat()
        {
            var xs = Observable.Range(1, 3, Scheduler.CurrentThread)
                .Concat(Observable.Return(100))
                .Repeat()
                .Take(10)
                .ToArray()
                .Wait();
            xs.IsCollection(1, 2, 3, 100, 1, 2, 3, 100, 1, 2);
            Observable.Repeat(100).Take(5).ToArray().Wait().IsCollection(100, 100, 100, 100, 100);
        }




        [TestMethod]
        public void RepeatStatic()
        {
            var xss = Observable.Repeat(5, 3).ToArray().Wait();
            xss.IsCollection(5, 5, 5);
        }




        [TestMethod]
        public void ToObservable()
        {
            {
                var msgs = new List<string>();
                new[] { 1, 10, 100, 1000, 10000, 20000 }.ToObservable(Scheduler.CurrentThread)
                    .Do(i => msgs.Add("DO:" + i))
                    .Scan((x, y) =>
                    {
                        if (y == 100) throw new Exception("exception");
                        msgs.Add("x:" + x + " y:" + y);
                        return x + y;
                    })
                    .Subscribe(x => msgs.Add(x.ToString()), e => msgs.Add(e.Message), () => msgs.Add("comp"));

                msgs.IsCollection("DO:1", "1", "DO:10", "x:1 y:10", "11", "DO:100", "exception");
            }

            {
                var msgs = new List<string>();
                new[] { 1, 10, 100, 1000, 10000, 20000 }.ToObservable(Scheduler.Immediate)
                    .Do(i => msgs.Add("DO:" + i))
                    .Scan((x, y) =>
                    {
                        if (y == 100) throw new Exception("exception");
                        msgs.Add("x:" + x + " y:" + y);
                        return x + y;
                    })
                    .Subscribe(x => msgs.Add(x.ToString()), e => msgs.Add(e.Message), () => msgs.Add("comp"));

                msgs.IsCollection("DO:1", "1", "DO:10", "x:1 y:10", "11", "DO:100", "exception",
                    "DO:1000", "x:11 y:1000",
                    "DO:10000", "x:1011 y:10000",
                    "DO:20000", "x:11011 y:20000"
                    );
            }
        }




        [TestMethod]
        public void Throw()
        {
            var ex = new Exception();
            Observable.Throw<string>(ex).Materialize().ToArray().Wait().IsCollection(Notification.CreateOnError<string>(ex));
        }



    }


}

