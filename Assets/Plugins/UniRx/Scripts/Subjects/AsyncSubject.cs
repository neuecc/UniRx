using System;
using System.Collections.Generic;
using UniRx.InternalUtil;

namespace UniRx
{
    public sealed class AsyncSubject<T> : ISubject<T>, IOptimizedObservable<T>, IDisposable
    {
        object observerLock = new object();

        T lastValue;
        bool hasValue;
        bool isStopped;
        bool isDisposed;
        Exception lastError;
        IObserver<T> outObserver = EmptyObserver<T>.Instance;

        public T Value
        {
            get
            {
                ThrowIfDisposed();
                if (!isStopped) throw new InvalidOperationException("AsyncSubject is not completed yet");
                if (lastError != null) throw lastError;
                return lastValue;
            }
        }

        public bool HasObservers
        {
            get
            {
                return !(outObserver is EmptyObserver<T>) && !isStopped && !isDisposed;
            }
        }

        public bool IsCompleted { get { return isStopped; } }

        public void OnCompleted()
        {
            IObserver<T> old;
            T v;
            bool hv;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                v = lastValue;
                hv = hasValue;
            }

            if (hv)
            {
                old.OnNext(v);
                old.OnCompleted();
            }
            else
            {
                old.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            if (error == null) throw new ArgumentNullException("error");

            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                lastError = error;
            }

            old.OnError(error);
        }

        public void OnNext(T value)
        {
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                this.hasValue = true;
                this.lastValue = value;
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            var ex = default(Exception);
            var v = default(T);
            var hv = false;

            lock (observerLock)
            {
                ThrowIfDisposed();
                if (!isStopped)
                {
                    var listObserver = outObserver as ListObserver<T>;
                    if (listObserver != null)
                    {
                        outObserver = listObserver.Add(observer);
                    }
                    else
                    {
                        var current = outObserver;
                        if (current is EmptyObserver<T>)
                        {
                            outObserver = observer;
                        }
                        else
                        {
                            outObserver = new ListObserver<T>(new ImmutableList<IObserver<T>>(new[] { current, observer }));
                        }
                    }

                    return new Subscription(this, observer);
                }

                ex = lastError;
                v = lastValue;
                hv = hasValue;
            }

            if (ex != null)
            {
                observer.OnError(ex);
            }
            else if (hv)
            {
                observer.OnNext(v);
                observer.OnCompleted();
            }
            else
            {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        }

        public void Dispose()
        {
            lock (observerLock)
            {
                isDisposed = true;
                outObserver = DisposedObserver<T>.Instance;
                lastError = null;
                lastValue = default(T);
            }
        }

        void ThrowIfDisposed()
        {
            if (isDisposed) throw new ObjectDisposedException("");
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        class Subscription : IDisposable
        {
            readonly object gate = new object();
            AsyncSubject<T> parent;
            IObserver<T> unsubscribeTarget;

            public Subscription(AsyncSubject<T> parent, IObserver<T> unsubscribeTarget)
            {
                this.parent = parent;
                this.unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose()
            {
                lock (gate)
                {
                    if (parent != null)
                    {
                        lock (parent.observerLock)
                        {
                            var listObserver = parent.outObserver as ListObserver<T>;
                            if (listObserver != null)
                            {
                                parent.outObserver = listObserver.Remove(unsubscribeTarget);
                            }
                            else
                            {
                                parent.outObserver = EmptyObserver<T>.Instance;
                            }

                            unsubscribeTarget = null;
                            parent = null;
                        }
                    }
                }
            }
        }
    }
}
