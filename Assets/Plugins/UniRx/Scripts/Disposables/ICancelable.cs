using System;

namespace UniRx
{
    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
