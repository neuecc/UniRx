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
        Exception lastError;
        List<IObserver<T>> observers = new List<IObserver<T>>();

        public void OnCompleted()
        {
            if (isStopped) return;

            isStopped = true;
            foreach (var item in observers.ToArray())
            {
                item.OnCompleted();
            }
            observers.Clear();
        }

        public void OnError(Exception error)
        {
            if (isStopped) return;

            isStopped = true;
            lastError = error;
            foreach (var item in observers.ToArray())
            {
                item.OnError(error);
            }
            observers.Clear();
        }

        public void OnNext(T value)
        {
            if (isStopped) return;

            foreach (var item in observers.ToArray())
            {
                item.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!isStopped)
            {
                observers.Add(observer);

                return Disposable.Create(() => observers.Remove(observer));
            }
            else if (lastError != null)
            {
                observer.OnError(lastError);
                return Disposable.Empty;
            }
            else
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
        }
    }
}