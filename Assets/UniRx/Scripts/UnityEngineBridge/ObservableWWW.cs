using System;
using System.Collections;
using UnityEngine;

namespace UniRx
{
    public static partial class ObservableWWW
    {
#if !(UNITY_METRO || UNITY_WP8)
        static IEnumerator GetWWWBytes(string url, Hashtable headers, Action<byte[]> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
#else
        static IEnumerator GetWWWBytes(string url, System.Collections.Generic.Dictionary<string, string> headers, Action<byte[]> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
#endif
        {
            return FetchBytes(new WWW(url, null, headers), onSuccess, onError, reportProgress, cancel);
        }

#if !(UNITY_METRO || UNITY_WP8)
        static IEnumerator GetWWWText(string url, Hashtable headers, Action<string> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
#else
        static IEnumerator GetWWWText(string url, System.Collections.Generic.Dictionary<string, string> headers, Action<string> onSuccess, Action<string> onError, IProgress<float> reportProgress, ICancelable cancel)
#endif
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

        public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
        {
            return Observable.Create<string>(observer =>
            {
                var cancel = new BooleanDisposable();

                var e = PostWWWText(url, content, x =>
                {
                    observer.OnNext(x);
                    observer.OnCompleted();
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
                    observer.OnNext(x);
                    observer.OnCompleted();
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }

#if !(UNITY_METRO || UNITY_WP8)
        public static IObservable<string> Get(string url, Hashtable headers = null, IProgress<float> progress = null)
#else
        public static IObservable<string> Get(string url, System.Collections.Generic.Dictionary<string, string> headers = null, IProgress<float> progress = null)
#endif
        {
            return Observable.Create<string>(observer =>
            {
                var cancel = new BooleanDisposable();

#if !(UNITY_METRO || UNITY_WP8)
                var e = GetWWWText(url, headers ?? new Hashtable(), x =>
#else
                var e = GetWWWText(url, headers ?? new System.Collections.Generic.Dictionary<string, string>(), x =>
#endif
                {
                    observer.OnNext(x);
                    observer.OnCompleted();
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }

#if !(UNITY_METRO || UNITY_WP8)
        public static IObservable<byte[]> GetAndGetBytes(string url, Hashtable headers = null, IProgress<float> progress = null)
#else
        public static IObservable<byte[]> GetAndGetBytes(string url, System.Collections.Generic.Dictionary<string, string> headers = null, IProgress<float> progress = null)
#endif
        {
            return Observable.Create<byte[]>(observer =>
            {
                var cancel = new BooleanDisposable();

#if !(UNITY_METRO || UNITY_WP8)
                var e = GetWWWBytes(url, headers ?? new Hashtable(), x =>
#else
                var e = GetWWWBytes(url, headers ?? new System.Collections.Generic.Dictionary<string, string>(), x =>
#endif
                {
                    observer.OnNext(x);
                    observer.OnCompleted();
                }, x => observer.OnError(new Exception(x)), progress, cancel);

                MainThreadDispatcher.StartCoroutine(e);

                return cancel;
            });
        }
    }
}