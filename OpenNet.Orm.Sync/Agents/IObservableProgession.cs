namespace OpenNet.Orm.Sync.Agents
{
    public interface IObservableProgession
    {
        void AddObserver(SyncStates stateObserve, IOrmSyncObserver observer);
    }
}