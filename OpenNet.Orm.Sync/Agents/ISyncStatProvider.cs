namespace OpenNet.Orm.Sync.Agents
{
    public interface ISyncStatProvider 
    {
        void SetNewState(SyncStates syncStates, IObservableProgession observableProgession);
    }
}