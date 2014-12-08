using System;
using UniRx.InternalUtil;

namespace UniRx
{
    public sealed class BehaviorSubject<T> : ISubject<T>, IDisposable
    {
        object observerLock = new object();

        bool isStopped;
        bool isDisposed;
        T lastValue;
        Exception lastError;
        IObserver<T> outObserver = new EmptyObserver<T>();

        public BehaviorSubject(T defaultValue)
        {
            lastValue = defaultValue;
        }

        public T Value
        {
            get
            {
                ThrowIfDisposed();
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

        public void OnCompleted()
        {
            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = new EmptyObserver<T>();
                isStopped = true;
            }

            old.OnCompleted();
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
                outObserver = new EmptyObserver<T>();
                isStopped = true;
                lastError = error;
            }

            old.OnError(error);
        }

        public void OnNext(T value)
        {
            IObserver<T> current;
            lock (observerLock)
            {
                if (isStopped) return;

                lastValue = value;
                current = outObserver;
            }

            current.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            var ex = default(Exception);
            var v = default(T);
            var subscription = default(Subscription);

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

                    v = lastValue;
                    subscription = new Subscription(this, observer);
                }
                else
                {
                    ex = lastError;
                }
            }

            if (subscription != null)
            {
                observer.OnNext(v);
                return subscription;
            }
            else if (ex != null)
            {
                observer.OnError(ex);
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
                outObserver = new DisposedObserver<T>();
                lastError = null;
                lastValue = default(T);
            }
        }

        void ThrowIfDisposed()
        {
            if (isDisposed) throw new ObjectDisposedException("");
        }

        class Subscription : IDisposable
        {
            readonly object gate = new object();
            BehaviorSubject<T> parent;
            IObserver<T> unsubscribeTarget;

            public Subscription(BehaviorSubject<T> parent, IObserver<T> unsubscribeTarget)
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
                                parent.outObserver = new EmptyObserver<T>();
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