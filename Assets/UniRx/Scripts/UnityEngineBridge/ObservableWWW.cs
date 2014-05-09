using System;
using System.Collections;
using UnityEngine;

namespace UniRx
{
#if !(UNITY_METRO || UNITY_WP8)
    using Hash = System.Collections.Hashtable;
#else
    using Hash = System.Collections.Generic.Dictionary<string, string>;
#endif

    public static partial class ObservableWWW
    {
        public static IObservable<string> Get(string url, Hash headers = null, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, null, (headers ?? new Hash())), observer, progress, cancellation));
        }

        public static IObservable<byte[]> GetAndGetBytes(string url, Hash headers = null, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, null, (headers ?? new Hash())), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content), observer, progress, cancellation));
        }

        static IEnumerator FetchText(WWW www, IObserver<string> observer, IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    if (reportProgress != null) reportProgress.Report(www.progress);
                    yield return null;
                }

                if (cancel.IsCancellationRequested) yield break;

                if (www.error != null)
                {
                    observer.OnError(new Exception(www.error));
                }
                else
                {
                    observer.OnNext(www.text);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchBytes(WWW www, IObserver<byte[]> observer, IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    if (reportProgress != null) reportProgress.Report(www.progress);
                    yield return null;
                }

                if (cancel.IsCancellationRequested) yield break;

                if (www.error != null)
                {
                    observer.OnError(new Exception(www.error));
                }
                else
                {
                    observer.OnNext(www.bytes);
                    observer.OnCompleted();
                }
            }
        }
    }
}