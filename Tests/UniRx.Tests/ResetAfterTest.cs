using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class ResetAfterTest
    {
        [TestMethod]
        public void ResetAfter()
        {
            //should publish default value
            var results = Observable.Concat(
                    Observable.Return(1),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(0.5)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(1.1))
                )
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArrayWait();

            results.Length.Is(5);
            results[0].Value.Is(1);
            results[1].Value.Is(2);
            results[2].Value.Is(0); //default value
            results[3].Value.Is(3);
            results[4].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ResetAfter2()
        {
            //should measure time from the last message
            var results = Observable.Concat(
                    Observable.Return(1).Delay(TimeSpan.FromSeconds(0.1)),
                    Observable.Return(2).Delay(TimeSpan.FromSeconds(0.1)),
                    Observable.Return(3).Delay(TimeSpan.FromSeconds(1.1)), // after 1 second from previous message
                    Observable.Return(4).Delay(TimeSpan.FromSeconds(0.1))
                )
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArrayWait();

            results.Length.Is(6);
            results[0].Value.Is(1);
            results[1].Value.Is(2);
            results[2].Value.Is(0); //default value
            results[3].Value.Is(3);
            results[4].Value.Is(4);
            results[5].Kind.Is(NotificationKind.OnCompleted);
        }


        [TestMethod]
        public void ResetAfter3()
        {
            //should publish default value even if last message is the same as default value 
            var results = Observable.Concat(
                    Observable.Return(0),
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(1.5))
                )
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArrayWait();

            results.Length.Is(4);
            results[0].Value.Is(0);
            results[1].Value.Is(0);//default value
            results[2].Value.Is(5);
            results[3].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ResetAfter4()
        {
            //should be able to set any value as default
            var results = Observable.Concat(
                    Observable.Return("first"),
                    Observable.Return("second").Delay(TimeSpan.FromSeconds(1.5))
                )
                .ResetAfter("default", TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArrayWait();

            results.Length.Is(4);
            results[0].Value.Is("first");
            results[1].Value.Is("default");//default value
            results[2].Value.Is("second");
            results[3].Kind.Is(NotificationKind.OnCompleted);
        }

        [TestMethod]
        public void ResetAfter5()
        {
            //should publish OnCompleted immediately when parent observer is finished
            var results = Observable.Return(1)
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArrayWait();

            results.Length.Is(2);
            results[0].Value.Is(1);
            results[1].Kind.Is(NotificationKind.OnCompleted);
        }


        [TestMethod]
        public void ResetAfter6()
        {
            //should publish OnError immediately
            var results = Observable.Return(1)
                .Concat(Observable.Throw<int>(new Exception("error occurred.")))
                .ResetAfter(TimeSpan.FromSeconds(1))
                .Materialize()
                .ToArrayWait();

            results.Length.Is(2);
            results[0].Value.Is(1);
            results[1].Kind.Is(NotificationKind.OnError);
        }
    }
}
