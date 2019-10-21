using System;
using OpenNet.Orm.Entity;

namespace OpenNet.Orm.Sync
{
    public interface ISyncSessionInfo : IDistinctableEntity
    {
        /// <summary>
        /// Get or set last anchor used in last sync
        /// </summary>
        DateTime LowBoundaryAnchor { get; }

        /// <summary>
        /// Get or set datetime that contains actual end time limit
        /// </summary>
        DateTime HighBoundaryAnchor { get; }

        /// <summary>
        /// Get or set state of sync for current session
        /// </summary>
        bool? HasSuccess { get; set; }
    }
}