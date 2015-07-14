using System;

namespace UniRx.Operators
{
    internal interface ISelect<TR>
    {
        IObservable<TR2> CombineSelector<TR2>(Func<TR, TR2> selector);
    }

    internal class Select<T, TR> : OperatorObservableBase<TR>, ISelect<TR>
    {
        readonly IObservable<T> source;
        readonly Func<T, TR> selector;
        readonly Func<T, int, TR> selectorWithIndex;

        public Select(IObservable<T> source, Func<T, TR> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        public Select(IObservable<T> source, Func<T, int, TR> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selectorWithIndex = selector;
        }

        public IObservable<TR2> CombineSelector<TR2>(Func<TR, TR2> combineSelector)
        {
            if (this.selector != null)
            {
                return new Select<T, TR2>(source, x => combineSelector(this.selector(x)));
            }
            else
            {
                return new Select<TR, TR2>(this, combineSelector);
            }
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            if (selector != null)
            {
                return source.Subscribe(new SelectObserver(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new SelectObserverWithIndex(this, observer, cancel));
            }
        }

        class SelectObserver : OperatorObserverBase<T, TR>
        {
            readonly Select<T, TR> parent;

            public SelectObserver(Select<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                var v = default(TR);
                try
                {
                    v = parent.selector(value);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                observer.OnNext(v);
            }
        }

        class SelectObserverWithIndex : OperatorObserverBase<T, TR>
        {
            readonly Select<T, TR> parent;
            int index;

            public SelectObserverWithIndex(Select<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.index = 0;
            }

            public override void OnNext(T value)
            {
                var v = default(TR);
                try
                {
                    v = parent.selectorWithIndex(value, index++);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                observer.OnNext(v);
            }
        }
    }
}