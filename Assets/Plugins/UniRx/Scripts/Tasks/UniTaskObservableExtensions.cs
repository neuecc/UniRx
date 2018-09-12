#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UniRx.Async;

namespace UniRx
{
    public static class UniTaskObservableExtensions
    {
        public static UniTask<T> ToUniTask<T>(this IObservable<T> source, CancellationToken cancellationToken = default(CancellationToken), bool useFirstValue = false)
        {
            var promise = new UniTaskCompletionSource<T>();
            var disposable = new SingleAssignmentDisposable();

            var observer = useFirstValue
                ? (IObserver<T>)new FirstValueToUniTaskObserver<T>(promise, disposable, cancellationToken)
                : (IObserver<T>)new ToUniTaskObserver<T>(promise, disposable, cancellationToken);

            try
            {
                disposable.Disposable = source.Subscribe(observer);
            }
            catch (Exception ex)
            {
                promise.TrySetException(ex);
            }

            return promise.Task;
        }

        public static IObservable<T> ToObservable<T>(this UniTask<T> task)
        {
            if (task.IsCompleted)
            {
                try
                {
                    return Observable.Return<T>(task.GetAwaiter().GetResult());
                }
                catch (Exception ex)
                {
                    return Observable.Throw<T>(ex);
                }
            }

            var subject = new AsyncSubject<T>();
            Fire(subject, task).Forget();
            return subject;
        }

        public static IObservable<Unit> ToObservable(this UniTask task)
        {
            if (task.IsCompleted)
            {
                try
                {
                    return Observable.ReturnUnit();
                }
                catch (Exception ex)
                {
                    return Observable.Throw<Unit>(ex);
                }
            }

            var subject = new AsyncSubject<Unit>();
            Fire(subject, task).Forget();
            return subject;
        }

        static async UniTaskVoid Fire<T>(AsyncSubject<T> subject, UniTask<T> task)
        {
            try
            {
                var value = await task;
                subject.OnNext(value);
                subject.OnCompleted();
            }
            catch (Exception ex)
            {
                subject.OnError(ex);
            }
        }

        static async UniTaskVoid Fire(AsyncSubject<Unit> subject, UniTask task)
        {
            try
            {
                await task;
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            }
            catch (Exception ex)
            {
                subject.OnError(ex);
            }
        }

        class ToUniTaskObserver<T> : IObserver<T>
        {
            static readonly Action<object> callback = OnCanceled;

            readonly UniTaskCompletionSource<T> promise;
            readonly SingleAssignmentDisposable disposable;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration registration;

            bool hasValue;
            T latestValue;

            public ToUniTaskObserver(UniTaskCompletionSource<T> promise, SingleAssignmentDisposable disposable, CancellationToken cancellationToken)
            {
                this.promise = promise;
                this.disposable = disposable;
                this.cancellationToken = cancellationToken;

                if (this.cancellationToken.CanBeCanceled)
                {
                    this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(callback, this);
                }
            }

            static void OnCanceled(object state)
            {
                var self = (ToUniTaskObserver<T>)state;
                self.disposable.Dispose();
                self.promise.TrySetCanceled();
            }

            public void OnNext(T value)
            {
                hasValue = true;
                latestValue = value;
            }

            public void OnError(Exception error)
            {
                try
                {
                    promise.TrySetException(error);
                }
                finally
                {
                    registration.Dispose();
                    disposable.Dispose();
                }
            }

            public void OnCompleted()
            {
                try
                {
                    if (hasValue)
                    {
                        promise.TrySetResult(latestValue);
                    }
                    else
                    {
                        promise.TrySetException(new InvalidOperationException("Sequence has no elements"));
                    }
                }
                finally
                {
                    registration.Dispose();
                    disposable.Dispose();
                }
            }
        }

        class FirstValueToUniTaskObserver<T> : IObserver<T>
        {
            static readonly Action<object> callback = OnCanceled;

            readonly UniTaskCompletionSource<T> promise;
            readonly SingleAssignmentDisposable disposable;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration registration;

            bool hasValue;

            public FirstValueToUniTaskObserver(UniTaskCompletionSource<T> promise, SingleAssignmentDisposable disposable, CancellationToken cancellationToken)
            {
                this.promise = promise;
                this.disposable = disposable;
                this.cancellationToken = cancellationToken;

                if (this.cancellationToken.CanBeCanceled)
                {
                    this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(callback, this);
                }
            }

            static void OnCanceled(object state)
            {
                var self = (FirstValueToUniTaskObserver<T>)state;
                self.disposable.Dispose();
                self.promise.TrySetCanceled();
            }

            public void OnNext(T value)
            {
                hasValue = true;
                try
                {
                    promise.TrySetResult(value);
                }
                finally
                {
                    registration.Dispose();
                    disposable.Dispose();
                }
            }

            public void OnError(Exception error)
            {
                try
                {
                    promise.TrySetException(error);
                }
                finally
                {
                    registration.Dispose();
                    disposable.Dispose();
                }
            }

            public void OnCompleted()
            {
                try
                {
                    if (!hasValue)
                    {
                        promise.TrySetException(new InvalidOperationException("Sequence has no elements"));
                    }
                }
                finally
                {
                    registration.Dispose();
                    disposable.Dispose();
                }
            }
        }
    }
}

#endif