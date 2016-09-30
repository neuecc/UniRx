#if !NETFX_CORE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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
            this.Values = new List<T>();
            this.Notifications = new List<Notification<T>>();
        }

        public void DisposeSubscription()
        {
            subscription.Dispose();
        }

        public void OnNext(T value)
        {
            lock (gate)
            {
                Values.Add(value);
                Notifications.Add(Notification.CreateOnNext<T>(value));
            }
        }

        public void OnError(Exception error)
        {
            lock (gate)
            {
                Notifications.Add(Notification.CreateOnError<T>(error));
            }
        }
        public void OnCompleted()
        {
            lock (gate)
            {
                Notifications.Add(Notification.CreateOnCompleted<T>());
            }
        }
    }

    class DecrementEnumerator : IEnumerator
    {
        readonly int original;
        int count;

        public DecrementEnumerator(int count)
        {
            this.original = count;
            this.count = count;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }
        public int OriginalCount { get { return original; } }

        public int Count { get { return count; } }

        public bool MoveNext()
        {
            return count-- > 0;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return count + "/" + original;
        }
    }

    public partial class MicroCoroutineTest
    {
        static UniRx.InternalUtil.MicroCoroutine Create()
        {
            return new InternalUtil.MicroCoroutine(ex => Console.WriteLine(ex));
        }

        static int FindLast(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var coroutines = mc.GetType().GetField("coroutines", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var enumerators = (IEnumerator[])coroutines.GetValue(mc);

            int tail = -1;
            for (int i = 0; i < enumerators.Length; i++)
            {
                if (enumerators[i] == null)
                {
                    if (tail == -1)
                    {
                        tail = i;
                    }
                }
                else
                {
                    if (tail != -1)
                    {
                        throw new Exception("what's happen?");
                    }
                }
            }

            if (tail == -1) tail = enumerators.Length;
            return tail;
        }

        static int GetTailDynamic(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var tail = mc.GetType().GetField("tail", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            return (int)tail.GetValue(mc);
        }
    }
}

#endif