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
}