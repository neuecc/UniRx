using System;
using System.Collections.Generic;
using System.Text;

namespace UniRx
{
    public static partial class Observable
    {
        public static T Wait<T>(this IObservable<T> source)
        {
            return WaitCore(source, true, InfiniteTimeSpan);
        }

        public static T Wait<T>(this IObservable<T> source, TimeSpan timeout)
        {
            return WaitCore(source, true, timeout);
        }

        static T WaitCore<T>(IObservable<T> source, bool throwOnEmpty, TimeSpan timeout)
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
