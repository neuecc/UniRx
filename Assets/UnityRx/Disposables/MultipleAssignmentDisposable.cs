using System;
using System.Collections;

namespace UnityRx
{
    public class MultipleAssignmentDisposable : IDisposable
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