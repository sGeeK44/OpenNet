using System;
using System.Linq;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Entity.References;
using OpenNet.Orm.Entity.Serializers;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Sync.Entity
{
    public class SyncEntity : ISyncableEntity
    {
        private readonly IEntityInfo _syncEntity;
        private readonly SynchronizedEntityAttribute _synchronizedEntity;
        private object _solver;

        private SyncEntity(IEntityInfo syncEntity, SynchronizedEntityAttribute synchronizedEntity)
        {
            _syncEntity = syncEntity;
            _synchronizedEntity = synchronizedEntity;
        }

        /// <summary>
        /// Get all entites info
        /// </summary>
        public EntityInfoCollection Entities { get { return _syncEntity.Entities; } }

        /// <summary>
        ///  Get typeof entity
        /// </summary>
        public Type EntityType { get { return _syncEntity.EntityType; } }

        /// <summary>
        /// Get entity primary key
        /// </summary>
        public PrimaryKey PrimaryKey { get { return _syncEntity.PrimaryKey; } }

        /// <summary>
        /// Get entity foreign keys
        /// </summary>
        public DistinctCollection<ForeignKey> ForeignKeys { get { return _syncEntity.ForeignKeys; } }

        /// <summary>
        /// Get entity fields (Including PK and FK)
        /// </summary>
        public DistinctCollection<Field> Fields { get { return _syncEntity.Fields; } }

        /// <summary>
        /// Get references attributes
        /// </summary>
        public DistinctCollection<Reference> References { get { return _syncEntity.References; } }

        /// <summary>
        /// Get Indexes existing on current entity
        /// </summary>
        public DistinctCollection<Index> Indexes { get { return _syncEntity.Indexes; } }

        /// <summary>
        /// Return factory use to create specific fields db engine
        /// </summary>
        public IFieldPropertyFactory FieldPropertyFactory { get { return _syncEntity.FieldPropertyFactory; } }

        /// <summary>
        /// Get column name that served to sync framework to detect inserted row
        /// </summary>
        public string CreationTrackingColumn
        {
            get { return GetFieldName(_ => _.IsCreationTracking); }
        }

        /// <summary>
        /// Get column name that served to sync framework to detect updated row
        /// </summary>
        public string UpdateTrackingColumn
        {
            get { return GetFieldName(_ => _.IsUpdateTracking); }
        }

        /// <summary>
        /// Get column name of tombstone table that served to sync framework to detect deleted row
        /// </summary>
        public string DeletionTrackingColumn
        {
            get { return GetFieldName(_ => _.IsDeletionTracking); }
        }

        /// <summary>
        /// Get column name that served to sync framework to detect updated row during last sync
        /// </summary>
        public string LastSyncTrackingColumn
        {
            get { return GetFieldName(_ => _.IsLastSyncTracking); }
        }

        /// <summary>
        /// Get type of entity tombstone
        /// </summary>
        public Type EntityTombstoneType { get { return _synchronizedEntity.EntityTombstoneType; } }

        /// <summary>
        /// Get rentention entity time on client
        /// </summary>
        /// <returns></returns>
        public int ClientRetentionTime { get { return _synchronizedEntity.ClientRetentionTime; } }

        /// <summary>
        /// Indicate if current entity should be cleanned after a sync
        /// </summary>
        /// <returns></returns>
        public bool ShouldBeCleannedOnClient()
        {
            return _synchronizedEntity.ClientRetentionTime >= 0;
        }

        /// <summary>
        /// Get entity info for associated tombstone entity
        /// </summary>
        public ISyncableEntity EntityTombstoneInfo
        {
            get
            {
                var entityName = _syncEntity.Entities.GetNameForType(EntityTombstoneType);
                var entity = Entities[entityName];
                return Create(entity);
            }
        }

        public SyncDirection Direction { get { return _synchronizedEntity.Direction; } }

        /// <summary>
        /// Return true if current entity has tombstone table
        /// </summary>
        public bool IsDeleteTrackEnable { get { return EntityTombstoneType != null; } }

        /// <summary>
        /// Get entity name in store
        /// </summary>
        public string GetNameInStore()
        {
            return _syncEntity.GetNameInStore();
        }

        /// <summary>
        /// Return serializer to use to convert back entity from db
        /// </summary>
        /// <returns>Entity serializer</returns>
        public IEntitySerializer GetSerializer()
        {
            return _syncEntity.GetSerializer();
        }

        /// <summary>
        /// Get reference attribute associated to specified object type
        /// </summary>
        /// <param name="refType">Type of reference</param>
        /// <returns>Reference attribute found, else null</returns>
        public Reference GetReference(Type refType)
        {
            return _syncEntity.GetReference(refType);
        }

        /// <summary>
        /// Looking for specified attribute on entity class
        /// </summary>
        /// <typeparam name="T">Type of attribute searched.</typeparam>
        /// <returns>Attribute found or null.</returns>
        public T GetAttribute<T>()
        {
            return _syncEntity.GetAttribute<T>();
        }

        /// <summary>
        /// Create a new instance of entity
        /// </summary>
        /// <returns>New entity</returns>
        public object CreateNewInstance()
        {
            return _syncEntity.CreateNewInstance();
        }

        /// <summary>
        /// Get service that in responsability to solve conflic during Sync
        /// </summary>
        /// <returns>Entity conflict solver for current entity type</returns>
        public object GetSolver()
        {
            if (_solver != null)
                return _solver;

            _solver = _synchronizedEntity.CreateSolver();
            return _solver;
        }

        public ISyncableEntity CreateEntityTombstone()
        {
            var entity = EntityInfo.Create(FieldPropertyFactory, Entities, EntityTombstoneType);
            return Create(entity);
        }

        private string GetFieldName(Func<Field, bool> selector)
        {
            var field = _syncEntity.Fields.FirstOrDefault(selector);
            return field == null ? null : field.FieldName;
        }

        public static SyncEntity Create(IEntityInfo syncEntity)
        {
            if (syncEntity == null)
                return null;

            var synchronizedAttribute = syncEntity.GetAttribute<SynchronizedEntityAttribute>();
            if (synchronizedAttribute == null)
                return null;

            return new SyncEntity(syncEntity, synchronizedAttribute);
        }
    }
}