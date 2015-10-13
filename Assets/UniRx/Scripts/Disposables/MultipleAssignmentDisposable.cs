using System;
using System.Collections;

#if SystemReactive
namespace System.Reactive.Disposables
#else
namespace UniRx
#endif
{
    public class MultipleAssignmentDisposable : IDisposable, ICancelable
    {
        static readonly BooleanDisposable True = new BooleanDisposable(true);

        object gate = new object();
        IDisposable current;

        public bool IsDisposed
        {
            get
            {
                lock (gate)
                {
                    return current == True;
                }
            }
        }

        public IDisposable Disposable
        {
            get
            {
                lock (gate)
                {
                    return (current == True)
#if SystemReactive
                        ? System.Reactive.Disposables.Disposable.Empty
#else
                        ? UniRx.Disposable.Empty
#endif
                        : current;
                }
            }
            set
            {
                var shouldDispose = false;
                lock (gate)
                {
                    shouldDispose = (current == True);
                    if (!shouldDispose)
                    {
                        current = value;
                    }
                }
                if (shouldDispose && value != null)
                {
                    value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            IDisposable old = null;

            lock (gate)
            {
                if (current != True)
                {
                    old = current;
                    current = True;
                }
            }

            if (old != null) old.Dispose();
        }
    }
}