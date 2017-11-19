#if !(NET_4_6)

using System;

namespace UniRx
{
    public struct CancellationToken
    {
        readonly ICancelable source;

        public static readonly CancellationToken Empty = new CancellationToken(null);
        
        /// <summary>Same as Empty.</summary>
        public static readonly CancellationToken None = new CancellationToken(null);

        public CancellationToken(ICancelable source)
        {
            this.source = source;
        }

        public bool IsCancellationRequested
        {
            get
            {
                return (source == null) ? false : source.IsDisposed;
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

#endif

