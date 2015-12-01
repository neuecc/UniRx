using System;

namespace UniRx.Operators
{
    // implements note : all field must be readonly.
    public abstract class OperatorObservableBase<T> : IObservable<T>, IOptimizedObservable<T>
    {
        readonly bool isRequiredSubscribeOnCurrentThread;

        public OperatorObservableBase(bool isRequiredSubscribeOnCurrentThread)
        {
            this.isRequiredSubscribeOnCurrentThread = isRequiredSubscribeOnCurrentThread;
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return isRequiredSubscribeOnCurrentThread;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = new SingleAssignmentDisposable();

            IObserver<T> safeObserver;
            if (observer is ISafeObserver)
            {
                safeObserver = observer;
            }
            else
            {
                safeObserver = Observer.CreateAutoDetachObserver<T>(observer, subscription);
            }

            if (isRequiredSubscribeOnCurrentThread && Scheduler.IsCurrentThreadSchedulerScheduleRequired)
            {
                Scheduler.CurrentThread.Schedule(() => subscription.Disposable = SubscribeCore(safeObserver, subscription));
            }
            else
            {
                subscription.Disposable = SubscribeCore(safeObserver, subscription);
            }

            return subscription;
        }

        protected abstract IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel);
    }
}