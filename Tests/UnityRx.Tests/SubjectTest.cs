using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnityRx.Tests
{
    [TestClass]
    public class SubjectTests
    {
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
            }
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
    }
}
