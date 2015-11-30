using System;

namespace UniRx.Operators
{
    internal class Single<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool useDefault;
        readonly Func<T, bool> predicate;

        public Single(IObservable<T> source, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.useDefault = useDefault;
        }

        public Single(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
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
                return source.Subscribe(new SingleObserver(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new SingleObserverWithPredicate(this, observer, cancel));
            }
        }

        class SingleObserver : OperatorObserverBase<T, T>
        {
            readonly Single<T> parent;
            bool seenValue;
            T lastValue;

            public SingleObserver(Single<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.seenValue = false;
            }

            public override void OnNext(T value)
            {
                if (seenValue)
                {
                    OnError(new InvalidOperationException("sequence is not single"));
                }
                else
                {
                    seenValue = true;
                    lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (!seenValue)
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
                    if (!seenValue)
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

        class SingleObserverWithPredicate : OperatorObserverBase<T, T>
        {
            readonly Single<T> parent;
            bool seenValue;
            T lastValue;

            public SingleObserverWithPredicate(Single<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.seenValue = false;
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
                    if (seenValue)
                    {
                        OnError(new InvalidOperationException("sequence is not single"));
                        return;
                    }
                    else
                    {
                        seenValue = true;
                        lastValue = value;
                    }
                }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (!seenValue)
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
                    if (!seenValue)
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