// defined from .NET Framework 4.0 and NETFX_CORE

using System;

#if !(NETFX_CORE || NET_4_6)

namespace UniRx
{
    public interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}

#endif

namespace UniRx
{
    public interface IGroupedObservable<TKey, TElement> : IObservable<TElement>
    {
        TKey Key { get; }
    }
}