using System;
using System.Diagnostics;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Interfaces;
// ReSharper disable MergeConditionalExpression
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sync
{
    [Entity(NameInStore = TableName)]
    public class SyncSessionInfo : EntityBase<ISyncSessionInfo>, ISyncSessionInfo
    {
        public const string TableName = "sync_info_session";
        public const string ColumnNameHighBoundaryAnchor = "high_boundary_anchor";
        public const string ColumnNameLowBoundaryAnchor = "low_boundary_anchor";
        public const string ColumnNameHasSuccess = "has_success";

        /// <summary>
        /// Get or set last anchor used in last sync
        /// </summary>
        [Field(FieldName = ColumnNameLowBoundaryAnchor, AllowsNulls = false)]
        public DateTime LowBoundaryAnchor { get; set; }

        /// <summary>
        /// Get or set datetime that contains actual end time limit
        /// </summary>
        [Field(FieldName = ColumnNameHighBoundaryAnchor, AllowsNulls = false)]
        public DateTime HighBoundaryAnchor { get; set; }

        /// <summary>
        /// Get or set state of sync for current session
        /// </summary>
        [Field(FieldName = ColumnNameHasSuccess)]
        public bool? HasSuccess { get; set; }

        public override string ToString()
        {
            return string.Format("LowBoundaryAnchor:{0}. HighBoundaryAnchor:{1}. Success:{2}.", LowBoundaryAnchor, HighBoundaryAnchor, HasSuccess);
        }

        /// <summary>
        /// Create a new sync session
        /// </summary>
        /// <param name="dataStore">Datastore used to getting last sync info</param>
        /// <param name="dateTimeManager">Object used to get datetime value</param>
        /// <returns>New sync session</returns>
        public static SyncSessionInfo Create(IDataStore dataStore, IDateTimeManager dateTimeManager)
        {
            var syncSessionRepo = new SyncSessionInfoRepository(dataStore);
            var last = syncSessionRepo.GetLastSession();
            return Create(dateTimeManager, last);
        }

        /// <summary>
        /// Create a new sync session
        /// </summary>
        /// <param name="dateTimeManager">Object used to get datetime value</param>
        /// <param name="last">Last sync info</param>
        /// <returns>New sync session</returns>
        public static SyncSessionInfo Create(IDateTimeManager dateTimeManager, ISyncSessionInfo last)
        {
            var lowBoundary = last != null
                ? last.HighBoundaryAnchor
                : new DateTime(1973, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var result = new SyncSessionInfo
            {
                LowBoundaryAnchor = lowBoundary,
                HighBoundaryAnchor = dateTimeManager.UtcNow
            };
            
            return result;
        }
    }
}