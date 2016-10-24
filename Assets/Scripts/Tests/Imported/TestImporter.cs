#pragma warning disable 168
#if !UNITY_METRO && !UNITY_4_5
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Tests;
using UniRx;
using RuntimeUnitTestToolkit;

namespace RuntimeUnitTestToolkit
{

    public partial class AggregateTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new AggregateTest();
            UnitTest.AddTest(test.Aggregate);
            UnitTest.AddTest(test.Scan);
        }



        [TestMethod]
        public void Aggregate()
        {
            SetScehdulerForImport();
            AssertEx.Throws<InvalidOperationException>(() => Observable.Empty<int>().Aggregate((x, y) => x + y).Wait());
            Observable.Range(1, 5).Aggregate((x, y) => x + y).Wait().Is(15);
            Observable.Empty<int>().Aggregate(100, (x, y) => x + y).Wait().Is(100);
            Observable.Range(1, 5).Aggregate(100, (x, y) => x + y).Wait().Is(115);
            Observable.Empty<int>().Aggregate(100, (x, y) => x + y, x => x + x).Wait().Is(200);
            Observable.Range(1, 5).Aggregate(100, (x, y) => x + y, x => x + x).Wait().Is(230);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Scan()
        {
            SetScehdulerForImport();
            var range = Observable.Range(1, 5);
            range.Scan((x, y) => x + y).ToArrayWait().IsCollection(1, 3, 6, 10, 15);
            range.Scan(100, (x, y) => x + y).ToArrayWait().IsCollection(101, 103, 106, 110, 115);
            Observable.Empty<int>().Scan((x, y) => x + y).ToArrayWait().IsCollection();
            Observable.Empty<int>().Scan(100, (x, y) => x + y).ToArrayWait().IsCollection();
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ContinueWithTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ContinueWithTest();
            UnitTest.AddTest(test.ContinueWith);
            UnitTest.AddTest(test.ContinueWith2);
        }



        [TestMethod]
        public void ContinueWith()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            var record = subject.ContinueWith(x => Observable.Return(x)).Record();
            subject.OnNext(10);
            record.Values.Count.Is(0);
            subject.OnNext(100);
            record.Values.Count.Is(0);
            subject.OnCompleted();
            record.Values[0].Is(100);
            record.Notifications.Last().Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ContinueWith2()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            var record = subject.ContinueWith(x => Observable.Return(x).Delay(TimeSpan.FromMilliseconds(100))).Record();
            subject.OnNext(10);
            record.Values.Count.Is(0);
            subject.OnNext(100);
            record.Values.Count.Is(0);
            subject.OnCompleted();
            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            record.Values[0].Is(100);
            record.Notifications.Last().Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ConversionTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ConversionTest();
            UnitTest.AddTest(test.AsObservable);
            UnitTest.AddTest(test.AsSingleUnitObservable);
            UnitTest.AddTest(test.AsUnitObservable);
            UnitTest.AddTest(test.Cast);
            UnitTest.AddTest(test.OfType);
            UnitTest.AddTest(test.ToObservable);
        }



        [TestMethod]
        public void AsObservable()
        {
            SetScehdulerForImport();
            Observable.Range(1, 10).AsObservable().ToArrayWait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void AsSingleUnitObservable()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            var done = false;
            subject.AsSingleUnitObservable().Subscribe(_ => done = true);
            subject.OnNext(1);
            done.IsFalse();
            subject.OnNext(100);
            done.IsFalse();
            subject.OnCompleted();
            done.IsTrue();
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void AsUnitObservable()
        {
            SetScehdulerForImport();
            Observable.Range(1, 3).AsUnitObservable().ToArrayWait().IsCollection(Unit.Default, Unit.Default, Unit.Default);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Cast()
        {
            SetScehdulerForImport();
            Observable.Range(1, 3).Cast<int, object>().ToArrayWait().IsCollection(1, 2, 3);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void OfType()
        {
            SetScehdulerForImport();
            var subject = new Subject<object>();
            var list = new List<int>();
            subject.OfType(default(int)).Subscribe(x => list.Add(x));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("hogehoge");
            subject.OnNext(3);
            list.IsCollection(1, 2, 3);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToObservable()
        {
            SetScehdulerForImport();
            Enumerable.Range(1, 3).ToObservable(Scheduler.CurrentThread).ToArrayWait().IsCollection(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.ThreadPool).ToArrayWait().IsCollection(1, 2, 3);
            Enumerable.Range(1, 3).ToObservable(Scheduler.Immediate).ToArrayWait().IsCollection(1, 2, 3);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class DisposableTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new DisposableTest();
            UnitTest.AddTest(test.Boolean);
            UnitTest.AddTest(test.MultipleAssignment);
            UnitTest.AddTest(test.Serial);
            UnitTest.AddTest(test.SingleAssignment);
        }



        [TestMethod]
        public void Boolean()
        {
            SetScehdulerForImport();
            var bd = new BooleanDisposable();
            bd.IsDisposed.IsFalse();
            bd.Dispose();
            bd.IsDisposed.IsTrue();
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void MultipleAssignment()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Serial()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SingleAssignment()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class DoTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new DoTest();
            UnitTest.AddTest(test.Do);
            UnitTest.AddTest(test.DoObserver);
            UnitTest.AddTest(test.DoOnCancel);
            UnitTest.AddTest(test.DoOnCompleted);
            UnitTest.AddTest(test.DoOnError);
            UnitTest.AddTest(test.DoOnSubscribe);
            UnitTest.AddTest(test.DoOnTerminate);
        }



        [TestMethod]
        public void Do()
        {
            SetScehdulerForImport();
            var list = new List<int>();
            Observable.Empty<int>().Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).DefaultIfEmpty().Wait();
            list.IsCollection(1000);
            list.Clear();
            Observable.Range(1, 5).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Wait();
            list.IsCollection(1, 2, 3, 4, 5, 1000);
            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(1, 2, 3, 4, 5, 100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DoObserver()
        {
            SetScehdulerForImport();
            var observer = new ListObserver();
            Observable.Empty<int>().Do(observer).DefaultIfEmpty().Wait();
            observer.list.IsCollection(1000);
            observer = new ListObserver();
            Observable.Range(1, 5).Do(observer).Wait();
            observer.list.IsCollection(1, 2, 3, 4, 5, 1000);
            observer = new ListObserver();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(observer).Subscribe(_ => { }, _ => { }, () => { });
            observer.list.IsCollection(1, 2, 3, 4, 5, 100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DoOnCancel()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DoOnCompleted()
        {
            SetScehdulerForImport();
            var list = new List<int>();
            Observable.Empty<int>().DoOnCompleted(() => list.Add(1000)).DefaultIfEmpty().Wait();
            list.IsCollection(1000);
            list.Clear();
            Observable.Range(1, 5).DoOnCompleted(() => list.Add(1000)).Wait();
            list.IsCollection(1000);
            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnCompleted(() => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection();
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DoOnError()
        {
            SetScehdulerForImport();
            var list = new List<int>();
            Observable.Empty<int>().DoOnError(_ => list.Add(100)).DefaultIfEmpty().Wait();
            list.IsCollection();
            list.Clear();
            Observable.Range(1, 5).DoOnError(_ => list.Add(100)).Wait();
            list.IsCollection();
            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnError(_ => list.Add(100)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DoOnSubscribe()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DoOnTerminate()
        {
            SetScehdulerForImport();
            var list = new List<int>();
            Observable.Empty<int>().DoOnTerminate(() => list.Add(1000)).DefaultIfEmpty().Wait();
            list.IsCollection(1000);
            list.Clear();
            Observable.Range(1, 5).DoOnTerminate(() => list.Add(1000)).Wait();
            list.IsCollection(1000);
            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnTerminate(() => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.IsCollection(1000);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class DurabilityTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new DurabilityTest();
            UnitTest.AddTest(test.Durability);
            UnitTest.AddTest(test.FromEventPattern);
            UnitTest.AddTest(test.FromEventUnity);
            UnitTest.AddTest(test.FromEventUnityLike);
        }



        [TestMethod]
        public void Durability()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FromEventPattern()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FromEventUnity()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FromEventUnityLike()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ErrorHandlingTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ErrorHandlingTest();
            UnitTest.AddTest(test.Catch);
            UnitTest.AddTest(test.CatchEnumerable);
            UnitTest.AddTest(test.Finally);
        }



        [TestMethod]
        public void Catch()
        {
            SetScehdulerForImport();
            var xs = Observable.Return(2, Scheduler.ThreadPool).Concat(Observable.Throw<int>(new InvalidOperationException()))
                .Catch((InvalidOperationException ex) =>
                {
                    return Observable.Range(1, 3);
                })
                .ToArrayWait();
            xs.IsCollection(2, 1, 2, 3);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CatchEnumerable()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Finally()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class MicroCoroutineTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new MicroCoroutineTest();
            UnitTest.AddTest(test.EnumerationCycle);
            UnitTest.AddTest(test.EnumerationCycleBlank);
            UnitTest.AddTest(test.EnumerationCycleFull);
            UnitTest.AddTest(test.EnumerationCycleRandom);
        }



        [TestMethod]
        public void EnumerationCycle()
        {
            SetScehdulerForImport();
            var coroutines = new[]
            {
                new DecrementEnumerator(1),
                new DecrementEnumerator(2),
                new DecrementEnumerator(3),
                new DecrementEnumerator(4),
                new DecrementEnumerator(5),
            };
            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            mc.Run();
            GetTailDynamic(mc).Is(5);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(0, 1, 2, 3, 4);
            mc.Run();
            GetTailDynamic(mc).Is(4);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, 0, 1, 2, 3);
            mc.Run();
            GetTailDynamic(mc).Is(3);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, 0, 1, 2);
            mc.Run();
            GetTailDynamic(mc).Is(2);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, 0, 1);
            mc.Run();
            GetTailDynamic(mc).Is(1);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, 0);
            mc.Run();
            GetTailDynamic(mc).Is(0);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, -1);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void EnumerationCycleBlank()
        {
            SetScehdulerForImport();
            var coroutines = new[]
            {
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
            };
            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            GetTailDynamic(mc).Is(5);
            mc.Run();
            GetTailDynamic(mc).Is(0);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, -1);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void EnumerationCycleFull()
        {
            SetScehdulerForImport();
            var coroutines = new[]
            {
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
            };
            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            GetTailDynamic(mc).Is(16);
            mc.Run();
            GetTailDynamic(mc).Is(16);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            mc.Run();
            GetTailDynamic(mc).Is(0);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void EnumerationCycleRandom()
        {
            SetScehdulerForImport();
            // pattern for shuffle
            var rand = new System.Random();
            // large number
            {
                for (int i = 0; i < 1000; i++)
                {
                    var expectedTail = rand.Next(1, 100);
                    var coroutines = Enumerable.Range(1, expectedTail)
                        .Select(x => new DecrementEnumerator(rand.Next(0, 100)))
                        .ToArray();
                    var mc = Create();
                    foreach (var item in coroutines)
                    {
                        mc.AddCoroutine(item);
                    }
                    var maxRunCount = coroutines.Max(x => x.OriginalCount);
                    var expected = coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).ToArray();
                    for (int j = 0; j < maxRunCount + 1; j++)
                    {
                        mc.Run();
                        // validate
                        expected = expected.Select(x => (x == -1) ? -1 : (x - 1)).ToArray();
                        coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(expected);
                        var tail = FindLast(mc);
                        GetTailDynamic(mc).Is(tail);
                    }
                    GetTailDynamic(mc).Is(0);
                }
            }
            {
                // small case
                for (int i = 0; i < 1000; i++)
                {
                    var expectedTail = rand.Next(1, 100);
                    var coroutines = Enumerable.Range(1, expectedTail)
                        .Select(x => new DecrementEnumerator(rand.Next(0, 5)))
                        .ToArray();
                    var mc = Create();
                    foreach (var item in coroutines)
                    {
                        mc.AddCoroutine(item);
                    }
                    var maxRunCount = coroutines.Max(x => x.OriginalCount);
                    var expected = coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).ToArray();
                    for (int j = 0; j < maxRunCount + 1; j++)
                    {
                        mc.Run();
                        // validate
                        expected = expected.Select(x => (x == -1) ? -1 : (x - 1)).ToArray();
                        coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(expected);
                        var tail = FindLast(mc);
                        GetTailDynamic(mc).Is(tail);
                    }
                    GetTailDynamic(mc).Is(0);
                }
            }
            {
                // small case2
                for (int i = 0; i < 1000; i++)
                {
                    var expectedTail = rand.Next(1, 10);
                    var coroutines = Enumerable.Range(1, expectedTail)
                        .Select(x => new DecrementEnumerator(rand.Next(0, 5)))
                        .ToArray();
                    var mc = Create();
                    foreach (var item in coroutines)
                    {
                        mc.AddCoroutine(item);
                    }
                    var maxRunCount = coroutines.Max(x => x.OriginalCount);
                    var expected = coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).ToArray();
                    for (int j = 0; j < maxRunCount + 1; j++)
                    {
                        mc.Run();
                        // validate
                        expected = expected.Select(x => (x == -1) ? -1 : (x - 1)).ToArray();
                        coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(expected);
                        var tail = FindLast(mc);
                        GetTailDynamic(mc).Is(tail);
                    }
                    GetTailDynamic(mc).Is(0);
                }
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservableConcatTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservableConcatTest();
            UnitTest.AddTest(test.CombineLatest);
            UnitTest.AddTest(test.CombineLatest2);
            UnitTest.AddTest(test.CombineLatest3);
            UnitTest.AddTest(test.CombineLatest4);
            UnitTest.AddTest(test.CombineLatestMulti);
            UnitTest.AddTest(test.CombineLatestMulti2);
            UnitTest.AddTest(test.CombineLatestMulti3);
            UnitTest.AddTest(test.CombineLatestMulti4);
            UnitTest.AddTest(test.Concat);
            UnitTest.AddTest(test.Merge);
            UnitTest.AddTest(test.MergeConcurrent);
            UnitTest.AddTest(test.StartWith);
            UnitTest.AddTest(test.Switch);
            UnitTest.AddTest(test.WhenAll);
            UnitTest.AddTest(test.WithLatestFrom);
            UnitTest.AddTest(test.Zip);
            UnitTest.AddTest(test.Zip2);
            UnitTest.AddTest(test.ZipLatest);
            UnitTest.AddTest(test.ZipLatest2);
            UnitTest.AddTest(test.ZipLatest2Ex);
            UnitTest.AddTest(test.ZipLatestMulti);
            UnitTest.AddTest(test.ZipLatestMulti2);
            UnitTest.AddTest(test.ZipLatestNth);
            UnitTest.AddTest(test.ZipMulti);
            UnitTest.AddTest(test.ZipMulti2);
            UnitTest.AddTest(test.ZipNth);
        }



        [TestMethod]
        public void CombineLatest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatest2()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatest3()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatest4()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatestMulti()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatestMulti2()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatestMulti3()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CombineLatestMulti4()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Concat()
        {
            SetScehdulerForImport();
            var a = Observable.Range(1, 5, Scheduler.ThreadPool);
            var b = Observable.Range(10, 3, Scheduler.ThreadPool);
            var c = Observable.Return(300, Scheduler.ThreadPool);
            Observable.Concat(a, b, c).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 10, 11, 12, 300);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Merge()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void MergeConcurrent()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void StartWith()
        {
            SetScehdulerForImport();
            var seq = Observable.Range(1, 5);
            seq.StartWith(100).ToArray().Wait().IsCollection(100, 1, 2, 3, 4, 5);
            seq.StartWith(() => 100).ToArray().Wait().IsCollection(100, 1, 2, 3, 4, 5);
            seq.StartWith(100, 1000, 10000).ToArray().Wait().IsCollection(100, 1000, 10000, 1, 2, 3, 4, 5);
            seq.StartWith(Enumerable.Range(100, 3)).ToArray().Wait().IsCollection(100, 101, 102, 1, 2, 3, 4, 5);
            seq.StartWith(Scheduler.ThreadPool, 100).ToArray().Wait().IsCollection(100, 1, 2, 3, 4, 5);
            seq.StartWith(Scheduler.ThreadPool, 100, 1000, 10000).ToArray().Wait().IsCollection(100, 1000, 10000, 1, 2, 3, 4, 5);
            seq.StartWith(Scheduler.ThreadPool, Enumerable.Range(100, 3)).ToArray().Wait().IsCollection(100, 101, 102, 1, 2, 3, 4, 5);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Switch()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WhenAll()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WithLatestFrom()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            var record = a.WithLatestFrom(b, (x, y) => new { x, y }).Record();
            b.OnNext(0); // 50
            b.OnNext(1); // 100
            a.OnNext(0); // 140
            b.OnNext(2); // 150
            b.OnNext(3); // 200
            b.OnNext(4); // 250
            a.OnNext(1); // 280
            b.OnNext(5); // 300
            b.OnNext(6); // 350
            b.OnNext(7); // 400
            a.OnNext(2); // 420
            b.OnNext(8); // 450
            b.OnNext(9); // 500
            b.OnNext(10); // 550
            a.OnNext(3); // 600
            record.Values.IsCollection(
                new { x = 0, y = 1 },
                new { x = 1, y = 4 },
                new { x = 2, y = 7 },
                new { x = 3, y = 10 });
            a.OnCompleted();
            record.Notifications.Last().Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Zip()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Zip2()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipLatest()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            a.OnNext(10);
            b.OnNext(20);
            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.ZipLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));
            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });
            b.OnNext(3000);
            l.Count.Is(1);
            a.OnNext(5000);
            l[1].Value.Is(new { x = 5000, y = 3000 });
            a.OnCompleted();
            l.Count.Is(2);
            a.OnNext(1001);
            l.Count.Is(2);
            b.OnNext(5);
            l.Count.Is(3);
            l[2].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipLatest2()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            a.OnNext(10);
            b.OnNext(20);
            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.ZipLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));
            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });
            b.OnNext(3000);
            l.Count.Is(1);
            a.OnNext(5000);
            l[1].Value.Is(new { x = 5000, y = 3000 });
            a.OnNext(9999); // one more
            a.OnCompleted();
            l.Count.Is(2);
            a.OnNext(1001);
            l.Count.Is(2);
            b.OnNext(5);
            l.Count.Is(4);
            l[2].Value.Is(new { x = 9999, y = 5 });
            l[3].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipLatest2Ex()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            a.OnNext(10);
            b.OnNext(20);
            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            a.ZipLatest(b, (x, y) => new { x, y }).Materialize().Subscribe(x => l.Add(x));
            b.OnNext(2000);
            a.OnCompleted();
            l.Count.Is(0);
            b.OnNext(30);
            l.Count.Is(1);
            l[0].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipLatestMulti()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.ZipLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));
            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });
            b.OnNext(3000);
            l.Count.Is(1);
            a.OnNext(5000);
            l[1].Value.Is(new { x = 5000, y = 3000 });
            a.OnCompleted();
            l.Count.Is(2);
            b.OnNext(5);
            l[2].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipLatestMulti2()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            var l = Enumerable.Empty<Unit>().Select(_ => Notification.CreateOnNext(new { x = 0, y = 0 })).ToList();
            Observable.ZipLatest(a, b).Select((xs) => new { x = xs[0], y = xs[1] }).Materialize().Subscribe(x => l.Add(x));
            a.OnNext(1000);
            b.OnNext(2000);
            l[0].Value.Is(new { x = 1000, y = 2000 });
            b.OnNext(3000);
            l.Count.Is(1);
            a.OnNext(5000);
            l[1].Value.Is(new { x = 5000, y = 3000 });
            a.OnNext(900);
            a.OnCompleted();
            l.Count.Is(2);
            b.OnNext(5);
            l[2].Value.Is(new { x = 900, y = 5 });
            l[3].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipLatestNth()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            var c = new Subject<int>();
            var d = new Subject<int>();
            var record = a.ZipLatest(b, c, d, (x, y, z, w) => new { x, y, z, w }).Record();
            a.OnNext(1);
            b.OnNext(2);
            c.OnNext(3);
            record.Values.Count.Is(0);
            d.OnNext(4);
            record.Values[0].Is(new { x = 1, y = 2, z = 3, w = 4 });
            a.OnNext(10);
            record.Values.Count.Is(1);
            b.OnNext(20);
            c.OnNext(30);
            d.OnNext(40);
            record.Values[1].Is(new { x = 10, y = 20, z = 30, w = 40 });
            // complete
            a.OnCompleted();
            record.Notifications.Count.Is(2);
            b.OnNext(200);
            record.Notifications.Count.Is(3);
            record.Notifications.Last().Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipMulti()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipMulti2()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ZipNth()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservableConcurrencyTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservableConcurrencyTest();
            UnitTest.AddTest(test.AmbMultiTest);
            UnitTest.AddTest(test.AmbTest);
            UnitTest.AddTest(test.ObserveOnTest);
        }



        [TestMethod]
        public void AmbMultiTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void AmbTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ObserveOnTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservableEventsTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservableEventsTest();
            UnitTest.AddTest(test.FromEvent);
            UnitTest.AddTest(test.FromEventPattern);
        }



        [TestMethod]
        public void FromEvent()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FromEventPattern()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservableGeneratorTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservableGeneratorTest();
            UnitTest.AddTest(test.Empty);
            UnitTest.AddTest(test.Never);
            UnitTest.AddTest(test.OptimizeReturnTest);
            UnitTest.AddTest(test.Range);
            UnitTest.AddTest(test.Repeat);
            UnitTest.AddTest(test.Repeat2);
            UnitTest.AddTest(test.RepeatStatic);
            UnitTest.AddTest(test.Return);
            UnitTest.AddTest(test.Throw);
            UnitTest.AddTest(test.ToObservable);
        }



        [TestMethod]
        public void Empty()
        {
            SetScehdulerForImport();
            var material = Observable.Empty<Unit>().Materialize().ToArray().Wait();
            material.IsCollection(Notification.CreateOnCompleted<Unit>());
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Never()
        {
            SetScehdulerForImport();
            AssertEx.Catch<TimeoutException>(() =>
                Observable.Never<Unit>().Materialize().ToArray().Wait(TimeSpan.FromMilliseconds(10)));
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void OptimizeReturnTest()
        {
            SetScehdulerForImport();
            for (int i = -1; i <= 9; i++)
            {
                var r = Observable.Return(i);
                var xs = r.Record();
                xs.Values[0].Is(i);
                r.GetType().FullName.Contains("ImmutableReturnInt32Observable").IsTrue();
            }
            foreach (var i in new[] { -2, 10, 100 })
            {
                var r = Observable.Return(i);
                r.Record().Values[0].Is(i);
                r.GetType().FullName.Contains("ImmediateReturnObservable").IsTrue();
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Range()
        {
            SetScehdulerForImport();
            Observable.Range(1, 5).ToArray().Wait().IsCollection(1, 2, 3, 4, 5);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Repeat()
        {
            SetScehdulerForImport();
            var xs = Observable.Range(1, 3, Scheduler.CurrentThread)
                .Concat(Observable.Return(100))
                .Repeat()
                .Take(10)
                .ToArray()
                .Wait();
            xs.IsCollection(1, 2, 3, 100, 1, 2, 3, 100, 1, 2);
            Observable.Repeat(100).Take(5).ToArray().Wait().IsCollection(100, 100, 100, 100, 100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Repeat2()
        {
            SetScehdulerForImport();
            Observable.Repeat("a", 5, Scheduler.Immediate).ToArrayWait().IsCollection("a", "a", "a", "a", "a");
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void RepeatStatic()
        {
            SetScehdulerForImport();
            var xss = Observable.Repeat(5, 3).ToArray().Wait();
            xss.IsCollection(5, 5, 5);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Return()
        {
            SetScehdulerForImport();
            Observable.Return(100).Materialize().ToArray().Wait().IsCollection(Notification.CreateOnNext(100), Notification.CreateOnCompleted<int>());
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Throw()
        {
            SetScehdulerForImport();
            var ex = new Exception();
            Observable.Throw<string>(ex).Materialize().ToArray().Wait().IsCollection(Notification.CreateOnError<string>(ex));
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToObservable()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservablePagingTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservablePagingTest();
            UnitTest.AddTest(test.Buffer);
            UnitTest.AddTest(test.Buffer3);
            UnitTest.AddTest(test.BufferComplete2);
            UnitTest.AddTest(test.BufferEmpty);
            UnitTest.AddTest(test.BufferSkip);
            UnitTest.AddTest(test.BufferTime);
            UnitTest.AddTest(test.BufferTimeAndCount);
            UnitTest.AddTest(test.BufferTimeAndCountTimeSide);
            UnitTest.AddTest(test.BufferTimeComplete);
            UnitTest.AddTest(test.BufferTimeEmptyBuffer);
            UnitTest.AddTest(test.BufferTimeEmptyComplete);
            UnitTest.AddTest(test.BufferWindowBoundaries);
            UnitTest.AddTest(test.First);
            UnitTest.AddTest(test.FirstOrDefault);
            UnitTest.AddTest(test.GroupBy);
            UnitTest.AddTest(test.Last);
            UnitTest.AddTest(test.LastOrDefault);
            UnitTest.AddTest(test.Pairwise);
            UnitTest.AddTest(test.Pairwise2);
            UnitTest.AddTest(test.Single);
            UnitTest.AddTest(test.SingleOrDefault);
            UnitTest.AddTest(test.Skip);
            UnitTest.AddTest(test.SkipTime);
            UnitTest.AddTest(test.SkipUntil);
            UnitTest.AddTest(test.SkipWhile);
            UnitTest.AddTest(test.SkipWhileIndex);
            UnitTest.AddTest(test.TakeLast);
            UnitTest.AddTest(test.TakeLastDuration);
            UnitTest.AddTest(test.TakeUntil);
            UnitTest.AddTest(test.TakeWhile);
        }



        [TestMethod]
        public void Buffer()
        {
            SetScehdulerForImport();
            var xs = Observable.Range(1, 10)
                .Buffer(3)
                .ToArray()
                .Wait();
            xs[0].IsCollection(1, 2, 3);
            xs[1].IsCollection(4, 5, 6);
            xs[2].IsCollection(7, 8, 9);
            xs[3].IsCollection(10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Buffer3()
        {
            SetScehdulerForImport();
            var xs = Observable.Empty<int>()
                .Buffer(1, 2)
                .ToArray()
                .Wait();
            xs.Length.Is(0);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferComplete2()
        {
            SetScehdulerForImport();
            var xs = Observable.Range(1, 2)
                .Buffer(2)
                .ToArray()
                .Wait();
            xs.Length.Is(1);
            xs[0].IsCollection(1, 2);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferEmpty()
        {
            SetScehdulerForImport();
            var xs = Observable.Empty<int>()
                .Buffer(10)
                .ToArray()
                .Wait();
            xs.Length.Is(0);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferSkip()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferTime()
        {
            SetScehdulerForImport();
            var hoge = Observable.Return(1000).Delay(TimeSpan.FromSeconds(4));
            var xs = Observable.Range(1, 10)
                .Concat(hoge)
                .Buffer(TimeSpan.FromSeconds(3))
                .ToArray()
                .Wait();
            xs[0].IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            xs[1].IsCollection(1000);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferTimeAndCount()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferTimeAndCountTimeSide()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            var record = subject.Buffer(TimeSpan.FromMilliseconds(100), 100).Take(5).Record();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            record.Values.Count.Is(5);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferTimeComplete()
        {
            SetScehdulerForImport();
            // when complete, return empty array.
            var xs = Observable.Return(1).Delay(TimeSpan.FromSeconds(2))
                .Concat(Observable.Return(1).Delay(TimeSpan.FromSeconds(2)).Skip(1))
                .Buffer(TimeSpan.FromSeconds(3))
                .ToArray()
                .Wait();
            xs.Length.Is(2);
            xs[0].Count.Is(1);
            xs[1].Count.Is(0);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferTimeEmptyBuffer()
        {
            SetScehdulerForImport();
            var xs = Observable.Return(10).Delay(TimeSpan.FromMilliseconds(3500))
                .Buffer(TimeSpan.FromSeconds(1))
                .ToArray()
                .Wait();
            xs.Length.Is(4);
            xs[0].Count.Is(0); // 1sec
            xs[1].Count.Is(0); // 2sec
            xs[2].Count.Is(0); // 3sec
            xs[3].Count.Is(1); // 4sec
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferTimeEmptyComplete()
        {
            SetScehdulerForImport();
            var xs = Observable.Empty<int>()
                .Buffer(TimeSpan.FromSeconds(1000))
                .ToArray()
                .Wait();
            xs.Length.Is(1);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BufferWindowBoundaries()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            var boundaries = new Subject<int>();
            var record = subject.Buffer(boundaries).Record();
            subject.OnNext(1);
            subject.OnNext(2);
            record.Values.Count.Is(0);
            boundaries.OnNext(0);
            record.Values.Count.Is(1);
            record.Values[0].IsCollection(1, 2);
            boundaries.OnNext(0);
            record.Values.Count.Is(2);
            record.Values[1].Count.Is(0);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void First()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FirstOrDefault()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void GroupBy()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            RecordObserver<int> a = null;
            RecordObserver<int> b = null;
            RecordObserver<int> c = null;
            subject.GroupBy(x => x % 3)
                .Subscribe(x =>
                {
                    if (x.Key == 0)
                    {
                        a = x.Record();
                    }
                    else if (x.Key == 1)
                    {
                        b = x.Record();
                    }
                    else if (x.Key == 2)
                    {
                        c = x.Record();
                    }
                });
            subject.OnNext(99);
            subject.OnNext(100);
            subject.OnNext(101);
            subject.OnNext(0);
            subject.OnNext(1);
            subject.OnNext(2);
            a.Values.IsCollection(99, 0);
            b.Values.IsCollection(100, 1);
            c.Values.IsCollection(101, 2);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Last()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void LastOrDefault()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Pairwise()
        {
            SetScehdulerForImport();
            var xs = Observable.Range(1, 5).Pairwise((x, y) => x + ":" + y).ToArrayWait();
            xs.IsCollection("1:2", "2:3", "3:4", "4:5");
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Pairwise2()
        {
            SetScehdulerForImport();
            var xs = Observable.Range(1, 5).Pairwise().ToArrayWait();
            xs[0].Previous.Is(1); xs[0].Current.Is(2);
            xs[1].Previous.Is(2); xs[1].Current.Is(3);
            xs[2].Previous.Is(3); xs[2].Current.Is(4);
            xs[3].Previous.Is(4); xs[3].Current.Is(5);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Single()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SingleOrDefault()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Skip()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SkipTime()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SkipUntil()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SkipWhile()
        {
            SetScehdulerForImport();
            Observable.Range(1, 10)
                .SkipWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .IsCollection(6, 7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SkipWhileIndex()
        {
            SetScehdulerForImport();
            Observable.Range(1, 10)
                .SkipWhile((x, i) => i <= 5)
                .ToArray()
                .Wait()
                .IsCollection(7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TakeLast()
        {
            SetScehdulerForImport();
            var record = Observable.Range(1, 2).TakeLast(3).Record();
            record.Values.IsCollection(1, 2);
            record = Observable.Range(1, 3).TakeLast(3).Record();
            record.Values.IsCollection(1, 2, 3);
            record = Observable.Range(1, 4).TakeLast(3).Record();
            record.Values.IsCollection(2, 3, 4);
            record = Observable.Range(1, 10).TakeLast(3).Record();
            record.Values.IsCollection(8, 9, 10);
            record = Observable.Empty<int>().TakeLast(3).Record();
            record.Notifications[0].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TakeLastDuration()
        {
            SetScehdulerForImport();
            var subject = new Subject<long>();
            var record = subject.Record();
            subject.OnCompleted();
            record.Notifications[0].Kind.Is(NotificationKind.OnCompleted);
            // 0, 200, 400, 600, 800
            var data = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(200))
                .Take(5)
                .TakeLast(TimeSpan.FromMilliseconds(250))
                .ToArrayWait();
            data.IsCollection(3, 4);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TakeUntil()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TakeWhile()
        {
            SetScehdulerForImport();
            Observable.Range(1, 10)
                .TakeWhile(x => x <= 5)
                .ToArray()
                .Wait()
                .IsCollection(1, 2, 3, 4, 5);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservableTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservableTest();
            UnitTest.AddTest(test.DefaultIfEmpty);
            UnitTest.AddTest(test.Dematerialize);
            UnitTest.AddTest(test.Distinct);
            UnitTest.AddTest(test.DistinctUntilChanged);
            UnitTest.AddTest(test.ForEachAsync);
            UnitTest.AddTest(test.IgnoreElements);
            UnitTest.AddTest(test.Materialize);
            UnitTest.AddTest(test.Select);
            UnitTest.AddTest(test.SelectMany);
            UnitTest.AddTest(test.ToArray);
            UnitTest.AddTest(test.ToArray_Dispose);
            UnitTest.AddTest(test.Wait);
            UnitTest.AddTest(test.Where);
        }



        [TestMethod]
        public void DefaultIfEmpty()
        {
            SetScehdulerForImport();
            Observable.Range(1, 3).DefaultIfEmpty(-1).ToArrayWait().IsCollection(1, 2, 3);
            Observable.Empty<int>().DefaultIfEmpty(-1).ToArrayWait().IsCollection(-1);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Dematerialize()
        {
            SetScehdulerForImport();
            var m = Observable.Empty<int>().Materialize().Dematerialize().ToArrayWait();
            m.IsCollection();
            m = Observable.Range(1, 3).Materialize().Dematerialize().ToArrayWait();
            m.IsCollection(1, 2, 3);
            var l = new List<int>();
            Observable.Range(1, 3).Concat(Observable.Throw<int>(new Exception())).Materialize()
                .Dematerialize()
                .Subscribe(x => l.Add(x), ex => l.Add(1000), () => l.Add(10000));
            l.IsCollection(1, 2, 3, 1000);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Distinct()
        {
            SetScehdulerForImport();
            {
                var subject = new Subject<int>();
                int[] array = null;
                subject.Distinct().ToArray().Subscribe(xs => array = xs);
                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }) { subject.OnNext(item); };
                subject.OnCompleted();
                array.IsCollection(1, 10, 100, 5, 70, 7);
            }
            {
                var subject = new Subject<int>();
                int[] array = null;
                subject.Distinct(x => x, EqualityComparer<int>.Default).ToArray().Subscribe(xs => array = xs);

                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5, 70, 7 }) { subject.OnNext(item); };
                subject.OnCompleted();
                array.IsCollection(1, 10, 100, 5, 70, 7);
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void DistinctUntilChanged()
        {
            SetScehdulerForImport();
            {
                var subject = new Subject<int>();
                int[] array = null;
                subject.DistinctUntilChanged().ToArray().Subscribe(xs => array = xs);
                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5 }) { subject.OnNext(item); };
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
                foreach (var item in new[] { 1, 10, 10, 1, 100, 100, 100, 5 }) { subject.OnNext(item); };
                subject.OnCompleted();
                array.IsCollection(1, 10, 1, 100, 5);
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ForEachAsync()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void IgnoreElements()
        {
            SetScehdulerForImport();
            var xs = Observable.Range(1, 10).IgnoreElements().Materialize().ToArrayWait();
            xs[0].Kind.Is(NotificationKind.OnCompleted);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Materialize()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Select()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SelectMany()
        {
            SetScehdulerForImport();
            var a = new Subject<int>();
            var b = new Subject<int>();
            var list = new List<int>();
            a.SelectMany(_ => b).Subscribe(x => list.Add(x));
            a.OnNext(10);
            a.OnCompleted();
            b.OnNext(100);
            list.Count.Is(1);
            list[0].Is(100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToArray()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            int[] array = null;
            subject.ToArray().Subscribe(xs => array = xs);
            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);
            subject.OnCompleted();
            array.IsCollection(1, 10, 100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToArray_Dispose()
        {
            SetScehdulerForImport();
            var subject = new Subject<int>();
            int[] array = null;
            var disp = subject.ToArray().Subscribe(xs => array = xs);
            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);
            disp.Dispose();
            subject.OnCompleted();
            array.IsNull();
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Wait()
        {
            SetScehdulerForImport();
#if !UNITY_METRO
            var subject = new Subject<int>();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                subject.OnNext(100);
                subject.OnCompleted();
            });
            subject.Wait().Is(100);
#endif
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Where()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ObservableTimeTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ObservableTimeTest();
            UnitTest.AddTest(test.DelayTest);
            UnitTest.AddTest(test.SampleTest);
            UnitTest.AddTest(test.ThrottleFirstTest);
            UnitTest.AddTest(test.ThrottleTest);
            UnitTest.AddTest(test.TimeInterval);
            UnitTest.AddTest(test.TimeoutTest);
            UnitTest.AddTest(test.TimeoutTestOffset);
            UnitTest.AddTest(test.TimerTest);
            UnitTest.AddTest(test.Timestamp);
        }



        [TestMethod]
        public void DelayTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SampleTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ThrottleFirstTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ThrottleTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TimeInterval()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TimeoutTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TimeoutTestOffset()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void TimerTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Timestamp()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class QueueWorkerTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new QueueWorkerTest();
            UnitTest.AddTest(test.Enq);
        }



        [TestMethod]
        public void Enq()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class RangeTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new RangeTest();
            UnitTest.AddTest(test.Range);
        }



        [TestMethod]
        public void Range()
        {
            SetScehdulerForImport();
            AssertEx.Throws<ArgumentOutOfRangeException>(() => Observable.Range(1, -1).ToArray().Wait());
            Observable.Range(1, 0).ToArray().Wait().Length.Is(0);
            Observable.Range(1, 10).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            Observable.Range(1, 0, Scheduler.Immediate).ToArray().Wait().Length.Is(0);
            Observable.Range(1, 10, Scheduler.Immediate).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ReactivePropertyTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ReactivePropertyTest();
            UnitTest.AddTest(test.ClassType);
            UnitTest.AddTest(test.FinishedSourceToReactiveProperty);
            UnitTest.AddTest(test.FinishedSourceToReadOnlyReactiveProperty);
            UnitTest.AddTest(test.ToReadOnlyReactivePropertyClassType);
            UnitTest.AddTest(test.ToReadOnlyReactivePropertyValueType);
            UnitTest.AddTest(test.ValueType);
            UnitTest.AddTest(test.WithLastTest);
        }



        [TestMethod]
        public void ClassType()
        {
            SetScehdulerForImport();
            {
                var rp = new ReactiveProperty<string>(); // null
                var result = rp.Record();
                result.Values.IsCollection((string)null);
                rp.Value = null;
                result.Values.IsCollection((string)null);
                rp.Value = "a";
                result.Values.IsCollection((string)null, "a");
                rp.Value = "b";
                result.Values.IsCollection((string)null, "a", "b");
                rp.Value = "b";
                result.Values.IsCollection((string)null, "a", "b");
            }
            {
                var rp = new ReactiveProperty<string>("z");
                var result = rp.Record();
                result.Values.IsCollection("z");
                rp.Value = "z";
                result.Values.IsCollection("z");
                rp.Value = "a";
                result.Values.IsCollection("z", "a");
                rp.Value = "b";
                result.Values.IsCollection("z", "a", "b");
                rp.Value = "b";
                result.Values.IsCollection("z", "a", "b");
                rp.Value = null;
                result.Values.IsCollection("z", "a", "b", null);
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FinishedSourceToReactiveProperty()
        {
            SetScehdulerForImport();
            // pattern of OnCompleted
            {
                var source = Observable.Return(9);
                var rxProp = source.ToReactiveProperty();
                var notifications = rxProp.Record().Notifications;
                notifications.IsCollection(Notification.CreateOnNext(9));
                rxProp.Value = 9999;
                notifications.IsCollection(Notification.CreateOnNext(9), Notification.CreateOnNext(9999));
                rxProp.Record().Values.IsCollection(9999);
            }
            // pattern of OnError
            {
                // after
                {
                    var ex = new Exception();
                    var source = Observable.Throw<int>(ex);
                    var rxProp = source.ToReactiveProperty();
                    var notifications = rxProp.Record().Notifications;
                    notifications.IsCollection(Notification.CreateOnError<int>(ex));
                    rxProp.Value = 9999;
                    notifications.IsCollection(Notification.CreateOnError<int>(ex));
                    rxProp.Record().Notifications.IsCollection(Notification.CreateOnError<int>(ex));
                }
                // immediate
                {
                    var ex = new Exception();
                    var source = new Subject<int>();
                    var rxProp = source.ToReactiveProperty();
                    var record = rxProp.Record();
                    source.OnError(new Exception());
                    var notifications = record.Notifications;
                    notifications.Count.Is(1);
                    notifications[0].Kind.Is(NotificationKind.OnError);
                    rxProp.Value = 9999;
                    notifications.Count.Is(1);
                    notifications[0].Kind.Is(NotificationKind.OnError);
                    rxProp.Record().Notifications[0].Kind.Is(NotificationKind.OnError);
                }
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void FinishedSourceToReadOnlyReactiveProperty()
        {
            SetScehdulerForImport();
            // pattern of OnCompleted
            {
                var source = Observable.Return(9);
                var rxProp = source.ToReadOnlyReactiveProperty();
                var notifications = rxProp.Record().Notifications;
                notifications.IsCollection(Notification.CreateOnNext(9), Notification.CreateOnCompleted<int>());
                rxProp.Record().Notifications.IsCollection(
                    Notification.CreateOnNext(9),
                    Notification.CreateOnCompleted<int>());
            }
            // pattern of OnError
            {
                // after
                {
                    var ex = new Exception();
                    var source = Observable.Throw<int>(ex);
                    var rxProp = source.ToReadOnlyReactiveProperty();
                    var notifications = rxProp.Record().Notifications;
                    notifications.IsCollection(Notification.CreateOnError<int>(ex));
                    rxProp.Record().Notifications.IsCollection(Notification.CreateOnError<int>(ex));
                }
                // immediate
                {
                    var ex = new Exception();
                    var source = new Subject<int>();
                    var rxProp = source.ToReadOnlyReactiveProperty();
                    var record = rxProp.Record();
                    source.OnError(new Exception());
                    var notifications = record.Notifications;
                    notifications.Count.Is(1);
                    notifications[0].Kind.Is(NotificationKind.OnError);
                    rxProp.Record().Notifications[0].Kind.Is(NotificationKind.OnError);
                }
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToReadOnlyReactivePropertyClassType()
        {
            SetScehdulerForImport();
            {
                var source = new Subject<string>();
                var rp = source.ToReadOnlyReactiveProperty();
                var result = rp.Record();
                result.Values.Count.Is(0);
                source.OnNext(null);
                result.Values.IsCollection((string)null);
                source.OnNext("a");
                result.Values.IsCollection((string)null, "a");
                source.OnNext("b");
                result.Values.IsCollection((string)null, "a", "b");
                source.OnNext("b");
                result.Values.IsCollection((string)null, "a", "b");
            }
            {
                var source = new Subject<string>();
                var rp = source.ToSequentialReadOnlyReactiveProperty();
                var result = rp.Record();
                result.Values.Count.Is(0);
                source.OnNext(null);
                result.Values.IsCollection((string)null);
                source.OnNext("a");
                result.Values.IsCollection((string)null, "a");
                source.OnNext("b");
                result.Values.IsCollection((string)null, "a", "b");
                source.OnNext("b");
                result.Values.IsCollection((string)null, "a", "b", "b");
            }
            {
                var source = new Subject<string>();
                var rp = source.ToReadOnlyReactiveProperty("z");
                var result = rp.Record();
                result.Values.IsCollection("z");
                source.OnNext("z");
                result.Values.IsCollection("z");
                source.OnNext("a");
                result.Values.IsCollection("z", "a");
                source.OnNext("b");
                result.Values.IsCollection("z", "a", "b");
                source.OnNext("b");
                result.Values.IsCollection("z", "a", "b");
                source.OnNext(null);
                result.Values.IsCollection("z", "a", "b", null);
                source.OnNext(null);
                result.Values.IsCollection("z", "a", "b", null);
            }
            {
                var source = new Subject<string>();
                var rp = source.ToSequentialReadOnlyReactiveProperty("z");
                var result = rp.Record();
                result.Values.IsCollection("z");
                source.OnNext("z");
                result.Values.IsCollection("z", "z");
                source.OnNext("a");
                result.Values.IsCollection("z", "z", "a");
                source.OnNext("b");
                result.Values.IsCollection("z", "z", "a", "b");
                source.OnNext("b");
                result.Values.IsCollection("z", "z", "a", "b", "b");
                source.OnNext(null);
                result.Values.IsCollection("z", "z", "a", "b", "b", null);
                source.OnNext(null);
                result.Values.IsCollection("z", "z", "a", "b", "b", null, null);
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToReadOnlyReactivePropertyValueType()
        {
            SetScehdulerForImport();
            {
                var source = new Subject<int>();
                var rp = source.ToReadOnlyReactiveProperty();
                var result = rp.Record();
                result.Values.Count.Is(0);
                source.OnNext(0);
                result.Values.IsCollection(0);
                source.OnNext(10);
                result.Values.IsCollection(0, 10);
                source.OnNext(100);
                result.Values.IsCollection(0, 10, 100);
                source.OnNext(100);
                result.Values.IsCollection(0, 10, 100);
            }
            {
                var source = new Subject<int>();
                var rp = source.ToSequentialReadOnlyReactiveProperty();
                var result = rp.Record();
                result.Values.Count.Is(0);
                source.OnNext(0);
                result.Values.IsCollection(0);
                source.OnNext(10);
                result.Values.IsCollection(0, 10);
                source.OnNext(100);
                result.Values.IsCollection(0, 10, 100);
                source.OnNext(100);
                result.Values.IsCollection(0, 10, 100, 100);
            }
            {
                var source = new Subject<int>();
                var rp = source.ToReadOnlyReactiveProperty(0);
                var result = rp.Record();
                result.Values.IsCollection(0);
                source.OnNext(0);
                result.Values.IsCollection(0);
                source.OnNext(10);
                result.Values.IsCollection(0, 10);
                source.OnNext(100);
                result.Values.IsCollection(0, 10, 100);
                source.OnNext(100);
                result.Values.IsCollection(0, 10, 100);
            }
            {
                var source = new Subject<int>();
                var rp = source.ToSequentialReadOnlyReactiveProperty(0);
                var result = rp.Record();
                result.Values.IsCollection(0);
                source.OnNext(0);
                result.Values.IsCollection(0, 0);
                source.OnNext(10);
                result.Values.IsCollection(0, 0, 10);
                source.OnNext(100);
                result.Values.IsCollection(0, 0, 10, 100);
                source.OnNext(100);
                result.Values.IsCollection(0, 0, 10, 100, 100);
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ValueType()
        {
            SetScehdulerForImport();
            {
                var rp = new ReactiveProperty<int>(); // 0
                var result = rp.Record();
                result.Values.IsCollection(0);
                rp.Value = 0;
                result.Values.IsCollection(0);
                rp.Value = 10;
                result.Values.IsCollection(0, 10);
                rp.Value = 100;
                result.Values.IsCollection(0, 10, 100);
                rp.Value = 100;
                result.Values.IsCollection(0, 10, 100);
            }
            {
                var rp = new ReactiveProperty<int>(20);
                var result = rp.Record();
                result.Values.IsCollection(20);
                rp.Value = 0;
                result.Values.IsCollection(20, 0);
                rp.Value = 10;
                result.Values.IsCollection(20, 0, 10);
                rp.Value = 100;
                result.Values.IsCollection(20, 0, 10, 100);
                rp.Value = 100;
                result.Values.IsCollection(20, 0, 10, 100);
            }
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WithLastTest()
        {
            SetScehdulerForImport();
            var rp1 = Observable.Return("1").ToReadOnlyReactiveProperty();
            rp1.Last().Record().Notifications.IsCollection(
                Notification.CreateOnNext("1"),
                Notification.CreateOnCompleted<string>());
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class SchedulerTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new SchedulerTest();
            UnitTest.AddTest(test.CurrentThread);
            UnitTest.AddTest(test.CurrentThread2);
            UnitTest.AddTest(test.CurrentThread3);
            UnitTest.AddTest(test.Immediate);
        }



        [TestMethod]
        public void CurrentThread()
        {
            SetScehdulerForImport();
            var hoge = ScheduleTasks(Scheduler.CurrentThread);
            hoge.IsCollection("outer start.", "outer end.", "--innerAction start.", "--innerAction end.", "----leafAction.");
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CurrentThread2()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void CurrentThread3()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Immediate()
        {
            SetScehdulerForImport();
            var hoge = ScheduleTasks(Scheduler.Immediate);
            hoge.IsCollection("outer start.", "--innerAction start.", "----leafAction.", "--innerAction end.", "outer end.");
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class SelectMany
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new SelectMany();
            UnitTest.AddTest(test.ResultSelector);
            UnitTest.AddTest(test.ResultSelectorEnumerable);
            UnitTest.AddTest(test.ResultSelectorEnumerableWithIndex);
            UnitTest.AddTest(test.ResultSelectorWithIndex);
            UnitTest.AddTest(test.Selector);
            UnitTest.AddTest(test.SelectorEnumerable);
            UnitTest.AddTest(test.SelectorEnumerableWithIndex);
            UnitTest.AddTest(test.SelectorWithIndex);
        }



        [TestMethod]
        public void ResultSelector()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ResultSelectorEnumerable()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ResultSelectorEnumerableWithIndex()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ResultSelectorWithIndex()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Selector()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SelectorEnumerable()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SelectorEnumerableWithIndex()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SelectorWithIndex()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class SelectWhereOptimizeTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new SelectWhereOptimizeTest();
            UnitTest.AddTest(test.SelectSelect);
            UnitTest.AddTest(test.SelectWhere);
            UnitTest.AddTest(test.WhereSelect);
            UnitTest.AddTest(test.WhereWhere);
        }



        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SelectSelect()
        {
            SetScehdulerForImport();
            // Combine selector currently disabled.
            var selectselect = Observable.Range(1, 10)
                .Select(x => x)
                .Select(x => x * -1);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SelectWhere()
        {
            SetScehdulerForImport();
            var selectWhere = Observable.Range(1, 10)
                .Select(x => x * x)
                .Where(x => x % 2 == 0);
            selectWhere.GetType().Name.Contains("SelectWhere").IsTrue();
            selectWhere.ToArrayWait().IsCollection(4, 16, 36, 64, 100);
            var selectWhere2 = Observable.Range(1, 10)
                .Select((x, i) => x * x)
                .Where(x => x % 2 == 0);
            selectWhere2.GetType().Name.Contains("SelectWhere").IsFalse();
            selectWhere2.ToArrayWait().IsCollection(4, 16, 36, 64, 100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void WhereSelect()
        {
            SetScehdulerForImport();
            var whereSelect = Observable.Range(1, 10)
                .Where(x => x % 2 == 0)
                .Select(x => x * x);
            whereSelect.GetType().Name.Contains("WhereSelect").IsTrue();
            whereSelect.ToArrayWait().IsCollection(4, 16, 36, 64, 100);
            var whereSelect2 = Observable.Range(1, 10)
                .Where((x, i) => x % 2 == 0)
                .Select(x => x * x);
            whereSelect2.GetType().Name.Contains("WhereSelect").IsFalse();
            whereSelect2.ToArrayWait().IsCollection(4, 16, 36, 64, 100);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void WhereWhere()
        {
            SetScehdulerForImport();
            var wherewhere = Observable.Range(1, 10)
                .Where(x => x % 2 == 0)
                .Where(x => x > 5);
            wherewhere.ToArrayWait().IsCollection(6, 8, 10);
            var wherewhere2 = Observable.Range(1, 10)
                .Where((x, i) => x % 2 == 0)
                .Where(x => x > 5);
            wherewhere2.ToArrayWait().IsCollection(6, 8, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class SubjectTests
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new SubjectTests();
            UnitTest.AddTest(test.AsyncSubjectTest);
            UnitTest.AddTest(test.BehaviorSubject);
            UnitTest.AddTest(test.ReplaySubject);
            UnitTest.AddTest(test.ReplaySubjectWindowReplay);
            UnitTest.AddTest(test.Subject);
            UnitTest.AddTest(test.SubjectSubscribeTest);
        }



        [TestMethod]
        public void AsyncSubjectTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void BehaviorSubject()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ReplaySubject()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ReplaySubjectWindowReplay()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void Subject()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void SubjectSubscribeTest()
        {
            SetScehdulerForImport();
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
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class TakeTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new TakeTest();
            UnitTest.AddTest(test.TakeCount);
        }



        [TestMethod]
        public void TakeCount()
        {
            SetScehdulerForImport();
            var range = Observable.Range(1, 10);
            AssertEx.Throws<ArgumentOutOfRangeException>(() => range.Take(-1));
            range.Take(0).ToArray().Wait().Length.Is(0);
            range.Take(3).ToArrayWait().IsCollection(1, 2, 3);
            range.Take(15).ToArrayWait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class ToTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new ToTest();
            UnitTest.AddTest(test.ToArray);
            UnitTest.AddTest(test.ToList);
        }



        [TestMethod]
        public void ToArray()
        {
            SetScehdulerForImport();
            Observable.Empty<int>().ToArray().Wait().IsCollection();
            Observable.Return(10).ToArray().Wait().IsCollection(10);
            Observable.Range(1, 10).ToArray().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void ToList()
        {
            SetScehdulerForImport();
            Observable.Empty<int>().ToList().Wait().IsCollection();
            Observable.Return(10).ToList().Wait().IsCollection(10);
            Observable.Range(1, 10).ToList().Wait().IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }


    public partial class WhenAllTest
    {
        void SetScehdulerForImport()
        {

            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            var test = new WhenAllTest();
            UnitTest.AddTest(test.WhenAll);
            UnitTest.AddTest(test.WhenAllEmpty);
            UnitTest.AddTest(test.WhenAllEnumerable);
            UnitTest.AddTest(test.WhenAllUnit);
            UnitTest.AddTest(test.WhenAllUnitEmpty);
            UnitTest.AddTest(test.WhenAllUnitEnumerable);
        }



        [TestMethod]
        public void WhenAll()
        {
            SetScehdulerForImport();
            var xs = Observable.WhenAll(
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4))
                .Wait();
            xs.IsCollection(100, 5, 4);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WhenAllEmpty()
        {
            SetScehdulerForImport();
            var xs = Observable.WhenAll(new IObservable<int>[0]).Wait();
            xs.Length.Is(0);
            var xs2 = Observable.WhenAll(Enumerable.Empty<IObservable<int>>().Select(x => x)).Wait();
            xs2.Length.Is(0);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WhenAllEnumerable()
        {
            SetScehdulerForImport();
            var xs = new[] {
                    Observable.Return(100),
                    Observable.Timer(TimeSpan.FromSeconds(1)).Select(_ => 5),
                    Observable.Range(1, 4)
            }.Select(x => x).WhenAll().Wait();
            xs.IsCollection(100, 5, 4);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WhenAllUnit()
        {
            SetScehdulerForImport();
            var xs = Observable.WhenAll(
                    Observable.Return(100).AsUnitObservable(),
                    Observable.Timer(TimeSpan.FromSeconds(1)).AsUnitObservable(),
                    Observable.Range(1, 4).AsUnitObservable())
                .Wait();
            xs.Is(Unit.Default);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WhenAllUnitEmpty()
        {
            SetScehdulerForImport();
            var xs = Observable.WhenAll(new IObservable<Unit>[0]).Wait();
            xs.Is(Unit.Default);
            var xs2 = Observable.WhenAll(Enumerable.Empty<IObservable<Unit>>().Select(x => x)).Wait();
            xs2.Is(Unit.Default);
            UniRx.Scheduler.SetDefaultForUnity();
        }


        [TestMethod]
        public void WhenAllUnitEnumerable()
        {
            SetScehdulerForImport();
            var xs = new[] {
                    Observable.Return(100).AsUnitObservable(),
                    Observable.Timer(TimeSpan.FromSeconds(1)).AsUnitObservable(),
                    Observable.Range(1, 4).AsUnitObservable()
            }.Select(x => x).WhenAll().Wait();
            xs.Is(Unit.Default);
            UniRx.Scheduler.SetDefaultForUnity();
        }


    }

}

#pragma warning restore 168
#endif
