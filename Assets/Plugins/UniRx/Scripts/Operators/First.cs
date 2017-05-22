using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class FirstObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool publishOnError;
        readonly bool publishDefaultValue;
        readonly Func<T, bool> predicate;

        public FirstObservable(IObservable<T> source, bool publishOnError, bool publishDefaultValue)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.publishOnError = publishOnError;
            this.publishDefaultValue = publishDefaultValue;
        }

        public FirstObservable(IObservable<T> source, Func<T, bool> predicate, bool publishOnError, bool publishDefaultValue)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
            this.publishOnError = publishOnError;
            this.publishDefaultValue = publishDefaultValue;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate == null)
            {
                return source.Subscribe(new First(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new First_(this, observer, cancel));
            }
        }

        class First : OperatorObserverBase<T, T>
        {
            readonly FirstObservable<T> parent;
            bool notPublished;

            public First(FirstObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                    return;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (!parent.publishOnError)
                {
                    if (notPublished && parent.publishDefaultValue)
                    {
                        observer.OnNext(default(T));
                    }
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
                else
                {
                    if (notPublished)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is empty")); }
                        finally { Dispose(); }
                    }
                    else
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }

        // with predicate
        class First_ : OperatorObserverBase<T, T>
        {
            readonly FirstObservable<T> parent;
            bool notPublished;

            public First_(FirstObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }

                    if (isPassed)
                    {
                        notPublished = false;
                        observer.OnNext(value);
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (!parent.publishOnError)
                {
                    if (notPublished && parent.publishDefaultValue)
                    {
                        observer.OnNext(default(T));
                    }
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
                else
                {
                    if (notPublished)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is empty")); }
                        finally { Dispose(); }
                    }
                    else
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }
    }
}