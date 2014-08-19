// warning disable for Unity4.5's WWW(dictionary)
#pragma warning disable 612, 618

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
        public static IObservable<WWW> GetWWW(string url, Hash headers = null, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, null, (headers ?? new Hash())), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, byte[] postData, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, postData), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, postData, headers), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            var contentHeaders = (Hash)(object)content.headers;
            return Observable.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, postData), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, postData, headers), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            var contentHeaders = (Hash)(object)content.headers;
            return Observable.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, byte[] postData, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, postData), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, postData, headers), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            var contentHeaders = (Hash)(object)content.headers;
            return Observable.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
        }

        static Hash MergeHash(Hash source1, Hash source2)
        {
            foreach (HashEntry item in source2)
            {
                source1.Add(item.Key, item.Value);
            }
            return source1;
        }

        static IEnumerator Fetch(WWW www, IObserver<WWW> observer, IProgress<float> reportProgress, CancellationToken cancel)
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

                if (!string.IsNullOrEmpty(www.error))
                {
                    observer.OnError(new WWWErrorException(www));
                }
                else
                {
                    observer.OnNext(www);
                    observer.OnCompleted();
                }
            }
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

                if (!string.IsNullOrEmpty(www.error))
                {
                    observer.OnError(new WWWErrorException(www));
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

                if (!string.IsNullOrEmpty(www.error))
                {
                    observer.OnError(new WWWErrorException(www));
                }
                else
                {
                    observer.OnNext(www.bytes);
                    observer.OnCompleted();
                }
            }
        }
    }

    public class WWWErrorException : Exception
    {
        public string RawErrorMessage { get; private set; }
        public bool HasResponse { get; private set; }
        public System.Net.HttpStatusCode StatusCode { get; private set; }
        public System.Collections.Generic.Dictionary<string, string> ResponseHeaders { get; private set; }
        public WWW WWW { get; private set; }

        public WWWErrorException(WWW www)
        {
            this.WWW = www;
            this.RawErrorMessage = www.error;
            this.ResponseHeaders = www.responseHeaders;
            this.HasResponse = false;

            var splitted = RawErrorMessage.Split(' ');
            if (splitted.Length != 0)
            {
                int statusCode;
                if (int.TryParse(splitted[0], out statusCode))
                {
                    this.HasResponse = true;
                    this.StatusCode = (System.Net.HttpStatusCode)statusCode;
                }
            }
        }

        public override string ToString()
        {
            return RawErrorMessage;
        }
    }
}

#pragma warning restore 612, 618