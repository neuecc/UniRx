using System;
using System.Threading;

namespace UniRx.Operators
{
    internal abstract class OperatorObserverBase<TSource, TResult> : IDisposable, IObserver<TSource>, ISafeObserver
    {
        protected internal volatile IObserver<TResult> observer;
        IDisposable cancel;

        public OperatorObserverBase(IObserver<TResult> observer, IDisposable cancel)
        {
            this.observer = observer;
            this.cancel = cancel;
        }

        public abstract void OnNext(TSource value);

        public virtual void OnError(Exception error)
        {
            observer.OnError(error);
            Dispose();
        }

        public virtual void OnCompleted()
        {
            observer.OnCompleted();
            Dispose();
        }

        public void Dispose()
        {
            observer = new UniRx.InternalUtil.EmptyObserver<TResult>();
            var target = System.Threading.Interlocked.Exchange(ref cancel, null);
            if (target != null)
            {
                target.Dispose();
            }
        }
    }

    internal abstract class AutoDetachOperatorObserverBase<T> : IObserver<T>, IDisposable, ISafeObserver
    {
        protected internal volatile IObserver<T> observer;
        IDisposable cancel;

        int isStopped = 0;

        public AutoDetachOperatorObserverBase(IObserver<T> observer, IDisposable cancel)
        {
            this.observer = observer;
            this.cancel = cancel;
        }

        public void OnNext(T value)
        {
            try
            {
                this.observer.OnNext(value);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void OnError(Exception error)
        {
            if (Interlocked.Increment(ref isStopped) == 1)
            {
                try
                {
                    this.observer.OnError(error);
                }
                finally
                {
                    Dispose();
                }
            }
        }

        public void OnCompleted()
        {
            if (Interlocked.Increment(ref isStopped) == 1)
            {
                try
                {
                    this.observer.OnCompleted();
                }
                finally
                {
                    Dispose();
                }
            }
        }

        public void Dispose()
        {
            observer = new UniRx.InternalUtil.EmptyObserver<T>();
            var target = System.Threading.Interlocked.Exchange(ref cancel, null);
            if (target != null)
            {
                target.Dispose();
            }
        }
    }
}