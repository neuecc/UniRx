using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservableEventsTest
    {
        class EventTestesr
        {
            public event EventHandler Event1;
            public event EventHandler<MyEventArgs> Event2;
            public event MyEventHandler Event3;
            public event Action Event4;
            public event Action<int> Event5;
            public event Action<int, string> Event6;

            public void Fire(int num)
            {
                switch (num)
                {
                    case 1:
                        if (Event1 == null) return;
                        Event1(this, new EventArgs());
                        break;
                    case 2:
                        if (Event2 == null) return;
                        Event2(this, new MyEventArgs());
                        break;
                    case 3:
                        if (Event3 == null) return;
                        Event3(this, new MyEventArgs());
                        break;
                    case 4:
                        if (Event4 == null) return;
                        Event4();
                        break;
                    case 5:
                        if (Event5 == null) return;
                        Event5(100);
                        break;
                    case 6:
                        if (Event6 == null) return;
                        Event6(100, "hogehoge");
                        break;
                    default:
                        break;
                }
            }
        }

        delegate void MyEventHandler(object sender, MyEventArgs eventArgs);

        class MyEventArgs : EventArgs
        {

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
}