// defined from .NET Framework 4.0 and NETFX_CORE

#if !(NETFX_CORE || NET_4_6)

using System;

namespace UniRx
{
    public interface IObserver<T>
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(T value);
    }
}

#endif