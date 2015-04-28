using System;
using System.Collections.Generic;

namespace UniRx
{
    public interface IMessageBroker
    {
        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        void Publish<T>(T message);

        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IObservable<T> Receive<T>();
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class MessageBroker : IMessageBroker, IDisposable
    {
        /// <summary>
        /// MessageBroker in Global scope.
        /// </summary>
        public static readonly IMessageBroker Default = new MessageBroker();

        bool isDisposed = false;
        readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();

        public void Publish<T>(T message)
        {
            object notifier;
            lock (notifiers)
            {
                if (isDisposed) return;

                if (!notifiers.TryGetValue(typeof(T), out notifier))
                {
                    return;
                }
            }
            ((Subject<T>)notifier).OnNext(message);
        }

        public IObservable<T> Receive<T>()
        {
            object notifier;
            lock (notifiers)
            {
                if (isDisposed) throw new ObjectDisposedException("MessageBroker");

                if (!notifiers.TryGetValue(typeof(T), out notifier))
                {
                    notifier = new Subject<T>();
                    notifiers.Add(typeof(T), notifier);
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
}