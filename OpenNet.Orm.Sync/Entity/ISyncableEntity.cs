using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Sync.Entity
{
    public interface ISyncableEntity : IEntityInfo
    {
        /// <summary>
        /// Get column name that served to sync framework to detect inserted row
        /// </summary>
        string CreationTrackingColumn { get; }

        /// <summary>
        /// Get column name that served to sync framework to detect updated row
        /// </summary>
        string UpdateTrackingColumn { get; }

        /// <summary>
        /// Get column name of tombstone table that served to sync framework to detect deleted row
        /// </summary>
        string DeletionTrackingColumn { get; }

        /// <summary>
        /// Get column name that served to sync framework to detect updated row during last sync
        /// </summary>
        string LastSyncTrackingColumn { get; }

        /// <summary>
        /// Return true if current entity has tombstone table
        /// </summary>
        bool IsDeleteTrackEnable { get; }

        /// <summary>
        /// Get entity info for associated tombstone entity
        /// </summary>
        ISyncableEntity EntityTombstoneInfo { get; }

        /// <summary>
        /// Get sync direction
        /// </summary>
        SyncDirection Direction { get; }
    }
}