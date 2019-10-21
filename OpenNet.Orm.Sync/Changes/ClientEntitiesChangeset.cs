using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.Changes
{
    public class ClientEntitiesChangeset : EntitiesChangeset
    {
        public ClientEntitiesChangeset(ISqlDataStore dataStore, ISyncSessionInfo syncSessionInfo)
            : base(dataStore, syncSessionInfo) { }
        
        protected override bool ShouldSkipLocalChange(SyncEntity entity)
        {
            return entity.Direction == SyncDirection.DownloadOnly;
        }

        protected override SyncStates GetStepName()
        {
            return SyncStates.ComputeRemoteChange;
        }
    }
}