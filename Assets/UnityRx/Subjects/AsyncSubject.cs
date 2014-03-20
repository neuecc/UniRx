using System;
using System.Collections.Generic;

namespace UnityRx
{
    // MEMO:should be threadsafe?

    public sealed class AsyncSubject<T> : ISubject<T>
    {
        bool isStopped;
        T lastValue;
        bool hasValue;
        Exception lastError;
        LinkedList<IObserver<T>> observers = new LinkedList<IObserver<T>>();

        public void OnCompleted()
        {
            if (isStopped) return;

            isStopped = true;
            if (hasValue)
            {
                foreach (var item in observers)
                {
                    item.OnNext(lastValue);
                    item.OnCompleted();
                }
            }
            else
            {
                foreach (var item in observers)
                {
                    item.OnCompleted();
                }
            }
            observers.Clear();
        }

        public void OnError(Exception error)
        {
            if (isStopped) return;

            isStopped = true;
            lastError = error;
            foreach (var item in observers)
            {
                item.OnError(error);
            }
            observers.Clear();
        }

        public void OnNext(T value)
        {
            if (isStopped) return;

            hasValue = true;
            lastValue = value;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!isStopped)
            {
                var node = new LinkedListNode<IObserver<T>>(observer);
                observers.AddLast(node);

                return Disposable.Create(() =>
                {
                    var list = node.List;
                    if (list != null) list.Remove(node);
                });
            }
            else
            {
                if (lastError != null)
                {
                    observer.OnError(lastError);
                }
                else if (hasValue)
                {
                    observer.OnNext(lastValue);
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnCompleted();
                }

                return Disposable.Empty;
            }
        }
    }
}