using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Sync.Conflicts
{
    public interface IEntityConflictSolver
    {
        /// <summary>
        /// Used to provide access to sync info session
        /// </summary>
        ISyncSessionInfo SyncSessionInfo { get; set; }

        /// <summary>
        /// Compute result of conflict between both entity
        /// </summary>
        /// <param name="localDataStore">A ref to local datastore to apply database resolution</param>
        /// <param name="local">Value of entity in local datastore</param>
        /// <param name="remote">Value of entity in remote datastore</param>
        /// <returns>A merge result that contains remote operation to run to solve conflict</returns>
        RemoteMergeResolution Merge(IDataStore localDataStore, object local, object remote);
    }
}