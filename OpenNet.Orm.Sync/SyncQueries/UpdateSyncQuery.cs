using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.SyncQueries
{
    public class UpdateSyncQuery : SyncQuery
    {
        public UpdateSyncQuery(IDataStore datastore, ISyncableEntity entity, ISyncSessionInfo syncSession)
            : base(datastore, entity, syncSession) { }

        protected override string TrackingColumn { get { return Entity.UpdateTrackingColumn; } }
    }
}