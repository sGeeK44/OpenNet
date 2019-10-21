﻿using System.Linq;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity(NameInStore = TableName)]
    [SynchronizedEntity(EntityTombstoneType = typeof(EntityTombstone<EntitySync, IEntitySync>), ClientRetentionTime = RentionTimeClient)]
    public class EntitySync : SyncableEntity<IEntitySync>, IEntitySync
    {
        public const int RentionTimeClient = 10;
        public const string TableName = "entity";
        public const string ColumnNameUniqueIdentifier = "unique_identifier";
        public const string ColumnNameStringField = "string_field";

        public EntitySync()
        {
        }

        public EntitySync(ISyncable syncable)
            : base(syncable) { }

        [Field(FieldName = ColumnNameUniqueIdentifier, AllowsNulls = true, RequireUniqueValue = true)]
        public string UniqueIdentifier { get; set; }

        [Field(FieldName = ColumnNameStringField, AllowsNulls = true)]
        public string StringField { get; set; }

        /// <summary>
        /// Determine if specified entity represente same entity as current (Even if Id is not same)
        /// </summary>
        /// <param name="remoteEntity">Remote entity</param>
        /// <returns>True if entity is same, false else</returns>
        public override bool IsSameEntity(object remoteEntity)
        {
            var remote = remoteEntity as EntitySync;
            if (remote == null)
                return false;

            return UniqueIdentifier == remote.UniqueIdentifier;
        }

        /// <summary>
        /// Should return existing entity on uniq constraint
        /// </summary>
        public override ISyncable FindDuplicateEntity(IDataStore storeToMadeSearch)
        {
            var indexCondition = storeToMadeSearch.Condition<EntitySync>(ColumnNameUniqueIdentifier, UniqueIdentifier, FilterOperator.Equals);
            return storeToMadeSearch.Select<EntitySync, ISyncable>().Where(indexCondition).GetValues().FirstOrDefault();
        }

        /// <summary>
        /// Loking for current entity in specified datastore
        /// </summary>
        /// <param name="dataStore"></param>
        /// <returns></returns>
        public EntitySync GetOnSpecifiedDatastore(IDataStore dataStore)
        {
            var condition = dataStore.Condition<EntitySync>(ColumnNameUniqueIdentifier, UniqueIdentifier, FilterOperator.Equals);
            return dataStore.Select<EntitySync>().Where(condition).GetValues().FirstOrDefault();
        }
    }
}
