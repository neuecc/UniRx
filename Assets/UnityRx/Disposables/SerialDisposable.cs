using System;
using System.Collections;

namespace UnityRx
{
    public class SerialDisposable : IDisposable
    {
        IDisposable disposable;

        public bool IsDisposed { get; private set; }

        public IDisposable Disposable
        {
            get
            {
                return disposable;
            }
            set
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                disposable = value;
                if (IsDisposed && disposable != null)
                {
                    disposable.Dispose();

                }
            }
        }


        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}