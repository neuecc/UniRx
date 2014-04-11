using System;
using System.Collections;
using UnityEngine;

namespace UnityRx
{
    public static partial class ObservableWWW
    {
        static IEnumerator GetWWWBytes(string url, Hashtable headers, Action<byte[]> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
        {
            return FetchBytes(new WWW(url, null, headers), onSuccess, onError, reportProgress, cancel);
        }
        static IEnumerator GetWWWText(string url, Hashtable headers, Action<string> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
        {
            return FetchText(new WWW(url, null, headers), onSuccess, onError, reportProgress, cancel);
        }

        static IEnumerator PostWWWBytes(string url, WWWForm content, Action<byte[]> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
        {
            return FetchBytes(new WWW(url, content), onSuccess, onError, reportProgress, cancel);
        }

        static IEnumerator PostWWWText(string url, WWWForm content, Action<string> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
        {
            return FetchText(new WWW(url, content), onSuccess, onError, reportProgress, cancel);
        }

        static IEnumerator FetchText(WWW www, Action<string> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsDisposed)
                {
                    if (reportProgress != null) reportProgress.Report(www.progress);
                    yield return null;
                }

                if (www.error != null)
                {
                    onError(www.error);
                }
                else
                {
                    if (!cancel.IsDisposed)
                    {
                        onSuccess(www.text);
                    }
                }
            }
        }

        static IEnumerator FetchBytes(WWW www, Action<byte[]> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsDisposed)
                {
                    if (reportProgress != null) reportProgress.Report(www.progress);
                    yield return null;
                }

                if (www.error != null)
                {
                    onError(www.error);
                }
                else
                {
                    if (!cancel.IsDisposed)
                    {
                        onSuccess(www.bytes);
                    }
                }
            }
        }

        public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.Create<string>(observer =>
            {
                var cancel = new BooleanDisposable();

                var e = PostWWWText(url, content, x =>
                {
                    try
                    {
                        observer.OnNext(x);
                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.Create<byte[]>(observer =>
            {
                var cancel = new BooleanDisposable();

                var e = PostWWWBytes(url, content, x =>
                {
                    try
                    {
                        observer.OnNext(x);
                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }

        public static IObservable<string> Get(string url, Hashtable headers = null, IProgress<float> progress = null)
        {
            return Observable.Create<string>(observer =>
            {
                var cancel = new BooleanDisposable();

                var e = GetWWWText(url, headers ?? new Hashtable(), x =>
                {
                    try
                    {
                        observer.OnNext(x);
                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }

        public static IObservable<byte[]> GetAndGetBytes(string url, Hashtable headers = null, IProgress<float> progress = null)
        {
            return Observable.Create<byte[]>(observer =>
            {
                var cancel = new BooleanDisposable();

                var e = GetWWWBytes(url, headers ?? new Hashtable(), x =>
                {
                    try
                    {
                        observer.OnNext(x);
                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }
    }
}