using UnityEngine;

namespace UniRx.Examples
{
    public class Sample04_ConvertFromUnityCallback : TypedMonoBehaviour
    {
        // This is about log but more reliable log sample => Sample11_Logger

        private class LogCallback
        {
            public string Condition;
            public string StackTrace;
            public UnityEngine.LogType LogType;
        }

        static class LogHelper
        {
            static Subject<LogCallback> subject;

            public static IObservable<LogCallback> LogCallbackAsObservable()
            {
                if (subject == null)
                {
                    subject = new Subject<LogCallback>();

                    // Publish to Subject in callback
                    UnityEngine.Application.RegisterLogCallback((condition, stackTrace, type) =>
                    {
                        subject.OnNext(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type });
                    });
                }

                return subject.AsObservable();
            }
        }

        public override void Awake()
        {
            // method is separatable and composable
            LogHelper.LogCallbackAsObservable()
                .Where(x => x.LogType == LogType.Warning)
                .Subscribe(x => Debug.Log(x));

            LogHelper.LogCallbackAsObservable()
                .Where(x => x.LogType == LogType.Error)
                .Subscribe(x => Debug.Log(x));
        }
    }
}