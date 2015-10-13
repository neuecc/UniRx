﻿// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;


namespace UniRx
{
#if SystemReactive
    using System.Reactive.Linq;
    using Observable = System.Reactive.Linq.Observable;
#endif

    public static partial class UnityEventExtensions
    {
        public static IObservable<Unit> AsObservable(this UnityEngine.Events.UnityEvent unityEvent)
        {
            var dummy = 0;
            return Observable.FromEvent<UnityAction>(h =>
            {
                dummy.GetHashCode(); // capture for AOT issue
                return new UnityAction(h);
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h))
#if SystemReactive
            .Select(_ => Unit.Default)
#endif
            ;
        }

        public static IObservable<T> AsObservable<T>(this UnityEngine.Events.UnityEvent<T> unityEvent)
        {
            var dummy = 0;
            return Observable.FromEvent<UnityAction<T>, T>(h =>
            {
                dummy.GetHashCode(); // capture for AOT issue
                return new UnityAction<T>(h);
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<Tuple<T0, T1>> AsObservable<T0, T1>(this UnityEngine.Events.UnityEvent<T0, T1> unityEvent)
        {
            var dummy = 0;
            return Observable.FromEvent<UnityAction<T0, T1>, Tuple<T0, T1>>(h =>
            {
                return new UnityAction<T0, T1>((t0, t1) =>
                {
                    dummy.GetHashCode(); // capture for AOT issue
                    h(Tuple.Create(t0, t1));
                });
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<Tuple<T0, T1, T2>> AsObservable<T0, T1, T2>(this UnityEngine.Events.UnityEvent<T0, T1, T2> unityEvent)
        {
            var dummy = 0;
            return Observable.FromEvent<UnityAction<T0, T1, T2>, Tuple<T0, T1, T2>>(h =>
            {
                return new UnityAction<T0, T1, T2>((t0, t1, t2) =>
                {
                    dummy.GetHashCode(); // capture for AOT issue
                    h(Tuple.Create(t0, t1, t2));
                });
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<Tuple<T0, T1, T2, T3>> AsObservable<T0, T1, T2, T3>(this UnityEngine.Events.UnityEvent<T0, T1, T2, T3> unityEvent)
        {
            var dummy = 0;
            return Observable.FromEvent<UnityAction<T0, T1, T2, T3>, Tuple<T0, T1, T2, T3>>(h =>
            {
                return new UnityAction<T0, T1, T2, T3>((t0, t1, t2, t3) =>
                {
                    dummy.GetHashCode(); // capture for AOT issue
                    h(Tuple.Create(t0, t1, t2, t3));
                });
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }
    }
}

#endif