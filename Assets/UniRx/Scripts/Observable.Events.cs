using System;
using System.Collections.Generic;
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

        public static IObservable<Unit> FromEvent<TDelegate>(Func<Action, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
        {
            return Observable.Create<Unit>(observer =>
            {
                var handler = conversion(() => observer.OnNext(Unit.Default));
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
        }

        public static IObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
        {
            return Observable.Create<TEventArgs>(observer =>
            {
                var handler = conversion(observer.OnNext);
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
        }

        public static IObservable<Unit> FromEvent(Action<Action> addHandler, Action<Action> removeHandler)
        {
            return Observable.Create<Unit>(observer =>
            {
                Action handler = () => observer.OnNext(Unit.Default);
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
        }

        public static IObservable<T> FromEvent<T>(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
        {
            return Observable.Create<T>(observer =>
            {
                Action<T> handler = x => observer.OnNext(x);
                addHandler(handler);
                return Disposable.Create(() => removeHandler(handler));
            });
        }
    }
}