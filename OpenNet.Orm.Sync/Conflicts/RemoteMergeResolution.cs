using System.Collections.Generic;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm.Sync.Conflicts
{
    public class RemoteMergeResolution
    {
        private IdentityChange _identityChange;
        public List<MergeResolution> PreResolutionToApplies { get; set; }
        public List<MergeResolution> MergeResolutionToApplies { get; set; }

        public RemoteMergeResolution()
        {
            PreResolutionToApplies = new List<MergeResolution>();
            MergeResolutionToApplies = new List<MergeResolution>();
        }

        public IdentityChange GetIdentityChange()
        {
            return _identityChange;
        }

        public void Insert(IDataStore datastore, IEntity entity)
        {
            CreateAndAddMergeResolution(datastore, OperationTypes.Insert, entity);
        }

        public void Merge(IDataStore dataStore, ISyncable localEntity, ISyncable remoteEntity)
        {
            localEntity.MergeWithRemoteValue(dataStore, remoteEntity);
            dataStore.Update(localEntity);
            CreateAndAddMergeResolution(dataStore, OperationTypes.Update, localEntity);
        }

        public void MergeDistinctEntity(IDataStore dataStore, ISyncable localEntity, ISyncable remoteEntity)
        {
            var idBeforeMerge = remoteEntity.Id;
            dataStore.Insert(remoteEntity);
            CreateIdentityChange(dataStore, remoteEntity, idBeforeMerge, remoteEntity.Id);
            CreateAndAddMergeResolution(dataStore, OperationTypes.Update, localEntity);
            CreateAndAddMergeResolution(dataStore, OperationTypes.Insert, remoteEntity);
        }

        public void MergeAndKeepLocalIdentity(IDataStore dataStore, ISyncable localEntity, ISyncable remoteEntity)
        {
            var entityInfo = CreateIdentityChange(dataStore, remoteEntity, remoteEntity.Id, localEntity.Id);
            localEntity.MergeWithRemoteValue(dataStore, remoteEntity);
            dataStore.Update(localEntity);
            CreateAndAddPreResolution(entityInfo, OperationTypes.Delete, remoteEntity);
            CreateAndAddMergeResolution(entityInfo, OperationTypes.Insert, localEntity);
        }

        private IEntityInfo CreateIdentityChange(IDataStore dataStore, ISyncable remoteEntity, long idBeforeMerge, long idAfterMerge)
        {
            var entityInfo = dataStore.GetEntityInfo(remoteEntity);
            _identityChange = new IdentityChange
            {
                EntityName = entityInfo.GetNameInStore(),
                OldValue = idBeforeMerge,
                NewValue = idAfterMerge
            };
            return entityInfo;
        }

        private void CreateAndAddPreResolution(IEntityInfo entityInfo, OperationTypes operationType, IEntity entity)
        {
            var resolution = MergeResolution.Create(entityInfo, operationType, entity);
            if (resolution == null)
                return;

            PreResolutionToApplies.Insert(0, resolution);
        }

        private void CreateAndAddMergeResolution(IDataStore datastore, OperationTypes operationType, IEntity entity)
        {
            var entityInfo = datastore.GetEntityInfo(entity);
            CreateAndAddMergeResolution(entityInfo, operationType, entity);
        }

        private void CreateAndAddMergeResolution(IEntityInfo entityInfo, OperationTypes operationType, IEntity entity)
        {
            var resolution = MergeResolution.Create(entityInfo, operationType, entity);
            if (resolution == null)
                return;

            MergeResolutionToApplies.Add(resolution);
        }

        public void ApplyPreResolution(ISqlDataStore datastore, ISyncSessionInfo syncSessionInfo)
        {
            foreach (var mergeResolution in PreResolutionToApplies)
            {
                mergeResolution.ApplyResolution(datastore, syncSessionInfo);
            }
        }

        public void ApplyResolution(ISqlDataStore datastore, ISyncSessionInfo syncSessionInfo)
        {
            foreach (var mergeResolution in MergeResolutionToApplies)
            {
                mergeResolution.ApplyResolution(datastore, syncSessionInfo);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RemoteMergeResolution);
        }

        protected bool Equals(RemoteMergeResolution other)
        {
            if (other == null)
                return false;

            return MergeResolutionToApplies.IsEquals(other.MergeResolutionToApplies);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return MergeResolutionToApplies != null ? MergeResolutionToApplies.GetHashCode() : 0;
            }
        }

        public static void InsertInLocal(IDataStore localDataStore, ISyncSessionInfo syncSessionInfo, ISyncable remoteEntity)
        {
            var entityInfo = localDataStore.GetEntityInfo(remoteEntity);

            var mergeResolution = new MergeResolution
            {
                OperationType = OperationTypes.Insert,
                Entity = EntityChange.Create(entityInfo, remoteEntity)
            };
            mergeResolution.ApplyResolution(localDataStore as ISqlDataStore, syncSessionInfo);
        }
    }
}