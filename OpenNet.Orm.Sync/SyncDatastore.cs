using System;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync
{
    public class SyncDatastore : SqlDataStore
    {
        public SyncDatastore(IDbEngine dbEngine, ISqlFactory sqlFactory)
            : base(dbEngine, sqlFactory) { }

        protected override IEntityInfo AddType(Type entityType)
        {
            var entity = base.AddType(entityType);
            if (entity == null)
                return null;

            var syncEntity = SyncEntity.Create(entity);
            if (syncEntity != null)
                AddType(syncEntity.EntityTombstoneType);
            return entity;
        }

        protected override IEntityInfo AddTypeSafe(Type entityType)
        {
            var entity = base.AddTypeSafe(entityType);
            if (entity == null)
                return null;

            var syncEntity = SyncEntity.Create(entity);
            if (syncEntity != null)
                AddTypeSafe(syncEntity.EntityTombstoneType);
            return entity;
        }
    }
}
