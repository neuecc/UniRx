using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public interface ISubject<in TSource, out TResult> : IObserver<TSource>, IObservable<TResult>
    {
    }

    public interface ISubject<T> : ISubject<T, T>, IObserver<T>, IObservable<T>
    {
    }

    // TODO:need error handling

    public sealed class Subject<T> : ISubject<T>
    {
        LinkedList<IObserver<T>> observers = new LinkedList<IObserver<T>>();

        public void OnCompleted()
        {
            foreach (var item in observers)
            {
                item.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var item in observers)
            {
                item.OnError(error);
            }
        }

        public void OnNext(T value)
        {
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

    // TODO:AsyncSubject, Behavior
}