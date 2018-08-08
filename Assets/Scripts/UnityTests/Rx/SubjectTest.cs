using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace UniRx.Tests
{
    
    public class SubjectTests
    {
        [Test]
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

        [Test]
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

            Assert.Throws<ObjectDisposedException>(() => subject.OnNext(0));
            Assert.Throws<ObjectDisposedException>(() => subject.OnError(new Exception()));
            Assert.Throws<ObjectDisposedException>(() => subject.OnCompleted());
        }

        [Test]
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
                onNext.Is(1000);
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

        [Test]
        public void BehaviorSubject()
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

        [Test]
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

        public void ReplaySubjectBufferReplay()
        {
            var subject = new ReplaySubject<int>(bufferSize: 3);

            var onNext = new List<int>();
            var exception = new List<Exception>();
            int onCompletedCallCount = 0;
            var _ = subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);

            subject.OnNext(1);
            subject.OnNext(10);
            subject.OnNext(100);
            subject.OnNext(1000);
            subject.OnNext(10000);
            onNext.Is(100, 1000, 10000);  // cut 1, 10

            // replay subscription
            onNext.Clear();
            _.Dispose();
            subject.Subscribe(x => onNext.Add(x), x => exception.Add(x), () => onCompletedCallCount++);
            onNext.Is(100, 1000, 10000);

            subject.OnNext(20000);
            onNext.Is(1000, 10000, 20000);

            subject.OnCompleted();
            onCompletedCallCount.Is(1);
        }

        [Test]
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
