using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public sealed class ReplaySubject<T> : ISubject<T>
    {
        bool isStopped;
        Exception lastError;
        List<IObserver<T>> observers = new List<IObserver<T>>();

        readonly int bufferSize;
        readonly TimeSpan window;
        readonly DateTimeOffset startTime;
        readonly IScheduler scheduler;
        readonly Queue<TimeInterval<T>> queue = new Queue<TimeInterval<T>>();

        readonly object gate = new object();

        public ReplaySubject()
            : this(int.MaxValue, TimeSpan.MaxValue, Scheduler.CurrentThread)
        {
        }

        public ReplaySubject(IScheduler scheduler)
            : this(int.MaxValue, TimeSpan.MaxValue, scheduler)
        {
        }

        public ReplaySubject(int bufferSize)
            : this(bufferSize, TimeSpan.MaxValue, Scheduler.CurrentThread)
        {
        }

        public ReplaySubject(int bufferSize, IScheduler scheduler)
            : this(bufferSize, TimeSpan.MaxValue, scheduler)
        {
        }

        public ReplaySubject(TimeSpan window)
            : this(int.MaxValue, window, Scheduler.CurrentThread)
        {
        }

        public ReplaySubject(TimeSpan window, IScheduler scheduler)
            : this(int.MaxValue, window, scheduler)
        {
        }

        // full constructor
        public ReplaySubject(int bufferSize, TimeSpan window, IScheduler scheduler)
        {
            if (bufferSize < 0) throw new ArgumentOutOfRangeException("bufferSize");
            if (window < TimeSpan.Zero) throw new ArgumentOutOfRangeException("window");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            this.bufferSize = bufferSize;
            this.window = window;
            this.scheduler = scheduler;
            startTime = scheduler.Now;
        }

        void Trim()
        {
            var elapsedTime = Scheduler.Normalize(scheduler.Now - startTime);

            while (queue.Count > bufferSize)
            {
                queue.Dequeue();
            }
            while (queue.Count > 0 && elapsedTime.Subtract(queue.Peek().Interval).CompareTo(window) > 0)
            {
                queue.Dequeue();
            }
        }

        public void OnCompleted()
        {
            lock (gate)
            {
                if (isStopped) return;

                isStopped = true;
                Trim();

                foreach (var item in observers.ToArray())
                {
                    item.OnCompleted();
                }
                observers.Clear();
            }
        }

        public void OnError(Exception error)
        {
            lock (gate)
            {
                if (isStopped) return;

                isStopped = true;
                lastError = error;
                Trim();

                foreach (var item in observers.ToArray())
                {
                    item.OnError(error);
                }
                observers.Clear();
            }
        }

        public void OnNext(T value)
        {
            lock (gate)
            {
                if (isStopped) return;

                // enQ
                queue.Enqueue(new TimeInterval<T>(value, Scheduler.Now - startTime));
                Trim();

                foreach (var item in observers.ToArray())
                {
                    item.OnNext(value);
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (gate)
            {
                Trim();
                foreach (var item in queue)
                {
                    observer.OnNext(item.Value);
                }

                if (lastError != null)
                {
                    observer.OnError(lastError);
                    return Disposable.Empty;
                }
                else if (isStopped)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                observers.Add(observer);
            }
            return Disposable.Create(() =>
            {
                lock (gate)
                {
                    observers.Remove(observer);
                }
            });
        }
    }
}