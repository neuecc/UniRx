using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class First<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool useDefault;
        readonly Func<T, bool> predicate;

        public First(IObservable<T> source, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.useDefault = useDefault;
        }

        public First(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
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
                return source.Subscribe(new FirstObserver(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new FirstObserverWithPredicate(this, observer, cancel));
            }
        }

        class FirstObserver : OperatorObserverBase<T, T>
        {
            readonly First<T> parent;
            bool notPublished;

            public FirstObserver(First<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                if (notPublished)
                {
                    notPublished = false;
                    observer.OnNext(value);
                    base.OnCompleted();
                    return;
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
                        base.OnCompleted();
                    }
                }
            }
        }

        class FirstObserverWithPredicate : OperatorObserverBase<T, T>
        {
            readonly First<T> parent;
            bool notPublished;

            public FirstObserverWithPredicate(First<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                if (notPublished)
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
                        observer.OnNext(value);
                        base.OnCompleted();
                    }
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
                        base.OnCompleted();
                    }
                }
            }
        }
    }
}