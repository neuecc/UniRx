#if UNITY_5_3_OR_NEWER

using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.Toolkit
{
    /// <summary>
    /// Bass class of ObjectPool.
    /// </summary>
    public abstract class ObjectPool<T> : IDisposable
        where T : UnityEngine.Component
    {
        bool disposedValue = false;
        Queue<T> q;

        /// <summary>
        /// Create instance when needed.
        /// </summary>
        protected abstract T CreateInstance();

        /// <summary>
        /// Called before return to pool, useful for set active object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeRent(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called before return to pool, useful for set inactive object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeReturn(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when clear or disposed, useful for destroy instance or other finalize method.
        /// </summary>
        protected virtual void OnClear(T instance)
        {
            if (instance == null) return;

            var go = instance.gameObject;
            if (go == null) return;
            UnityEngine.Object.Destroy(go);
        }

        /// <summary>
        /// Current pooled object count.
        /// </summary>
        public int Count
        {
            get
            {
                if (q == null) return 0;
                return q.Count;
            }
        }

        /// <summary>
        /// Get from pool.
        /// </summary>
        public T Rent()
        {
            if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (q == null) q = new Queue<T>();

            var instance = (q.Count > 0)
                ? q.Dequeue()
                : CreateInstance();

            OnBeforeRent(instance);
            return instance;
        }

        /// <summary>
        /// Return to pool.
        /// </summary>
        public void Return(T instance)
        {
            if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (instance == null) throw new ArgumentNullException("instance");

            OnBeforeReturn(instance);

            if (q == null) q = new Queue<T>();

            q.Enqueue(instance);
        }

        /// <summary>
        /// Clear pool.
        /// </summary>
        public void Clear(bool callOnBeforeRent = false)
        {
            while (q.Count != 0)
            {
                var instance = q.Dequeue();
                if (callOnBeforeRent)
                {
                    OnBeforeRent(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// Fill pool before rent operation.
        /// </summary>
        /// <param name="preloadCount">Pool instance count.</param>
        /// <param name="threshold">Create count per frame.</param>
        public IObservable<Unit> PreloadAsync(int preloadCount, int threshold)
        {
            if (q == null) q = new Queue<T>(preloadCount);

            return Observable.FromMicroCoroutine<Unit>((observer, cancel) => PreloadCore(preloadCount, threshold, observer, cancel));
        }

        IEnumerator PreloadCore(int preloadCount, int threshold, IObserver<Unit> observer, CancellationToken cancellationToken)
        {
            while (Count < preloadCount && !cancellationToken.IsCancellationRequested)
            {
                var requireCount = preloadCount - Count;
                if (requireCount <= 0) break;

                var createCount = Math.Min(requireCount, threshold);

                for (int i = 0; i < createCount; i++)
                {
                    try
                    {
                        var instance = CreateInstance();
                        Return(instance);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }
                yield return null; // next frame.
            }

            observer.OnNext(Unit.Default);
            observer.OnCompleted();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear(false);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    /// <summary>
    /// Bass class of ObjectPool. If needs asynchronous initialization, use this instead of standard ObjectPool.
    /// </summary>
    public abstract class AsyncObjectPool<T> : IDisposable
        where T : UnityEngine.Component
    {
        bool disposedValue = false;
        Queue<T> q;

        /// <summary>
        /// Create instance when needed.
        /// </summary>
        protected abstract IObservable<T> CreateInstanceAsync();

        /// <summary>
        /// Called before return to pool, useful for set active object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeRent(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called before return to pool, useful for set inactive object(it is default behavior).
        /// </summary>
        protected virtual void OnBeforeReturn(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when clear or disposed, useful for destroy instance or other finalize method.
        /// </summary>
        protected virtual void OnClear(T instance)
        {
            if (instance == null) return;

            var go = instance.gameObject;
            if (go == null) return;
            UnityEngine.Object.Destroy(go);
        }

        /// <summary>
        /// Current pooled object count.
        /// </summary>
        public int Count
        {
            get
            {
                if (q == null) return 0;
                return q.Count;
            }
        }

        /// <summary>
        /// Get from pool.
        /// </summary>
        public IObservable<T> RentAsync()
        {
            if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (q == null) q = new Queue<T>();

            if (q.Count > 0)
            {
                var instance = q.Dequeue();
                OnBeforeRent(instance);
                return Observable.Return(instance);
            }
            else
            {
                var instance = CreateInstanceAsync();
                return instance.Do(x => OnBeforeRent(x));
            }
        }

        /// <summary>
        /// Return to pool.
        /// </summary>
        public void Return(T instance)
        {
            if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (instance == null) throw new ArgumentNullException("instance");
            if (q == null) q = new Queue<T>();

            OnBeforeReturn(instance);
            q.Enqueue(instance);
        }

        /// <summary>
        /// Clear pool.
        /// </summary>
        public void Clear(bool callOnBeforeRent = false)
        {
            while (q.Count != 0)
            {
                var instance = q.Dequeue();
                if (callOnBeforeRent)
                {
                    OnBeforeRent(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// Fill pool before rent operation.
        /// </summary>
        /// <param name="preloadCount">Pool instance count.</param>
        /// <param name="threshold">Create count per frame.</param>
        public IObservable<Unit> PreloadAsync(int preloadCount, int threshold)
        {
            if (q == null) q = new Queue<T>(preloadCount);

            return Observable.FromMicroCoroutine<Unit>((observer, cancel) => PreloadCore(preloadCount, threshold, observer, cancel));
        }

        IEnumerator PreloadCore(int preloadCount, int threshold, IObserver<Unit> observer, CancellationToken cancellationToken)
        {
            while (Count < preloadCount && !cancellationToken.IsCancellationRequested)
            {
                var requireCount = preloadCount - Count;
                if (requireCount <= 0) break;

                var createCount = Math.Min(requireCount, threshold);

                var loaders = new IObservable<Unit>[createCount];
                for (int i = 0; i < createCount; i++)
                {
                    var instanceFuture = CreateInstanceAsync();
                    loaders[i] = instanceFuture.ForEachAsync(x => Return(x));
                }

                var awaiter = Observable.WhenAll(loaders).ToYieldInstruction(false, cancellationToken);
                while (!(awaiter.HasResult || awaiter.IsCanceled || awaiter.HasError))
                {
                    yield return null;
                }

                if (awaiter.HasError)
                {
                    observer.OnError(awaiter.Error);
                    yield break;
                }
                else if (awaiter.IsCanceled)
                {
                    yield break; // end.
                }
            }

            observer.OnNext(Unit.Default);
            observer.OnCompleted();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear(false);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}

#endif