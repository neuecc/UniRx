using System;
using System.Collections;
using UnityEngine;

namespace UniRx
{
    public static partial class AsyncOperationExtensions
    {
        /// <summary>
        /// If you needs return value, use AsAsyncOperationObservable instead.
        /// </summary>
        public static IObservable<AsyncOperation> AsObservable(this AsyncOperation asyncOperation, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<AsyncOperation>((observer, cancellation) => AsObservableCore(asyncOperation, observer, progress, cancellation));
        }

        // T: where T : AsyncOperation is ambigious with IObservable<T>.AsObservable
        public static IObservable<T> AsAsyncOperationObservable<T>(this T asyncOperation, IProgress<float> progress = null)
            where T : AsyncOperation
        {
            return Observable.FromCoroutine<T>((observer, cancellation) => AsObservableCore(asyncOperation, observer, progress, cancellation));
        }

        static IEnumerator AsObservableCore<T>(T asyncOperation, IObserver<T> observer, IProgress<float> reportProgress, CancellationToken cancel)
            where T : AsyncOperation
        {
            while (!asyncOperation.isDone && !cancel.IsCancellationRequested)
            {
                if (reportProgress != null)
                {
                    try
                    {
                        reportProgress.Report(asyncOperation.progress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                }
                yield return null;
            }

            if (cancel.IsCancellationRequested) yield break;

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(asyncOperation.progress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }

            observer.OnNext(asyncOperation);
            observer.OnCompleted();
        }
    }
}