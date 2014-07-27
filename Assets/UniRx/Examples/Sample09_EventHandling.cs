using System;

namespace UniRx.Examples
{
    public class Sample09_EventHandling : TypedMonoBehaviour
    {
        public class MyEventArgs : EventArgs
        {
            public int MyProperty { get; set; }
        }

        public event EventHandler<MyEventArgs> FooBar;
        public event Action<int> FooFoo;

        CompositeDisposable disposables = new CompositeDisposable();

        // Subject is Rx's native event expression and recommend way for use Rx as event.
        // Subject.OnNext as fire event,
        // expose IObserver is subscibable for external source, it's no need convert.
        Subject<int> onBarBar = new Subject<int>();
        public IObservable<int> OnBarBar { get { return onBarBar; } }

        public override void Awake()
        {
            // convert to IO<EventPattern> as (sender, eventArgs)
            Observable.FromEventPattern<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => h.Invoke, h => FooBar += h, h => FooBar -= h)
                .Subscribe()
                .AddTo(disposables); // IDisposable can add to collection easily by AddTo

            // convert to IO<EventArgs>, many situation this is useful than FromEventPattern
            Observable.FromEvent<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => (sender, e) => h(e), h => FooBar += h, h => FooBar -= h)
                .Subscribe()
                .AddTo(disposables);

            // You can convert Action like event.
            Observable.FromEvent<int>(
                    h => FooFoo += h, h => FooFoo -= h)
                .Subscribe()
                .AddTo(disposables);

            // Subject as like event.
            OnBarBar.Subscribe().AddTo(disposables);
            onBarBar.OnNext(1); // fire event
        }

        public override void OnDestroy()
        {
            // manage subscription lifecycle
            disposables.Dispose();
        }
    }
}