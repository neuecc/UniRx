using System;
using System.Collections;
using UnityEngine;

namespace UniRx.Operators
{
    internal class DelayFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public DelayFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new DelayFrame(this, observer, cancel).Run();
        }

        class DelayFrame : OperatorObserverBase<T, T>
        {
            readonly DelayFrameObservable<T> parent;
            YieldInstruction yieldInstruction;
            BooleanDisposable coroutineKey;

            public DelayFrame(DelayFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.yieldInstruction = parent.frameCountType.GetYieldInstruction();
            }

            public IDisposable Run()
            {
                coroutineKey = new BooleanDisposable();
                var sourceSubscription = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(coroutineKey, sourceSubscription);
            }

            IEnumerator OnNextDelay(T value)
            {
                var frameCount = parent.frameCount;
                while (!coroutineKey.IsDisposed && frameCount-- != 0)
                {
                    yield return yieldInstruction;
                }
                if (!coroutineKey.IsDisposed)
                {
                    observer.OnNext(value);
                }
            }

            IEnumerator OnCompletedDelay()
            {
                var frameCount = parent.frameCount;
                while (!coroutineKey.IsDisposed && frameCount-- != 0)
                {
                    yield return yieldInstruction;
                }
                if (!coroutineKey.IsDisposed)
                {
                    coroutineKey.Dispose();

                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
            }

            public override void OnNext(T value)
            {
                if (coroutineKey.IsDisposed) return;

                MainThreadDispatcher.StartCoroutine(OnNextDelay(value));
            }

            public override void OnError(Exception error)
            {
                if (coroutineKey.IsDisposed) return;

                coroutineKey.Dispose();
                try { base.observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (coroutineKey.IsDisposed) return;
                MainThreadDispatcher.StartCoroutine(OnCompletedDelay());
            }
        }
    }
}