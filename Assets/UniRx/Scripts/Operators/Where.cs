using System;

namespace UniRx.Operators
{
    internal class Where<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Func<T, bool> predicate;
        readonly Func<T, int, bool> predicateWithIndex;

        public Where(IObservable<T> source, Func<T, bool> predicate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
        }

        public Where(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicateWithIndex = predicateWithIndex;
        }

        // Optimize for .Where().Where()

        public IObservable<T> CombinePredicate(Func<T, bool> combinePredicate)
        {
            if (this.predicate != null)
            {
                return new Where<T>(source, x => this.predicate(x) && combinePredicate(x));
            }
            else
            {
                return new Where<T>(this, combinePredicate);
            }
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate != null)
            {
                return source.Subscribe(new WhereObserver(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new WhereObserverWithIndex(this, observer, cancel));
            }
        }

        class WhereObserver : OperatorObserverBase<T, T>
        {
            readonly Where<T> parent;

            public WhereObserver(Where<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                var isPassed = false;
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
                    observer.OnNext(value);
                }
            }
        }

        class WhereObserverWithIndex : OperatorObserverBase<T, T>
        {
            readonly Where<T> parent;
            int index;

            public WhereObserverWithIndex(Where<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.index = 0;
            }

            public override void OnNext(T value)
            {
                var isPassed = false;
                try
                {
                    isPassed = parent.predicateWithIndex(value, index++);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                if (isPassed)
                {
                    observer.OnNext(value);
                }
            }
        }
    }
}