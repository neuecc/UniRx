// defined from .NET Framework 4.0 and NETFX_CORE

using System;

namespace UniRx
{
    public interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}