using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace OpenNet.Orm.Sync.Conflicts
{
    public class Conflict
    {
        private readonly ISyncSessionInfo _syncSessionInfo;
        private readonly IEntityInfo _entityInfo;
        private readonly EntityChange _local;
        private readonly EntityChange _remote;
        private bool _alreadyMerge;
        public RemoteMergeResolution RemoteMergeResolution { get; set; }

        public Conflict() { }

        public Conflict(IEntityInfo entityInfo, EntityChange local, EntityChange remote, ISyncSessionInfo syncSessionInfo)
        {
            _entityInfo = entityInfo;
            _local = local;
            _remote = remote;
            _syncSessionInfo = syncSessionInfo;
        }

        public EntityChange RemoteChange { get { return _remote; } }

        public IdentityChange Resolve(IDataStore localDataStore)
        {
            if (_alreadyMerge)
                return null;

            var syncableEntity = SyncEntity.Create(_entityInfo);
            var solver = syncableEntity.GetSolver() as IEntityConflictSolver ?? new DefaultEntitySolver<ISyncable>();
            solver.SyncSessionInfo = _syncSessionInfo;

            var serializer = _entityInfo.GetSerializer();
            var localEntity = serializer.Deserialize(_local);
            var remoteEntity = serializer.Deserialize(_remote);
            RemoteMergeResolution = solver.Merge(localDataStore, localEntity, remoteEntity);
            _alreadyMerge = true;
            return RemoteMergeResolution.GetIdentityChange();
        }

        public void ApplyRemotePreResolution(ISqlDataStore datastore, ISyncSessionInfo syncSessionInfo)
        {
            if (RemoteMergeResolution == null)
                return;

            RemoteMergeResolution.ApplyPreResolution(datastore, syncSessionInfo);
        }

        public void ApplyRemoteResolution(ISqlDataStore datastore, ISyncSessionInfo syncSessionInfo)
        {
            if (RemoteMergeResolution == null)
                return;

            RemoteMergeResolution.ApplyResolution(datastore, syncSessionInfo);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Conflict);
        }

        protected bool Equals(Conflict other)
        {
            if (other == null)
                return false;

            return Equals(RemoteMergeResolution, other.RemoteMergeResolution);
        }

        public override int GetHashCode()
        {
            return RemoteMergeResolution != null ? RemoteMergeResolution.GetHashCode() : 0;
        }
    }
}