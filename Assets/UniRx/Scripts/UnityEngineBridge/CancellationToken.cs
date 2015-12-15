using System;

namespace UniRx
{
    public struct CancellationToken
    {
        readonly ICancelable source;

        public static CancellationToken Empty = new CancellationToken(null);

        public CancellationToken(ICancelable source)
        {
            this.source = source;
        }

        public bool IsCancellationRequested
        {
            get
            {
                return (source == null) ? true : source.IsDisposed;
            }
        }

        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
    }
}