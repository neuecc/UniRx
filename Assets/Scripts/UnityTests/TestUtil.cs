using System;
using System.Collections.Generic;

namespace UniRx.Tests
{
    public static class TestUtil
    {
        public static T[] ToArrayWait<T>(this IObservable<T> source)
        {
            return source.ToArray().Wait();
        }

        public static RecordObserver<T> Record<T>(this IObservable<T> source)
        {
            var d = new SingleAssignmentDisposable();
            var observer = new RecordObserver<T>(d);
            d.Disposable = source.Subscribe(observer);

            return observer;
        }

        public static void SetScehdulerForImport()
        {
            Scheduler.DefaultSchedulers.ConstantTimeOperations = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.TailRecursion = Scheduler.Immediate;
            Scheduler.DefaultSchedulers.Iteration = Scheduler.CurrentThread;
            Scheduler.DefaultSchedulers.TimeBasedOperations = Scheduler.ThreadPool;
            Scheduler.DefaultSchedulers.AsyncConversions = Scheduler.ThreadPool;
        }
    }

    public class RecordObserver<T> : IObserver<T>
    {
        readonly object gate = new object();
        readonly IDisposable subscription;

        public List<T> Values { get; set; }
        public List<Notification<T>> Notifications { get; set; }

        public RecordObserver(IDisposable subscription)
        {
            this.subscription = subscription;
            Values = new List<T>();
            Notifications = new List<Notification<T>>();
        }

        public void DisposeSubscription()
        {
            subscription.Dispose();
        }

        void IObserver<T>.OnNext(T value)
        {
            lock (gate)
            {
                Values.Add(value);
                Notifications.Add(Notification.CreateOnNext<T>(value));
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            lock (gate)
            {
                Notifications.Add(Notification.CreateOnError<T>(error));
            }
        }
        void IObserver<T>.OnCompleted()
        {
            lock (gate)
            {
                Notifications.Add(Notification.CreateOnCompleted<T>());
            }
        }
    }
}
