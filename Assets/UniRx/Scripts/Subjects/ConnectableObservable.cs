using System;
using System.Collections.Generic;
using System.Text;

#if SystemReactive
namespace System.Reactive.Subjects
#else
namespace UniRx
#endif
{
    public interface IConnectableObservable<T> : IObservable<T>
    {
        IDisposable Connect();
    }

#if SystemReactive
}

namespace System.Reactive.Linq
{
    using Subjects;

#endif
    public static partial class Observable
    {
        class ConnectableObservable<T> : IConnectableObservable<T>
        {
            readonly IObservable<T> source;
            readonly ISubject<T> subject;

            public ConnectableObservable(IObservable<T> source, ISubject<T> subject)
            {
                this.source = source;
                this.subject = subject;
            }

            public IDisposable Connect()
            {
                var subscription = source.Subscribe(subject);
                return subscription;
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return subject.Subscribe(observer);
            }
        }
    }
}