using System;
using UniRx.Operators;

namespace UniRx.Operators
{
    internal class Scan<TSource> : OperatorObservableBase<TSource>
    {
        readonly IObservable<TSource> source;
        readonly Func<TSource, TSource, TSource> accumulator;

        public Scan(IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.accumulator = accumulator;
        }

        protected override IDisposable SubscribeCore(IObserver<TSource> observer, IDisposable cancel)
        {
            return source.Subscribe(new ScanObserver(this, observer, cancel));
        }

        class ScanObserver : OperatorObserverBase<TSource, TSource>
        {
            readonly Scan<TSource> parent;
            TSource accumulation;
            bool isFirst;

            public ScanObserver(Scan<TSource> parent, IObserver<TSource> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.isFirst = true;
            }

            public override void OnNext(TSource value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    accumulation = value;
                }
                else
                {
                    try
                    {
                        accumulation = parent.accumulator(accumulation, value);
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                        return;
                    }
                }

                observer.OnNext(accumulation);
            }
        }
    }

    internal class Scan<TSource, TAccumulate> : OperatorObservableBase<TAccumulate>
    {
        readonly IObservable<TSource> source;
        readonly TAccumulate seed;
        readonly Func<TAccumulate, TSource, TAccumulate> accumulator;

        public Scan(IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.seed = seed;
            this.accumulator = accumulator;
        }

        protected override IDisposable SubscribeCore(IObserver<TAccumulate> observer, IDisposable cancel)
        {
            return source.Subscribe(new ScanObserver(this, observer, cancel));
        }

        class ScanObserver : OperatorObserverBase<TSource, TAccumulate>
        {
            readonly Scan<TSource, TAccumulate> parent;
            TAccumulate accumulation;
            bool isFirst;

            public ScanObserver(Scan<TSource, TAccumulate> parent, IObserver<TAccumulate> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.isFirst = true;
            }

            public override void OnNext(TSource value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    accumulation = parent.seed;
                }

                try
                {
                    accumulation = parent.accumulator(accumulation, value);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                observer.OnNext(accumulation);
            }
        }
    }
}