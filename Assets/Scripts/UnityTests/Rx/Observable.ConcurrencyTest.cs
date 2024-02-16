﻿using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace UniRx.Tests
{

    public class ObservableConcurrencyTest
    {
        [SetUp]
        public void Init()
        {
            TestUtil.SetScehdulerForImport();
        }

        [TearDown]
        public void Dispose()
        {
            Scheduler.SetDefaultForUnity();
        }

        [Test]
        public void ObserveOnTest()
        {
            Init();

            var xs = Observable.Range(1, 10)
                .ObserveOn(Scheduler.ThreadPool)
                .ToArrayWait();

            xs.OrderBy(x => x).Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

            var s = new Subject<int>();

            var list = new List<Notification<int>>();
            s.ObserveOn(Scheduler.Immediate).Materialize().Subscribe(list.Add);

            s.OnError(new Exception());

            list[0].Kind.Is(NotificationKind.OnError);

            s = new Subject<int>();
            s.ObserveOn(Scheduler.Immediate).Materialize().Subscribe(list.Add);

            s.OnCompleted();
            list[1].Kind.Is(NotificationKind.OnCompleted);
        }

        [Test]
        public void AmbTest()
        {
            var xs = Observable.Return(10).Delay(TimeSpan.FromSeconds(1), Scheduler.ThreadPool).Concat(Observable.Range(1, 3));
            var ys = Observable.Return(30).Delay(TimeSpan.FromSeconds(2), Scheduler.ThreadPool).Concat(Observable.Range(5, 3));

            // win left
            var result = xs.Amb(ys).ToArray().Wait();

            result[0].Is(10);
            result[1].Is(1);
            result[2].Is(2);
            result[3].Is(3);

            // win right
            result = ys.Amb(xs).ToArray().Wait();

            result[0].Is(10);
            result[1].Is(1);
            result[2].Is(2);
            result[3].Is(3);
        }

        [Test]
        public void AmbMultiTest()
        {
            var xs = Observable.Return(10).Delay(TimeSpan.FromSeconds(5), Scheduler.ThreadPool).Concat(Observable.Range(1, 3));
            var ys = Observable.Return(30).Delay(TimeSpan.FromSeconds(1), Scheduler.ThreadPool).Concat(Observable.Range(5, 3));
            var zs = Observable.Return(50).Delay(TimeSpan.FromSeconds(3), Scheduler.ThreadPool).Concat(Observable.Range(9, 3));

            // win center
            var result = Observable.Amb(xs, ys, zs).ToArray().Wait();

            result[0].Is(30);
            result[1].Is(5);
            result[2].Is(6);
            result[3].Is(7);

            // win first
            result = Observable.Amb(new[] { ys, xs, zs }.AsEnumerable()).ToArray().Wait();

            result[0].Is(30);
            result[1].Is(5);
            result[2].Is(6);
            result[3].Is(7);

            // win last
            result = Observable.Amb(new[] { zs, xs, ys }.AsEnumerable()).ToArray().Wait();

            result[0].Is(30);
            result[1].Is(5);
            result[2].Is(6);
            result[3].Is(7);
        }
    }
}