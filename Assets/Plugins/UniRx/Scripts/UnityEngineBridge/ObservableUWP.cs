#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using Hash = System.Collections.Generic.Dictionary<string, string>;
using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
using System.Threading;

public static partial class ObservableUWP
{
    public static IObservable<string> Get(string url, Hash headers = null, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "GET", null, (headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<byte[]> GetAndGetBytes(string url, Hash headers = null, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "GET", null, (headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<UnityWebRequest> GetWWW(string url, Hash headers = null, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(GetUWP(url, "GET", null, (headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<string> Post(string url, byte[] postData, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "POST", postData, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<string> Post(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "POST", postData, (headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "POST", content.data, content.headers ?? new Hash()), observer, progress, cancellation));
    }

    public static IObservable<string> Post(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
    {
        var contentHeaders = content.headers;
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "POST", content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
    }

    public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "POST", postData, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "POST", postData, (headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "POST", content.data, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
    {
        var contentHeaders = content.headers;
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "POST", content.data, MergeHash(contentHeaders, (headers ?? new Hash()))), observer, progress, cancellation));
    }

    public static IObservable<UnityWebRequest> PostWWW(string url, byte[] postData, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(GetUWP(url, "POST", postData, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<UnityWebRequest> PostWWW(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(GetUWP(url, "POST", postData, headers ?? new Hash()), observer, progress, cancellation));
    }

    public static IObservable<UnityWebRequest> PostWWW(string url, WWWForm content, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(GetUWP(url, "POST", content.data, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<UnityWebRequest> PostWWW(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
    {
        var contentHeaders = content.headers;
        return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(GetUWP(url, "POST", content.data, MergeHash(contentHeaders, headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, uint version, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) => FetchAssetBundle(UnityWebRequest.GetAssetBundle(url, version), observer, progress, cancellation));
    }

    public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, uint version, uint crc, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) => FetchAssetBundle(UnityWebRequest.GetAssetBundle(url, version, crc), observer, progress, cancellation));
    }

    public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, Hash128 hash128, uint crc, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) => FetchAssetBundle(UnityWebRequest.GetAssetBundle(url, hash128, crc), observer, progress, cancellation));
    }

    public static IObservable<string> Put(string url, byte[] putData, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "PUT", putData, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<string> Put(string url, byte[] putData, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "PUT", putData, headers ?? new Hash()), observer, progress, cancellation));
    }

    public static IObservable<byte[]> PutAndGetBytes(string url, byte[] postData, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "PUT", postData, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<byte[]> PutAndGetBytes(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(GetUWP(url, "PUT", postData, (headers ?? new Hash())), observer, progress, cancellation));
    }

    public static IObservable<string> Head(string url, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "HEAD", null, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<string> Head(string url, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "HEAD", null, headers ?? new Hash()), observer, progress, cancellation));
    }

    public static IObservable<string> Delete(string url, byte[] content, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "DELETE", content, new Hash()), observer, progress, cancellation));
    }

    public static IObservable<string> Delete(string url, byte[] content, Hash headers, IProgress<float> progress = null)
    {
        return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(GetUWP(url, "DELETE", content, headers ?? new Hash()), observer, progress, cancellation));
    }

    private static UnityWebRequest GetUWP(string url, string method, byte[] postData, Hash headers)
    {
        UnityWebRequest uwp = null;
        switch (method)
        {
            case ("GET"):
                {
                    uwp = UnityWebRequest.Get(url);
                    break;
                }
            case ("POST"):
                {
                    uwp = UnityWebRequest.Post(url, "");
                    break;
                }
            case ("PUT"):
                {
                    uwp = UnityWebRequest.Put(url, postData);
                    break;
                }
            case ("HEAD"):
                {
                    uwp = UnityWebRequest.Head(url);
                    break;
                }
            case ("DELETE"):
                {
                    uwp = UnityWebRequest.Delete(url);
                    break;
                }
            default:
                {
                    Debug.LogError("Unknown method: " + method);
                    break;
                }
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                uwp.SetRequestHeader(header.Key, header.Value);
            }
        }
        if (postData != null)
        {
            UploadHandler uploader = new UploadHandlerRaw(postData);
            uwp.uploadHandler = uploader;
        }
        return uwp;
    }

    static IEnumerator FetchText(UnityWebRequest www, IObserver<string> observer, IProgress<float> reportProgress, CancellationToken cancel)
    {
        using (www)
        {
            if (reportProgress != null)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    try
                    {
                        reportProgress.Report(www.downloadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return www.Send();
                }
            }
            else
            {
                if (!www.isDone)
                {
                    yield return www.Send();
                }
            }

            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(www.downloadProgress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }

            if (www.isNetworkError || www.isHttpError)
            {
                observer.OnError(new UnityWebRequestErrorException(www, www.downloadHandler.text));
            }
            else
            {
                observer.OnNext(www.downloadHandler.text);
                observer.OnCompleted();
            }
        }
    }

    static IEnumerator FetchBytes(UnityWebRequest www, IObserver<byte[]> observer, IProgress<float> reportProgress, CancellationToken cancel)
    {
        using (www)
        {
            if (reportProgress != null)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    try
                    {
                        reportProgress.Report(www.downloadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return www.Send();
                }
            }
            else
            {
                if (!www.isDone)
                {
                    yield return www;
                }
            }

            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(www.downloadProgress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                observer.OnError(new UnityWebRequestErrorException(www, www.downloadHandler.text));
            }
            else
            {
                observer.OnNext(www.downloadHandler.data);
                observer.OnCompleted();
            }
        }
    }

    static IEnumerator Fetch(UnityWebRequest www, IObserver<UnityWebRequest> observer, IProgress<float> reportProgress, CancellationToken cancel)
    {
        using (www)
        {
            if (reportProgress != null)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    try
                    {
                        reportProgress.Report(www.downloadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return www.Send();
                }
            }
            else
            {
                if (!www.isDone)
                {
                    yield return www;
                }
            }

            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(www.downloadProgress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                observer.OnError(new UnityWebRequestErrorException(www, www.downloadHandler.text));
            }
            else
            {
                observer.OnNext(www);
                observer.OnCompleted();
            }
        }
    }

    static IEnumerator FetchAssetBundle(UnityWebRequest www, IObserver<AssetBundle> observer, IProgress<float> reportProgress, CancellationToken cancel)
    {
        using (www)
        {
            if (reportProgress != null)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    try
                    {
                        reportProgress.Report(www.downloadProgress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return www.Send();
                }
            }
            else
            {
                if (!www.isDone)
                {
                    yield return www;
                }
            }

            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(www.downloadProgress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                observer.OnError(new UnityWebRequestErrorException(www, ""));
            }
            else
            {
                observer.OnNext(DownloadHandlerAssetBundle.GetContent(www));
                observer.OnCompleted();
            }
        }
    }

    static Hash MergeHash(Hash wwwFormHeaders, Hash externalHeaders)
    {
        foreach (HashEntry item in externalHeaders)
        {
            wwwFormHeaders[item.Key] = item.Value;
        }
        return wwwFormHeaders;
    }
}

public class UnityWebRequestErrorException : Exception
{
    public string RawErrorMessage { get; private set; }
    public bool HasResponse { get; private set; }
    public string Text { get; private set; }
    public System.Net.HttpStatusCode StatusCode { get; private set; }
    public System.Collections.Generic.Dictionary<string, string> ResponseHeaders { get; private set; }
    public UnityWebRequest WWW { get; private set; }

    // cache the text because if www was disposed, can't access it.
    public UnityWebRequestErrorException(UnityWebRequest www, string text)
    {
        this.WWW = www;
        this.RawErrorMessage = www.error;
        this.ResponseHeaders = www.GetResponseHeaders();
        this.HasResponse = false;
        this.Text = text;

        var splitted = RawErrorMessage.Split(' ', ':');
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
        var text = this.Text;
        if (string.IsNullOrEmpty(text))
        {
            return RawErrorMessage;
        }
        else
        {
            return RawErrorMessage + " " + text;
        }
    }
}
