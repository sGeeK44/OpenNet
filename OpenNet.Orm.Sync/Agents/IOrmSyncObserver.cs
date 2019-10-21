namespace OpenNet.Orm.Sync.Agents
{
    public interface IOrmSyncObserver : IOrmObserver
    {
        SyncStates CurrentState { get; }
        void OnNewState(SyncStates newState, bool isTimeDeterminated);
    }
}