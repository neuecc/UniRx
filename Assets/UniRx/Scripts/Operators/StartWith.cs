using System;

namespace UniRx.Operators
{
    internal class StartWith<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly T value;
        readonly Func<T> valueFactory;

        public StartWith(IObservable<T> source, T value)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.value = value;
        }

        public StartWith(IObservable<T> source, Func<T> valueFactory)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.valueFactory = valueFactory;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new StartWithObserver(this, observer, cancel).Run();
        }

        class StartWithObserver : OperatorObserverBase<T, T>
        {
            readonly StartWith<T> parent;

            public StartWithObserver(StartWith<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                T t;
                if (parent.valueFactory == null)
                {
                    t = parent.value;
                }
                else
                {
                    try
                    {
                        t = parent.valueFactory();
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                        return Disposable.Empty;
                    }
                }

                OnNext(t);
                return parent.source.Subscribe(base.observer); // good bye StartWithObserver
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }
        }
    }
}