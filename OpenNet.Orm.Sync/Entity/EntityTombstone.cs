using System;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Repositories;

// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertPropertyToExpressionBody

namespace OpenNet.Orm.Sync.Entity
{
    [EntityTombstone]
    [SynchronizedEntity]
    // ReSharper disable once UnusedTypeParameter => Used by reflection
    public class EntityTombstone<TEntity, TIEntity> : SyncableEntity<IEntityTombstone>, IEntityTombstone
        where TIEntity : IDistinctableEntity
    {
        public const string ColumnNameDeletedAt = "deleted_at";
        private IRepository<IEntityTombstone> _repository;

        public override IRepository<IEntityTombstone> Repository
        {
            get {return _repository; }
            set { _repository = value; }
        }

        public EntityTombstone() { }

        public EntityTombstone(IRepository<IEntityTombstone> entityTombstoneRepo, TIEntity deletedEntity, DateTime deathTime)
        {
            _repository = entityTombstoneRepo;
            Id = deletedEntity.Id;
            CreatedAt = deathTime;
            DeletedAt = deathTime;
        }

        /// <summary>
        /// Get unique object identifier
        /// </summary>
        [PrimaryKey(FieldName = IdColumnName)]
        public new long Id { get; set; }

        /// <summary>
        /// Get entity R.I.P timestamp
        /// </summary>
        [Field(FieldName = ColumnNameDeletedAt, IsDeletionTracking = true, SearchOrder = FieldSearchOrder.Ascending)]
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// Return true if current entity if a tombstone's syncable entity
        /// </summary>
        public override bool IsTombstone { get { return true; } }
    }
}
