namespace UniRx.Triggers
{
    public class ObservableFixedUpdateTrigger : ObservableTriggerBase
    {
        Subject<Unit> fixedUpdate;

        /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
        void FixedUpdateAsObservable()
        {
            if (fixedUpdate != null) fixedUpdate.OnNext(Unit.Default);
        }

        /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
        public IObservable<Unit> UpdateAsObservable()
        {
            return fixedUpdate ?? (fixedUpdate = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (fixedUpdate != null)
            {
                fixedUpdate.OnCompleted();
                fixedUpdate.Dispose();
            }
        }
    }
}