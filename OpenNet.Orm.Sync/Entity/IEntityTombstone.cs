using System;
using OpenNet.Orm.Entity;

namespace OpenNet.Orm.Sync.Entity
{
    public interface IEntityTombstone : IDistinctableEntity
    {
        /// <summary>
        /// Get entity R.I.P timestamp 
        /// </summary>
        DateTime DeletedAt { get; set; }
    }
}