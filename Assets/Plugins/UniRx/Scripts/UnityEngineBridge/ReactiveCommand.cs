using System;

namespace UniRx
{
    /// <summary>
    /// Represents ReactiveCommand&lt;Unit&gt;
    /// </summary>
    public class ReactiveCommand : ReactiveCommand<Unit>
    {
        /// <summary>
        /// CanExecute is always true.
        /// </summary>
        public ReactiveCommand()
            : base()
        { }

        /// <summary>
        /// CanExecute is changed from canExecute sequence.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue)
        {
        }

        /// <summary>Push null to subscribers.</summary>
        public void Execute()
        {
            Execute(Unit.Default);
        }
    }

    public class ReactiveCommand<T> : IObservable<T>, IDisposable
    {
        readonly Subject<T> trigger = new Subject<T>();
        readonly IDisposable canExecuteSubscription;

        ReactiveProperty<bool> canExecute;
        public IReadOnlyReactiveProperty<bool> CanExecute
        {
            get
            {
                return canExecute;
            }
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// CanExecute is always true.
        /// </summary>
        public ReactiveCommand()
        {
            this.canExecute = new ReactiveProperty<bool>(true);
            this.canExecuteSubscription = Disposable.Empty;
        }

        /// <summary>
        /// CanExecute is changed from canExecute sequence.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            this.canExecute = new ReactiveProperty<bool>(initialValue);
            this.canExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .Subscribe(b => canExecute.Value = b);
        }

        /// <summary>Push parameter to subscribers when CanExecute.</summary>
        public bool Execute(T parameter)
        {
            if (canExecute.Value)
            {
                trigger.OnNext(parameter);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Force push parameter to subscribers.</summary>
        public void ForceExecute(T parameter)
        {
            trigger.OnNext(parameter);
        }

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            canExecute.Dispose();
            trigger.OnCompleted();
            trigger.Dispose();
            canExecuteSubscription.Dispose();
        }
    }

    public static class ReactiveCommandExtensions
    {
        /// <summary>
        /// Create non parameter commands. CanExecute is changed from canExecute sequence.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        /// <summary>
        /// Create parametered comamnds. CanExecute is changed from canExecute sequence.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }

#if !UniRxLibrary

        // for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        public static IDisposable SubscribeToButton(this ReactiveCommand<Unit> command, UnityEngine.UI.Button button)
        {
            var d1 = command.CanExecute.SubscribeToInteractable(button);
            var d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            return StableCompositeDisposable.Create(d1, d2);
        }

#endif

#endif
    }
}