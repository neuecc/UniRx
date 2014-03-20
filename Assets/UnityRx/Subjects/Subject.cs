using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    // MEMO:should be threadsafe?

    public sealed class Subject<T> : ISubject<T>
    {
        bool isStopped;
        LinkedList<IObserver<T>> observers = new LinkedList<IObserver<T>>();

        public void OnCompleted()
        {
            if (isStopped) return;

            isStopped = true;
            foreach (var item in observers)
            {
                item.OnCompleted();
            }
            observers.Clear();
        }

        public void OnError(Exception error)
        {
            if (isStopped) return;

            isStopped = true;
            foreach (var item in observers)
            {
                item.OnError(error);
            }
            observers.Clear();
        }

        public void OnNext(T value)
        {
            if (isStopped) return;

            foreach (var item in observers)
            {
                item.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var node = new LinkedListNode<IObserver<T>>(observer);
            observers.AddLast(node);

            return Disposable.Create(() => observers.Remove(node));
        }
    }
}