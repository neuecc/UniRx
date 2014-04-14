using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public static partial class Observable
    {
        public static T Wait<T>(this IObservable<T> source)
        {
            return source.WaitCore(throwOnEmpty: true, timeout: InfiniteTimeSpan);
        }

        public static T Wait<T>(this IObservable<T> source, TimeSpan timeout)
        {
            return source.WaitCore(throwOnEmpty: true, timeout: timeout);
        }

        static T WaitCore<T>(this IObservable<T> source, bool throwOnEmpty, TimeSpan timeout)
        {
            if (source == null) throw new ArgumentNullException("source");

            var semaphore = new System.Threading.ManualResetEvent(false);

            var seenValue = false;
            var value = default(T);
            var ex = default(Exception);

            using (source.Subscribe(
                onNext: x => { seenValue = true; value = x; },
                onError: x => { ex = x; semaphore.Set(); },
                onCompleted: () => semaphore.Set()))
            {
                var waitComplete = (timeout == InfiniteTimeSpan)
                    ? semaphore.WaitOne()
                    : semaphore.WaitOne(timeout);

                if (!waitComplete)
                {
                    throw new TimeoutException("OnCompleted not fired.");
                }
            }

            if (ex != null) throw ex;
            if (throwOnEmpty && !seenValue) throw new InvalidOperationException("No Elements.");

            return value;
        }
    }
}
