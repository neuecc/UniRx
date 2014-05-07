using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public static partial class Observable
    {
        public static IObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
            where TEventArgs : EventArgs
        {
            return Observable.Create<EventPattern<TEventArgs>>(observer =>
            {
                var handler = conversion((sender, eventArgs) => observer.OnNext(new EventPattern<TEventArgs>(sender, eventArgs)));
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
        }

        private static IObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
        {
            return Observable.Create<TEventArgs>(observer =>
            {
                var handler = conversion(observer.OnNext);
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
        }
    }
}