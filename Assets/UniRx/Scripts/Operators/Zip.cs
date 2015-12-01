using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ZipObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TLeft> left;
        readonly IObservable<TRight> right;
        readonly Func<TLeft, TRight, TResult> selector;

        public ZipObservable(IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
            : base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
        {
            this.left = left;
            this.right = right;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : OperatorObserverBase<TResult, TResult>
        {
            readonly ZipObservable<TLeft, TRight, TResult> parent;

            readonly object gate = new object();
            readonly Queue<TLeft> leftQ = new Queue<TLeft>();
            bool leftCompleted = false;
            readonly Queue<TRight> rightQ = new Queue<TRight>();
            bool rightCompleted = false;

            public Zip(ZipObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var l = parent.left.Subscribe(new LeftZipObserver(this));
                var r = parent.right.Subscribe(new RightZipObserver(this));

                return StableCompositeDisposable.Create(l, r, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        leftQ.Clear();
                        rightQ.Clear();
                    }
                }));
            }

            // dequeue is in the lock
            void Dequeue()
            {
                TLeft lv;
                TRight rv;
                TResult v;

                if (leftQ.Count != 0 && rightQ.Count != 0)
                {
                    lv = leftQ.Dequeue();
                    rv = rightQ.Dequeue();
                }
                else if (leftCompleted || rightCompleted)
                {
                    OnCompleted();
                    return;
                }
                else
                {
                    return;
                }

                try
                {
                    v = parent.selector(lv, rv);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                OnNext(v);
            }

            public override void OnNext(TResult value)
            {
                base.observer.OnNext(value);
            }

            class LeftZipObserver : IObserver<TLeft>
            {
                readonly Zip parent;

                public LeftZipObserver(Zip parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TLeft value)
                {
                    lock (parent.gate)
                    {
                        parent.leftQ.Enqueue(value);
                        parent.Dequeue();
                    }
                }

                public void OnError(Exception ex)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(ex);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.leftCompleted = true;
                        if (parent.rightCompleted) parent.OnCompleted();
                    }
                }
            }

            class RightZipObserver : IObserver<TRight>
            {
                readonly Zip parent;

                public RightZipObserver(Zip parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TRight value)
                {
                    lock (parent.gate)
                    {
                        parent.rightQ.Enqueue(value);
                        parent.Dequeue();
                    }
                }

                public void OnError(Exception ex)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(ex);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.rightCompleted = true;
                        if (parent.leftCompleted) parent.OnCompleted();
                    }
                }
            }
        }
    }

    internal class ZipObservable<T> : OperatorObservableBase<IList<T>>
    {
        readonly IObservable<T>[] sources;

        public ZipObservable(IObservable<T>[] sources)
            : base(true)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<T>> observer, IDisposable cancel)
        {
            return new Zip(this, observer, cancel).Run();
        }

        class Zip : OperatorObserverBase<IList<T>, IList<T>>
        {
            readonly ZipObservable<T> parent;
            readonly object gate = new object();

            Queue<T>[] queues;
            bool[] isDone;
            int length;

            public Zip(ZipObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                length = parent.sources.Length;
                queues = new Queue<T>[length];
                isDone = new bool[length];

                for (int i = 0; i < length; i++)
                {
                    queues[i] = new Queue<T>();
                }

                var disposables = new IDisposable[length + 1];
                for (int i = 0; i < length; i++)
                {
                    var source = parent.sources[i];
                    disposables[i] = source.Subscribe(new ZipObserver(this, i));
                }

                disposables[length] = Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            var q = queues[i];
                            q.Clear();
                        }
                    }
                });

                return StableCompositeDisposable.CreateUnsafe(disposables);
            }

            // dequeue is in the lock
            void Dequeue(int index)
            {
                var allQueueHasValue = true;
                for (int i = 0; i < length; i++)
                {
                    if (queues[i].Count == 0)
                    {
                        allQueueHasValue = false;
                        break;
                    }
                }

                if (!allQueueHasValue)
                {
                    var allCompletedWithoutSelf = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (i == index) continue;
                        if (!isDone[i])
                        {
                            allCompletedWithoutSelf = false;
                            break;
                        }
                    }

                    if (allCompletedWithoutSelf)
                    {
                        OnCompleted();
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                var array = new T[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = queues[i].Dequeue();
                }

                OnNext(array);
            }

            public override void OnNext(IList<T> value)
            {
                base.observer.OnNext(value);
            }

            class ZipObserver : IObserver<T>
            {
                readonly Zip parent;
                readonly int index;

                public ZipObserver(Zip parent, int index)
                {
                    this.parent = parent;
                    this.index = index;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        parent.queues[index].Enqueue(value);
                        parent.Dequeue(index);
                    }
                }

                public void OnError(Exception ex)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(ex);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.isDone[index] = true;
                        var allTrue = true;
                        for (int i = 0; i < parent.length; i++)
                        {
                            if (!parent.isDone[i])
                            {
                                allTrue = false;
                                break;
                            }
                        }

                        if (allTrue)
                        {
                            parent.OnCompleted();
                        }
                    }
                }
            }
        }
    }
}