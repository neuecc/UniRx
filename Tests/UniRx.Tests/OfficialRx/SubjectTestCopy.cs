using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using System.Reactive.Concurrency;

namespace OfficialRx
{
    [TestClass]
    public class SubjectTests
    {
        [TestMethod]
        public void SubjectOfficialRx()
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
                onNext.Is(1, 10, 100, 1000);

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
                onNext.Is(1, 10, 100, 1000);

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
        public void AsyncSubjectOfficialRx()
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

                subject.IsCompleted.IsFalse();

                subject.OnCompleted();
                onNext.Is(1000);
                onCompletedCallCount.Is(1);

                subject.IsCompleted.IsTrue();

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
                onNext.Is(1000, 1000);
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

                subject.IsCompleted.IsFalse();

                subject.OnError(new Exception());

                subject.IsCompleted.IsTrue();

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
        public void BehaviorSubjectOfficialRx()
        {
             // OnCompletedPattern
            {
                var subject = new BehaviorSubject<int>(3333);
                
                var onNext = new List<int>();
                var exception = new List<Exception>();
                int onCompletedCallCount = 0;
                var _ = subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

                onNext.Is(3333);

                subject.OnNext(1);
                subject.OnNext(10);
                subject.OnNext(100);
                subject.OnNext(1000);
                
                onNext.Is(3333, 1, 10, 100, 1000);

                // re subscription
                onNext.Clear();
                _.Dispose();
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Is(1000);

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
                onNext.Is(3333, 1, 10, 100, 1000);

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
        public void ReplaySubjectRxOfficial()
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
                onNext.Is(1, 10, 100, 1000);

                // replay subscription
                onNext.Clear();
                _.Dispose();
                subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
                onNext.Is(1, 10, 100, 1000);

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
                onNext.Is(1, 10, 100, 1000);
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
                onNext.Is(1, 10, 100, 1000);

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
                onNext.Is(1, 10, 100, 1000);
                exception.Count.Is(1);
                onCompletedCallCount.Is(0);
            }
        }

        [TestMethod]
        public void ReplaySubjectWindowReplayRxOfficial()
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
            onNext.Is(10, 100);

            subject.OnNext(1000); // 900
            Thread.Sleep(TimeSpan.FromMilliseconds(300));

            _.Dispose();
            onNext.Clear();
            subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
            onNext.Is(100, 1000);

            subject.OnNext(10000); // 1200
            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            subject.OnNext(2); // 1500
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            subject.OnNext(20); // 1800
            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            _.Dispose();
            onNext.Clear();
            subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
            onNext.Is(10000, 2, 20);
        }
    }
}
