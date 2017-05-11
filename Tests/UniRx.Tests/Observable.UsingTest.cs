using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests
{
    [TestClass]
    public class ObservableUsingTest
    {
        private BooleanDisposable _resource;
        private Subject<Unit> _subject;
        private IObservable<Unit> _observable;

        [TestInitialize]
        public void Setup()
        {
            _resource = null;
            _subject = new Subject<Unit>();
            _observable = Observable.Using(
                () => new BooleanDisposable(),
                res =>
                {
                    _resource = res;
                    return _subject;
                });
        }

        [TestMethod]
        public void TestSubscribingAllocatesResourceAndPassesItToObservableFactory()
        {
            _resource.IsNull();
            _observable.Subscribe();
            _resource.IsNotNull();
        }

        [TestMethod]
        public void TestDisposingSubscriptionAlsoDisposesResource()
        {
            var subscription = _observable.Subscribe();
            _resource.IsDisposed.IsFalse();
            subscription.Dispose();
            _resource.IsDisposed.IsTrue();
        }

        [TestMethod]
        public void TestCompletingObservableDisposesResource()
        {
            _observable.Subscribe();
            _resource.IsDisposed.IsFalse();
            _subject.OnCompleted();
            _resource.IsDisposed.IsTrue();
        }

        [TestMethod]
        public void TestThrowingExceptionWithinResourceFactoryEmitsErrorViaObservable()
        {
            _observable = Observable.Using<Unit, BooleanDisposable>(
                () => throw new InvalidOperationException(),
                res => Observable.ReturnUnit());

            Exception exception = null;
            _observable.Subscribe(null, ex => exception = ex);

            exception.IsInstanceOf<InvalidOperationException>();
        }
    }
}