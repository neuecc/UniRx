using System;
using System.Collections.Generic;
using System.Text;

#if SystemReactive
namespace System.Reactive.Subjects
#else
namespace UniRx
#endif
{
    public interface ISubject<TSource, TResult> : IObserver<TSource>, IObservable<TResult>
    {
    }

    public interface ISubject<T> : ISubject<T, T>, IObserver<T>, IObservable<T>
    {
    }
}