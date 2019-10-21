using System;

namespace OpenNet.Orm.Sync.Agents
{
    public class LinearProgression : IObservableProgession
    {
        private int _currentStep;
        private readonly int _totalStep;
        private IOrmSyncObserver _observer;

        public LinearProgression(int totalStep)
        {
            if (totalStep < 1)
                throw new ArgumentException("Should have one step at least", "totalStep");
            _totalStep = totalStep;
        }

        public void AddObserver(SyncStates stateObserve, IOrmSyncObserver observer)
        {
            _observer = observer;
        }

        public void CurrentStepFinished()
        {
            _currentStep++;
            if (_observer == null)
                return;

            var progress = ComputePercentProgress();
            _observer.ReportProgess(progress);
        }

        private int ComputePercentProgress()
        {
            return _currentStep * 100 / _totalStep;
        }
    }
}