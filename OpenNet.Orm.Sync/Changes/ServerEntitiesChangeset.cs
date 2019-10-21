using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.Changes
{
    public class ServerEntitiesChangeset : EntitiesChangeset
    {
        public ServerEntitiesChangeset(ISqlDataStore dataStore, ISyncSessionInfo syncSessionInfo)
            : base(dataStore, syncSessionInfo) { }
        
        protected override bool ShouldSkipLocalChange(SyncEntity entity)
        {
            return entity.Direction == SyncDirection.UploadOnly;
        }

        protected override SyncStates GetStepName()
        {
            return SyncStates.ComputeServerChange;
        }
    }
}