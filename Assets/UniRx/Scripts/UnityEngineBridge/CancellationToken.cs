using System;

namespace UniRx
{
    public class CancellationToken
    {
        readonly ICancelable source;

        public static CancellationToken Empty = new CancellationToken(new BooleanDisposable());

        public CancellationToken(ICancelable source)
        {
            if (source == null) throw new ArgumentNullException("source");

            this.source = source;
        }

        public bool IsCancellationRequested
        {
            get
            {
                return source.IsDisposed;
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