// defined from .NET Framework 4.0 and NETFX_CORE

#if !NETFX_CORE

#if SystemReactive
namespace System
#else
using System;

namespace UniRx
#endif
{
    public interface IObserver<T>
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(T value);
    }
}

#endif