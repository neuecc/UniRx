using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Operators
{
    [TestClass]
    public class DurabilityTest
    {
        public delegate void LikeUnityAction();
        public delegate void LikeUnityAction<T0>(T0 arg0);

        public class MyEventArgs : EventArgs
        {
            public int MyProperty { get; set; }

            public MyEventArgs(int x)
            {
                this.MyProperty = x;
            }
        }

        public class EventTester
        {
            public EventHandler<MyEventArgs> genericEventHandler;
            public LikeUnityAction unityAction;
            public LikeUnityAction<int> intUnityAction;
            public Action<int> intAction;
            public Action unitAction;

            public void Fire(int x)
            {
                if (genericEventHandler != null)
                {
                    genericEventHandler.Invoke(this, new MyEventArgs(x));
                }
                else if (unityAction != null)
                {
                    unityAction();
                }
                else if (intUnityAction != null)
                {
                    intUnityAction(x);
                }
                else if (intAction != null)
                {
                    intAction.Invoke(x);
                }
                else if (unitAction != null)
                {
                    unitAction.Invoke();
                }
            }
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
}
