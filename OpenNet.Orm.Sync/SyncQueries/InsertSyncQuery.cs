using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable ArrangeAccessorOwnerBody

namespace OpenNet.Orm.Sync.SyncQueries
{
    public class InsertSyncQuery : SyncQuery
    {
        public InsertSyncQuery(IDataStore datastore, ISyncableEntity entity, ISyncSessionInfo syncSession)
            : base(datastore, entity, syncSession) { }

        protected override string TrackingColumn { get { return Entity.CreationTrackingColumn; } }
    }
}