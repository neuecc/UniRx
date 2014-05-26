using System;
using System.Collections;
using UnityEngine;

namespace UniRx
{
#if !(UNITY_METRO || UNITY_WP8)
    using Hash = System.Collections.Hashtable;
    using HashEntry = System.Collections.DictionaryEntry;
#else
    using Hash = System.Collections.Generic.Dictionary<string, string>;
    using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
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

        public static IObservable<string> Post(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, content.data, MergeHash(content.headers, headers)), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content.data, MergeHash(content.headers, headers)), observer, progress, cancellation));
        }

        static Hash MergeHash(Hash source1, Hash source2)
        {
            foreach (HashEntry item in source2)
            {
                source1.Add(item.Key, item.Value);
            }
            return source1;
        }

        static IEnumerator FetchText(WWW www, IObserver<string> observer, IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    if (reportProgress != null)
                    {
                        try
                        {
                            reportProgress.Report(www.progress);
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
                    if (reportProgress != null)
                    {
                        try
                        {
                            reportProgress.Report(www.progress);
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