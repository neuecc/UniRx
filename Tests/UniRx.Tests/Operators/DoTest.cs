using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [TestClass]
    public class DoTest
    {
        class ListObserver : IObserver<int>
        {
            public List<int> list = new List<int>();

            public void OnCompleted()
            {
                list.Add(1000);
            }

            public void OnError(Exception error)
            {
                list.Add(100);
            }

            public void OnNext(int value)
            {
                list.Add(value);
            }
        }

        [TestMethod]
        public void Do()
        {
            var list = new List<int>();
            Observable.Empty<int>().Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).DefaultIfEmpty().Wait();
            list.Is(1000);

            list.Clear();
            Observable.Range(1, 5).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Wait();
            list.Is(1, 2, 3, 4, 5, 1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.Is(1, 2, 3, 4, 5, 100);
        }

        [TestMethod]
        public void DoObserver()
        {
            var observer = new ListObserver();
            Observable.Empty<int>().Do(observer).DefaultIfEmpty().Wait();
            observer.list.Is(1000);

            observer = new ListObserver();
            Observable.Range(1, 5).Do(observer).Wait();
            observer.list.Is(1, 2, 3, 4, 5, 1000);

            observer = new ListObserver();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(observer).Subscribe(_ => { }, _ => { }, () => { });
            observer.list.Is(1, 2, 3, 4, 5, 100);
        }

        [TestMethod]
        public void DoOnError()
        {
            var list = new List<int>();
            Observable.Empty<int>().DoOnError(_ => list.Add(100)).DefaultIfEmpty().Wait();
            list.Is();

            list.Clear();
            Observable.Range(1, 5).DoOnError(_ => list.Add(100)).Wait();
            list.Is();

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnError(_ => list.Add(100)).Subscribe(_ => { }, _ => { }, () => { });
            list.Is(100);
        }

        [TestMethod]
        public void DoOnCompleted()
        {
            var list = new List<int>();
            Observable.Empty<int>().DoOnCompleted(() => list.Add(1000)).DefaultIfEmpty().Wait();
            list.Is(1000);

            list.Clear();
            Observable.Range(1, 5).DoOnCompleted(() => list.Add(1000)).Wait();
            list.Is(1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnCompleted(() => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.Is();
        }

        [TestMethod]
        public void DoOnTerminate()
        {
            var list = new List<int>();
            Observable.Empty<int>().DoOnTerminate(() => list.Add(1000)).DefaultIfEmpty().Wait();
            list.Is(1000);

            list.Clear();
            Observable.Range(1, 5).DoOnTerminate(() => list.Add(1000)).Wait();
            list.Is(1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).DoOnTerminate(() => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.Is(1000);
        }

        [TestMethod]
        public void DoOnSubscribe()
        {
            var list = new List<int>();
            Observable.Empty<int>()
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnSubscribe(() => list.Add(10000)).DefaultIfEmpty().Wait();
            list.Is(10000, 1000);

            list.Clear();
            Observable.Range(1, 5)
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnSubscribe(() => list.Add(10000)).Wait();
            list.Is(10000, 1, 2, 3, 4, 5, 1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception()))
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnSubscribe(() => list.Add(10000)).Subscribe(_ => { }, _ => { }, () => { });
            list.Is(10000, 1, 2, 3, 4, 5, 100);
        }

        [TestMethod]
        public void DoOnCancel()
        {
            var list = new List<int>();
            Observable.Empty<int>()
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnCancel(() => list.Add(5000))
                .DoOnCancel(() => list.Add(10000))
                .DefaultIfEmpty()
                .Subscribe(_ => { }, _ => { }, () => { });
            list.Is(1000);

            list.Clear();
            Observable.Throw<int>(new Exception())
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnCancel(() => list.Add(5000))
                .DoOnCancel(() => list.Add(10000))
                .DefaultIfEmpty()
                .Subscribe(_ => { }, _ => { }, () => { });
            list.Is(100);

            list.Clear();
            var subscription = Observable.Timer(TimeSpan.FromMilliseconds(1000)).Select(x => (int)x)
                .Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000))
                .DoOnCancel(() => list.Add(5000))
                .DoOnCancel(() => list.Add(10000))
                .Subscribe();
            subscription.Dispose();
            list.Is(5000, 10000);
        }
    }
}
