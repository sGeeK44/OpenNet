using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.SyncQueries
{
    public class DeleteSyncQuery : SyncQuery
    {
        public DeleteSyncQuery(IDataStore datastore, ISyncableEntity entity, ISyncSessionInfo syncSession)
            : base(datastore, entity.EntityTombstoneInfo, syncSession) { }

        protected override string TrackingColumn { get { return Entity.DeletionTrackingColumn; } }
    }
}