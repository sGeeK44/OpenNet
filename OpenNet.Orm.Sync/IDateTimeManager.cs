using System;

namespace OpenNet.Orm.Sync
{
    public interface IDateTimeManager
    {
        /// <summary>
        /// Get current utc time
        /// </summary>
        DateTime UtcNow { get; }
    }
}