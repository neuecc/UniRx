using System;
using System.Reactive.Disposables;
using System.Threading;

namespace UniRx
{
    public static class CancelableExtenstions
    {
        public static CancellationToken ToCancellationToken(this ICancelable cancelable)
        {
            var cts = new CancellationTokenSource();
            IDisposable disposable = null;
            disposable = cancelable.ObserveEveryValueChanged(x => x.IsDisposed).Subscribe(d =>
            {
                if (d)
                {
                    cts.Cancel();
                    disposable.Dispose();
                }
            });
            return cts.Token;
        }
    }
}
