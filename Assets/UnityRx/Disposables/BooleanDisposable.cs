using System;
using System.Collections;

namespace UnityRx
{
    public class BooleanDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed) IsDisposed = true;
        }
    }
}