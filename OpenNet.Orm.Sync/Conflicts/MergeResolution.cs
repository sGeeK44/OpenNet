using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.Conflicts
{
    public class MergeResolution
    {
        public OperationTypes OperationType { get; set; }

        public EntityChange Entity { get; set; }

        public void ApplyResolution(ISqlDataStore datastore, ISyncSessionInfo syncSessionInfo)
        {
            var entityInfo = datastore.Entities[Entity.EntityName];
            var entityChangesetBuilder = new EntityChangesetBuilder(entityInfo, syncSessionInfo);
            switch (OperationType)
            {
                case OperationTypes.Insert:
                    Entity.ApplyInsert(datastore, entityChangesetBuilder);
                    break;
                case OperationTypes.Update:
                    Entity.ApplyUpdate(datastore, entityChangesetBuilder);
                    break;
                case OperationTypes.Delete:
                    Entity.ApplyDelete(datastore, Entity.EntityName, entityChangesetBuilder);
                    break;
            }
        }

        public static MergeResolution Create(IEntityInfo entityInfo, OperationTypes operationType, IEntity entity)
        {
            var entitySync = SyncEntity.Create(entityInfo);
            if (entitySync.Direction == SyncDirection.UploadOnly)
                return null;

            return new MergeResolution
            {
                OperationType = operationType,
                Entity = EntityChange.Create(entityInfo, entity)
            };
        }
    }
}