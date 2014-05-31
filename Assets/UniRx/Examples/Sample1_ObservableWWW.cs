using UnityEngine;

namespace UniRx.Examples
{
    // sample script, attach your object.
    public class Sample1_ObservableWWW : TypedMonoBehaviour
    {
        public override void Awake()
        {
            // Basic: Download from google.
            {
                ObservableWWW.Get("http://google.co.jp/")
                    .Subscribe(
                        x => Debug.Log(x), // onSuccess
                        ex => Debug.LogException(ex)); // onError
            }

            // Linear Pattern with LINQ Query Expressions
            // download after google, start bing download
            {
                var query = from google in ObservableWWW.Get("http://google.com/")
                            from bing in ObservableWWW.Get("http://bing.com/")
                            select new { google, bing };

                var cancel = query.Subscribe(x => Debug.Log(x.google + ":" + x.bing));

                // Call Dispose is cancel downloading.
                cancel.Dispose();
            }


            // Observable.WhenAll is for parallel asynchronous operation
            // (It's like Observable.Zip but specialized for single async operations like Task.WhenAll of .NET 4)
            {
                var parallel = Observable.WhenAll(
                    ObservableWWW.Get("http://google.com/"),
                    ObservableWWW.Get("http://bing.com/"),
                    ObservableWWW.Get("http://yahoo.com/"));

                parallel.Subscribe(xs =>
                {
                    Debug.Log(xs[0]); // google
                    Debug.Log(xs[1]); // bing
                    Debug.Log(xs[2]); // yahoo
                });
            }

            // with Progress
            {
                // notifier for progress
                var progressNotifier = new ScheduledNotifier<float>();
                progressNotifier.Subscribe(x => Debug.Log(x)); // write www.progress

                // pass notifier to WWW.Get/Post
                ObservableWWW.Get("http://google.com/", progress: progressNotifier).Subscribe();
            }

            // with Error
            {
                // when WWW has .error, throws WWWErrorException
                // WWWErrorException has RawErrorMessage, HasResponse, StatusCode, ResponseHeaders
                ObservableWWW.Get("http://www.google.com/404")
                    .CatchIgnore((WWWErrorException ex) =>
                    {
                        Debug.Log(ex.RawErrorMessage);
                        if (ex.HasResponse)
                        {
                            Debug.Log(ex.StatusCode);
                        }
                        foreach (var item in ex.ResponseHeaders)
                        {
                            Debug.Log(item.Key + ":" + item.Value);
                        }
                    })
                    .Subscribe();
            }
        }
    }
}