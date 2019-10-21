using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Sync.Conflicts
{
    public class DefaultEntitySolver<T> : IEntityConflictSolver
        where T : class, ISyncable
    {
        public ISyncSessionInfo SyncSessionInfo { get; set; }

        public T RemoteEntity { get; set; }

        public T LocalEntity { get; set; }

        public IDataStore LocalDataStore { get; set; }

        /// <summary>
        /// Compute result of conflict between both entity
        /// </summary>
        /// <param name="localDataStore"></param>
        /// <param name="local">Value of entity in local datastore</param>
        /// <param name="remote">Value of entity in remote datastore</param>
        /// <returns>Value of entity to put in remote and local datastore</returns>
        public RemoteMergeResolution Merge(IDataStore localDataStore, object local, object remote)
        {
            LocalEntity = (T)local;
            RemoteEntity = (T)remote;
            LocalDataStore = localDataStore;

            var remoteMergeResolution = new RemoteMergeResolution();
            if (LocalEntity == null && RemoteEntity == null)
                    return remoteMergeResolution;

            if (RemoteEntity == null)
            {
                remoteMergeResolution.Insert(LocalDataStore, LocalEntity);
                return remoteMergeResolution;
            }

            RemoteEntity.LastSyncAt = SyncSessionInfo.HighBoundaryAnchor;

            if (LocalEntity != null && LocalEntity.IsSameEntity(RemoteEntity))
            {
                LocalEntity.LastSyncAt = SyncSessionInfo.HighBoundaryAnchor;
                remoteMergeResolution.Merge(LocalDataStore, LocalEntity, RemoteEntity);
                return remoteMergeResolution;
            }

            var duplicate = RemoteEntity.FindDuplicateEntity(LocalDataStore);
            if (duplicate != null)
            {
                duplicate.LastSyncAt = SyncSessionInfo.HighBoundaryAnchor;
                remoteMergeResolution.MergeAndKeepLocalIdentity(LocalDataStore, duplicate, RemoteEntity);
                return remoteMergeResolution;
            }

            if (LocalEntity == null)
            {
                RemoteMergeResolution.InsertInLocal(LocalDataStore, SyncSessionInfo, RemoteEntity);
                return remoteMergeResolution;
            }

            LocalEntity.LastSyncAt = SyncSessionInfo.HighBoundaryAnchor;
            remoteMergeResolution.MergeDistinctEntity(LocalDataStore, LocalEntity, RemoteEntity);
            return remoteMergeResolution;
        }
    }
}