using System;
using System.Collections.Generic;
using System.Text;

#if SystemReactive
namespace System.Reactive.Disposables
#else
namespace UniRx
#endif
{
    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
