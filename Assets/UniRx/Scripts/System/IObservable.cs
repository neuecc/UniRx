// defined from .NET Framework 4.0 and NETFX_CORE

#if !NETFX_CORE

#if SystemReactive
namespace System
#else
using System;

namespace UniRx
#endif
{
    public interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}

#endif