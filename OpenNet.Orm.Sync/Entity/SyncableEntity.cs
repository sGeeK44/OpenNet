using System;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Interfaces;

// ReSharper disable UseNullPropagation

namespace OpenNet.Orm.Sync.Entity
{
    public abstract class SyncableEntity<TIEntity> : EntityBase<TIEntity>, ISyncable
        where TIEntity : class
    {
        public const string ColumnNameCreatedAt = "created_at";
        public const string ColumnNameUpdatedAt = "updated_at";
        public const string ColumnNameLastSyncAt = "last_sync_at";

        protected SyncableEntity() { }

        protected SyncableEntity(ISyncable syncable)
            : base(syncable)
        {
            if (syncable == null)
                return;

            CreatedAt = syncable.CreatedAt;
            UpdatedAt = syncable.UpdatedAt;
        }

        /// <summary>
        /// Get entity creation timestamp
        /// </summary>
        [Field(FieldName = ColumnNameCreatedAt, AllowsNulls = true, IsCreationTracking = true, SearchOrder = FieldSearchOrder.Ascending)]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Get entity last update timestamp
        /// </summary>
        [Field(FieldName = ColumnNameUpdatedAt, AllowsNulls = true, IsUpdateTracking = true, SearchOrder = FieldSearchOrder.Ascending)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Get entity last sync timestamp
        /// </summary>
        [Field(FieldName = ColumnNameLastSyncAt, AllowsNulls = true, IsLastSyncTracking = true, SearchOrder = FieldSearchOrder.Ascending)]
        public DateTime? LastSyncAt { get; set; }

        /// <summary>
        /// Return true if current entity if a tombstone's syncable entity
        /// </summary>
        public virtual bool IsTombstone { get { return false; } }

        public bool ShouldSync(object obj)
        {
            var type = obj.GetType();

            if (type != GetType())
                return false;

            foreach (var property in DbField)
            {
                var propValue = property.GetValue(this, null);
                var otherPropValue = property.GetValue(obj, null);
                if (propValue == null ^ otherPropValue == null
                 || propValue != null && !propValue.Equals(otherPropValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Should return existing entity on uniq constraint
        /// </summary>
        public virtual ISyncable FindDuplicateEntity(IDataStore storeToMadeSearch)
        {
            return null;
        }

        /// <summary>
        /// Determine if specified entity represente same entity as current (Even if Id is not same)
        /// </summary>
        /// <param name="remoteEntity">Remote entity</param>
        /// <returns>True if entity is same, false else</returns>
        public virtual bool IsSameEntity(object remoteEntity)
        {
            var remote = remoteEntity as SyncableEntity<TIEntity>;
            if (remote == null)
                return false;

            return Id == remote.Id;
        }

        /// <summary>
        /// Merge current entity state with remote entity state
        /// </summary>
        /// <param name="locaDataStore">Datastore where current object was get</param>
        /// <param name="remoteEntity">Remote entity</param>
        public virtual void MergeWithRemoteValue(IDataStore locaDataStore, object remoteEntity)
        {
            var remoteSyncable = remoteEntity as ISyncable;
            if (remoteSyncable == null)
                return;

            object source, dest;
            if (UpdatedAt < remoteSyncable.UpdatedAt)
            {
                source = remoteEntity;
                dest = this;
            }
            else
            {
                source = this;
                dest = remoteEntity;
            }

            var currentId = Id;
            foreach (var property in DbField)
            {
                var sourcePropertyValue = property.GetValue(source, null);
                property.SetValue(dest, sourcePropertyValue, null);
            }
            Id = currentId;
        }

        /// <summary>
        /// Create new instance that be a copy (All db prop except Id) of current.
        /// </summary>
        /// <remarks>Id will initialize to NullId</remarks>
        /// <returns>New instance</returns>
        public object Clone()
        {
            var clone = (EntityBase<TIEntity>)Activator.CreateInstance(GetType());
            foreach (var property in DbField)
            {
                var propValue = property.GetValue(this, null);
                property.SetValue(clone, propValue, null);
            }
            clone.Id = NullId;
            return clone;
        }
    }
}
