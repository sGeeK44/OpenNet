using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.References;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity(NameInStore = TableName)]
    [SynchronizedEntity(EntityTombstoneType = typeof(CustomEntityTombstone<EntityRelated, IEntityRelated>))]
    public class EntityRelated : SyncableEntity<IEntityRelated>, IEntityRelated
    {
        private readonly NullableReferenceHolder<EntitySync> _entityRelated;

        public const string TableName = "entity_related";
        public const string ColumnNameWorkspaceId = "related_id";

        public EntityRelated()
        {
            _entityRelated = new NullableReferenceHolder<EntitySync>(null);
        }

        [ForeignKey(typeof(EntitySync), FieldName = ColumnNameWorkspaceId, SearchOrder = FieldSearchOrder.Ascending)]
        public long? RelatedId
        {
            get => _entityRelated.Id;
            set => _entityRelated.Id = value;
        }

        [Field]
        public int? IntField { get; set; }
    }
}