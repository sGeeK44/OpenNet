using System;
using System.Linq;

namespace OpenNet.Orm.Sync.Entity
{
    /// <summary>
    /// Mark an entity as synchronized
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SynchronizedEntityAttribute : Attribute
    {
        private const SyncDirection DefaultDirection = SyncDirection.TwoWay;
        private const int NoClean = -1;

        /// <summary>
        /// Create Sync properties with default behavior
        /// </summary>
        public SynchronizedEntityAttribute()
        {
            Direction = DefaultDirection;
            ClientRetentionTime = NoClean;
        }

        /// <summary>
        /// Specify direction for synchronization. Default value is TwoWay.
        /// </summary>
        public SyncDirection Direction { get; set; }
        /// <summary>
        /// Type of entity to manage entity deletion
        /// </summary>
        public Type EntityTombstoneType { get; set; }

        /// <summary>
        /// Type of class to delegate conflict resolution
        /// </summary>
        public Type ConflicSolver { get; set; }

        /// <summary>
        /// Specify a delay in day after that data on client are cleaned in end of sync.
        /// By default no clean is done
        /// </summary>
        public int ClientRetentionTime { get; set; }

        /// <summary>
        /// Create a new conflict from specified Conflict Solver type
        /// </summary>
        /// <returns>New solver if ovverided, else null</returns>
        public object CreateSolver()
        {
            if (ConflicSolver == null)
                return null;

            var constructor = ConflicSolver.GetConstructors().First();
            return constructor.Invoke(new object[0]);
        }
    }
}
