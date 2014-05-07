using System;
using System.Collections;

namespace UniRx
{
    public static class Disposable
    {
        public static readonly IDisposable Empty = EmptyDisposable.Singleton;

        public static IDisposable Create(Action disposeAction)
        {
            return new AnonymousDisposable(disposeAction);
        }

        class EmptyDisposable : IDisposable
        {
            public static EmptyDisposable Singleton = new EmptyDisposable();

            private EmptyDisposable()
            {

            }

            public void Dispose()
            {
            }
        }

        class AnonymousDisposable : IDisposable
        {
            bool isDisposed = false;
            readonly Action dispose;

            public AnonymousDisposable(Action dispose)
            {
                this.dispose = dispose;
            }

            public void Dispose()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    dispose();
                }
            }
        }
    }
}