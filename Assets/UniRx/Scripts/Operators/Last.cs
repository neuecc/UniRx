using System;

namespace UniRx.Operators
{
    internal class Last<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool useDefault;
        readonly Func<T, bool> predicate;

        public Last(IObservable<T> source, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.useDefault = useDefault;
        }

        public Last(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
            this.useDefault = useDefault;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate == null)
            {
                return source.Subscribe(new LastObserver(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new LastObserverWithPredicate(this, observer, cancel));
            }
        }

        class LastObserver : OperatorObserverBase<T, T>
        {
            readonly Last<T> parent;
            bool notPublished;
            T lastValue;

            public LastObserver(Last<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                notPublished = false;
                lastValue = value;
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (notPublished)
                    {
                        observer.OnNext(default(T));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                    }
                    base.OnCompleted();
                }
                else
                {
                    if (notPublished)
                    {
                        base.OnError(new InvalidOperationException("sequence is empty"));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                        base.OnCompleted();
                    }
                }
            }
        }

        class LastObserverWithPredicate : OperatorObserverBase<T, T>
        {
            readonly Last<T> parent;
            bool notPublished;
            T lastValue;

            public LastObserverWithPredicate(Last<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                bool isPassed;
                try
                {
                    isPassed = parent.predicate(value);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                if (isPassed)
                {
                    notPublished = false;
                    lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (notPublished)
                    {
                        observer.OnNext(default(T));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                    }
                    base.OnCompleted();
                }
                else
                {
                    if (notPublished)
                    {
                        base.OnError(new InvalidOperationException("sequence is empty"));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                        base.OnCompleted();
                    }
                }
            }
        }
    }
}