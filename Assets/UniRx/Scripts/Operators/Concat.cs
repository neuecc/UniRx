using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    // needs to more improvement

    public class Concat<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<IObservable<T>> sources;

        public Concat(IEnumerable<IObservable<T>> sources)
            : base(true)
        {
            this.sources = sources;
        }

        public IObservable<T> Combine(IEnumerable<IObservable<T>> combineSources)
        {
            return new Concat<T>(CombineSources(this.sources, combineSources));
        }

        static IEnumerable<IObservable<T>> CombineSources(IEnumerable<IObservable<T>> first, IEnumerable<IObservable<T>> second)
        {
            foreach (var item in first)
            {
                yield return item;
            }
            foreach (var item in second)
            {
                yield return item;
            }
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ConcatObserver(this, observer, cancel).Run();
        }

        class ConcatObserver : OperatorObserverBase<T, T>
        {
            readonly Concat<T> parent;
            readonly object gate = new object();

            bool isDisposed;
            IEnumerator<IObservable<T>> e;
            SerialDisposable subscription;
            Action nextSelf;

            public ConcatObserver(Concat<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;
                e = parent.sources.AsSafeEnumerable().GetEnumerator();
                subscription = new SerialDisposable();

                var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(RecursiveRun);

                return StableCompositeDisposable.Create(schedule, subscription, Disposable.Create(() =>
               {
                   lock (gate)
                   {
                       this.isDisposed = true;
                       this.e.Dispose();
                   }
               }));
            }

            void RecursiveRun(Action self)
            {
                lock (gate)
                {
                    this.nextSelf = self;
                    if (isDisposed) return;

                    var current = default(IObservable<T>);
                    var hasNext = false;
                    var ex = default(Exception);

                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext)
                        {
                            current = e.Current;
                            if (current == null) throw new InvalidOperationException("sequence is null.");
                        }
                        else
                        {
                            e.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                        e.Dispose();
                    }

                    if (ex != null)
                    {
                        OnError(ex);
                        return;
                    }

                    if (!hasNext)
                    {
                        base.OnCompleted();
                        base.Dispose();
                        return;
                    }

                    var source = e.Current;
                    var d = new SingleAssignmentDisposable();
                    subscription.Disposable = d;
                    d.Disposable = source.Subscribe(this);
                }
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnCompleted()
            {
                this.nextSelf();
            }
        }
    }
}
