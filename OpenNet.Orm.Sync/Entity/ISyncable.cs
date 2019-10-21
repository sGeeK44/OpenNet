using System;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Sync.Entity
{
    public interface ISyncable : IDistinctableEntity, ICloneable, IEntity
    {
        /// <summary>
        /// Get entity creation timestamp 
        /// </summary>
        DateTime? CreatedAt { get; }

        /// <summary>
        /// Get entity last update timestamp 
        /// </summary>
        DateTime? UpdatedAt { get; }

        /// <summary>
        /// Get entity last sync timestamp 
        /// </summary>
        DateTime? LastSyncAt { get; set; }

        /// <summary>
        /// Determine if specified entity represente same entity as current (Even if Id is not same)
        /// </summary>
        /// <param name="remoteEntity">Remote entity</param>
        /// <returns>True if entity is same, false else</returns>
        bool IsSameEntity(object remoteEntity);

        /// <summary>
        /// Merge current entity state with remote entity state
        /// </summary>
        /// <param name="locaDataStore">Datastore where current object was get</param>
        /// <param name="remoteEntity">Remote entity</param>
        void MergeWithRemoteValue(IDataStore locaDataStore, object remoteEntity);

        /// <summary>
        /// Indicate if specified object differ than current.
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if object differt and should be sync, false else</returns>
        bool ShouldSync(object obj);

        /// <summary>
        /// Should return existing entity on uniq constraint
        /// </summary>
        ISyncable FindDuplicateEntity(IDataStore storeToMadeSearch);
    }
}
