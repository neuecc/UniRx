using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;

namespace OfficialRx
{
    [TestClass]
    public class ObservableTimeTestCopy
    {
        [TestMethod]
        public void DelayRxOfficial()
        {
            /*
            var xs = Observable.Range(1, 3)
                .Delay(TimeSpan.FromSeconds(1))
                .Timestamp()
                .ToArray()
                .Wait();
            */
            var subject = new Subject<int>();

            var now = ThreadPoolScheduler.Instance.Now;
            Timestamped<Notification<int>> hoge = default(Timestamped<Notification<int>>);
            subject.Delay(TimeSpan.FromSeconds(1))
                .Materialize()
                .Timestamp().Subscribe(x => hoge = x);

            //subject.OnNext(1);
            //subject.OnError(new Exception());
            subject.OnCompleted();

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }
}
