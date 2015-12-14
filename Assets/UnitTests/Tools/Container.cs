using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Tests
{
    public partial class SchedulerTest
    {
        private static string[] ScheduleTasks(IScheduler scheduler)
        {
            var list = new List<string>();

            Action leafAction = () => list.Add("----leafAction.");
            Action innerAction = () =>
            {
                list.Add("--innerAction start.");
                scheduler.Schedule(leafAction);
                list.Add("--innerAction end.");
            };
            Action outerAction = () =>
            {
                list.Add("outer start.");
                scheduler.Schedule(innerAction);
                list.Add("outer end.");
            };
            scheduler.Schedule(outerAction);

            return list.ToArray();
        }
    }

    public delegate void LikeUnityAction();
    public delegate void LikeUnityAction<T0>(T0 arg0);

    public class MyEventArgs : EventArgs
    {
        public int MyProperty { get; set; }

        public MyEventArgs()
        {

        }

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

    public class EventTestesr
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

    public delegate void MyEventHandler(object sender, MyEventArgs eventArgs);


    public class IdDisp : IDisposable
    {
        public bool IsDisposed { get; set; }
        public int Id { get; set; }

        public IdDisp(int id)
        {
            this.Id = id;
        }


        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public class ListObserver : IObserver<int>
    {
        public List<int> list = new List<int>();

        public void OnCompleted()
        {
            list.Add(1000);
        }

        public void OnError(Exception error)
        {
            list.Add(100);
        }

        public void OnNext(int value)
        {
            list.Add(value);
        }
    }
}
