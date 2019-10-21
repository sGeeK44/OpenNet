using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.SyncQueries
{
    public class LastSyncQuery : SyncQuery
    {
        public LastSyncQuery(IDataStore datastore, ISyncableEntity entity, ISyncSessionInfo syncSession)
            : base(datastore, entity, syncSession) { }

        protected override string TrackingColumn { get { return Entity.LastSyncTrackingColumn; } }
    }
}