#if CSHARP_7_OR_LATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx.InternalUtil;

namespace UniRx
{
    public interface ITaggableMessagePublisher
    {
        /// <summary>
        /// Send Taggable Message to all receiver.
        /// </summary>
        void Publish<T>(T message, string tag);
    }

    public interface ITaggableMessageReceiver
    {
        /// <summary>
        /// Subscribe typed and taggable message.
        /// </summary>
        IObservable<T> Receive<T>(string tag);
    }

    public interface ITaggableMessageBroker : ITaggableMessagePublisher, ITaggableMessageReceiver
    {
    }

    public interface ITaggableAsyncMessagePublisher
    {
        /// <summary>
        /// Send Message to all tagged receiver and await complete.
        /// </summary>
        IObservable<Unit> PublishAsync<T>(T message, string tag = "");
    }

    public interface ITaggableAsyncMessageReceiver
    {
        /// <summary>
        /// Subscribe typed and taggable message.
        /// </summary>
        IDisposable Subscribe<T>(Func<T, IObservable<Unit>> asyncMessageReceiver, string tag = "");
    }

    public interface ITaggableAsyncMessageBroker : ITaggableAsyncMessagePublisher, ITaggableAsyncMessageReceiver
    {
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type and tag
    /// </summary>
    public class TaggableMessageBroker : ITaggableMessageBroker, IDisposable
    {
        /// <summary>
        /// TaggableMessageBroker in Global scope.
        /// </summary>
        public static readonly ITaggableMessageBroker Default = new TaggableMessageBroker();

        bool isDisposed = false;
        readonly Dictionary<ValueTuple<string, Type>, object> notifiers = new Dictionary<ValueTuple<string, Type>, object>();
        public void Publish<T>(T message, string tag = "")
        {
            object notifier;
            lock (notifiers)
            {
                if (isDisposed) return;

                if (!notifiers.TryGetValue(ValueTuple.Create(tag, typeof(T)), out notifier))
                {
                    return;
                }
            }
        ((ISubject<T>)notifier).OnNext(message);
        }

        public IObservable<T> Receive<T>(string tag = "")
        {
            object notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("MessageBroker");

                if (!notifiers.TryGetValue(ValueTuple.Create(tag, typeof(T)), out notifier))
                {
                    notifier = new Subject<T>().Synchronize();
                    notifiers.Add(ValueTuple.Create(tag, typeof(T)), notifier);
                }
            }
            return ((IObservable<T>)notifier).AsObservable();
        }
        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }
    }


    /// <summary>
    /// In-Memory PubSub filtered by Type and Tag.
    /// </summary>
    public class TaggableAsyncMessageBroker : ITaggableAsyncMessageBroker, IDisposable
    {
        /// <summary>
        /// TaggableAsyncMessageBroker in Global scope.
        /// </summary>
        public static readonly ITaggableAsyncMessageBroker Default = new TaggableAsyncMessageBroker();
        bool isDisposed = false;
        readonly Dictionary<ValueTuple<string, Type>, object> notifiers = new Dictionary<ValueTuple<string, Type>, object>();

        public IObservable<Unit> PublishAsync<T>(T message, string tag = "")
        {
            UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>> notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                var v = ValueTuple.Create(tag, typeof(T));
                if (notifiers.TryGetValue(v, out _notifier))
                {
                    notifier = (UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>>)_notifier;
                }
                else
                {
                    return Observable.ReturnUnit();
                }
            }
            var data = notifier.Data;
            var awaiter = new IObservable<Unit>[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                awaiter[i] = data[i].Invoke(message);
            }
            return Observable.WhenAll(awaiter);
        }

        public IDisposable Subscribe<T>(Func<T, IObservable<Unit>> asyncMessageReceiver, string tag = "")
        {

            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("AsyncMessageBroker");

                object _notifier;
                var t = ValueTuple.Create(tag, typeof(T));
                if (!notifiers.TryGetValue(t, out _notifier))
                {
                    var notifier = UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>>.Empty;
                    notifier = notifier.Add(asyncMessageReceiver);
                    notifiers.Add(t, notifier);
                }
                else
                {
                    var notifier = (ImmutableList<Func<T, IObservable<Unit>>>)_notifier;
                    notifier = notifier.Add(asyncMessageReceiver);
                    notifiers[t] = notifier;
                }
            }
            return new Subscription<T>(this, asyncMessageReceiver, tag);
        }

        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }

        class Subscription<T> : IDisposable
        {
            readonly TaggableAsyncMessageBroker parent;
            readonly Func<T, IObservable<Unit>> asyncMessageReceiver;
            readonly string tag;
            public Subscription(TaggableAsyncMessageBroker parent, Func<T, IObservable<Unit>> asyncMessageReceiver, string tag)
            {
                this.parent = parent;
                this.asyncMessageReceiver = asyncMessageReceiver;
                this.tag = tag;
            }

            public void Dispose()
            {
                lock (parent.notifiers)
                {
                    object _notifier;
                    var t = ValueTuple.Create(tag, typeof(T));
                    if (parent.notifiers.TryGetValue(t, out _notifier))
                    {
                        var notifier = (ImmutableList<Func<T, IObservable<Unit>>>)_notifier;
                        notifier = notifier.Remove(asyncMessageReceiver);
                        parent.notifiers[t] = notifier;
                    }
                }
            }
        }
    }
}
#endif
