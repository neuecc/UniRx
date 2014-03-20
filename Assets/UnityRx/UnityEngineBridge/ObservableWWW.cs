using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityRx
{
    public static class ObservableWWW
    {
        static readonly Hashtable defaultHeaders = new Hashtable()
        {
            {"User-Agent", "HogeHoge"}
        };

        static IEnumerator GetWWW(string url, Action<string> onSuccess, Action<string> onError)
        {
            using (var www = new WWW(url))
            {
                yield return www;
                if (www.isDone)
                {
                    if (www.error != null)
                    {
                        onError(www.error);
                    }
                    else
                    {
                        onSuccess(www.text);
                    }
                }
            }
        }

        //static IEnumerator PostWWW(string url, WWWForm form, Action<string> onSuccess, Action<string> onError)
        //{
            
        //    using (var www = new WWW(url, form))
        //    {
                
        //    }
        //}

        //public static IObservable<string> Post(string url)
        //{

        //}

        public static IObservable<string> Get(string url)
        {
            return Observable.Create<string>(observer =>
            {
                var e = GetWWW(url, x =>
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
                }, x => observer.OnError(new Exception(x)));

                GameLoopDispatcher.StartCoroutine(e);

                return Disposable.Empty;
            });
        }
    }
}