using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests
{
    [TestClass]
    public class ReactivePropertyTest
    {
        [TestMethod]
        public void ValueType()
        {
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
        }

        [TestMethod]
        public void ClassType()
        {
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
        }

        [TestMethod]
        public void ToReadOnlyReactivePropertyValueType()
        {
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
        }



        [TestMethod]
        public void ToReadOnlyReactivePropertyClassType()
        {
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
        }

        [TestMethod]
        public void FinishedSourceToReactiveProperty()
        {
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
        }

        [TestMethod]
        public void FinishedSourceToReadOnlyReactiveProperty()
        {
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
        }

        [TestMethod]
        public void WithLastTest()
        {
            var rp1 = Observable.Return("1").ToReadOnlyReactiveProperty();
            rp1.Last().Record().Notifications.IsCollection(
                Notification.CreateOnNext("1"),
                Notification.CreateOnCompleted<string>());
        }
    }
}
