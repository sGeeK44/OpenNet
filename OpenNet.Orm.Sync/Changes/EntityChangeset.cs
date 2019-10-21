using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Conflicts;
using OpenNet.Orm.Sync.Entity;
using OpenNet.Orm.Sync.SyncQueries;
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable UseStringInterpolation
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable ArrangeAccessorOwnerBody

namespace OpenNet.Orm.Sync.Changes
{
    public class EntityChangeset
    {
        private EntityChangesetBuilder _entitySerialiazer;
        private EntityChangesetBuilder _entityTombstoneSerialiazer;
        private ISqlDataStore _datastore;
        private ISyncSessionInfo _syncSession;

        public EntityChangeset(string entityName)
        {
            Insert = new List<EntityChange>();
            Update = new List<EntityChange>();
            Delete = new List<EntityChange>();
            LastSync = new List<EntityChange>();
            EntityName = entityName;
        }

        public string EntityName { get; set; }
        public List<EntityChange> Insert { get; set; }
        public List<EntityChange> Update { get; set; }
        public List<EntityChange> Delete { get; set; }
        public List<EntityChange> LastSync { get; set; }

        public void SetSyncSession(ISyncSessionInfo syncSessionInfo)
        {
            _syncSession = syncSessionInfo;
        }

        public void SetDatastore(ISqlDataStore value)
        {
            _datastore = value;
            var entityInfo = GetSyncableEntity();
            _entitySerialiazer = new EntityChangesetBuilder(entityInfo, _syncSession);

            if (entityInfo.IsDeleteTrackEnable)
                _entityTombstoneSerialiazer = new EntityChangesetBuilder(entityInfo.EntityTombstoneInfo, _syncSession);
        }

        public void Populate()
        {
            var entity = GetSyncableEntity();
            OrmDebug.Trace("");
            PopulateInsert(entity);
            OrmDebug.Trace(GetChangeDisplay("Inserted", Insert));
            PopulateUpdate(entity);
            OrmDebug.Trace(GetChangeDisplay("Updated", Update));
            if (entity.IsDeleteTrackEnable)
            {
                PopulateDelete(entity);
                OrmDebug.Trace(GetChangeDisplay("Deleted", Delete));
            }
            PopulateLastSync(entity);
            OrmDebug.Trace(GetChangeDisplay("LastSync", Update));
        }

        private static string GetChangeDisplay(string changeName, List<EntityChange> changes)
        {
            var result = new StringBuilder();
            result.AppendFormat("\t\t{0} :{1}\n", changeName, changes.Count);
            foreach (var change in changes)
            {
                result.AppendFormat("\t\t\t{0}\n", change);
            }
            return result.ToString();
        }

        private void PopulateInsert(ISyncableEntity entity)
        {
            var query = new InsertSyncQuery(_datastore, entity, _syncSession);
            Insert = _datastore.ExecuteQuery(query, _entitySerialiazer).ToList();
        }

        private void PopulateUpdate(ISyncableEntity entity)
        {
            var query = new UpdateSyncQuery(_datastore, entity, _syncSession);
            Update = _datastore.ExecuteQuery(query, _entitySerialiazer)
                               .Where(IsNotInserted)
                               .ToList();
        }

        private void PopulateLastSync(ISyncableEntity entity)
        {
            var query = new LastSyncQuery(_datastore, entity, _syncSession);
            LastSync = _datastore.ExecuteQuery(query, _entitySerialiazer)
                               .Where(IsNotInserted)
                               .Where(IsNotUpdated)
                               .ToList();
        }

        private void PopulateDelete(ISyncableEntity entity)
        {
            var query = new DeleteSyncQuery(_datastore, entity, _syncSession);
            Delete = _datastore.ExecuteQuery(query, _entityTombstoneSerialiazer).ToList();
        }

        private bool IsNotInserted(EntityChange entityChange)
        {
            return Insert.FirstOrDefault(_ => _.IsSameEntity(entityChange)) == null;
        }

        private bool IsNotUpdated(EntityChange entityChange)
        {
            return Update.FirstOrDefault(_ => _.IsSameEntity(entityChange)) == null;
        }

        public ISyncableEntity GetSyncableEntity()
        {
            var entity = _datastore.Entities[EntityName];
            return SyncEntity.Create(entity);
        }

        private EntityChange FindExisting(EntityChange entity)
        {
            var command = _entitySerialiazer.FindExisting(_datastore, entity);
            var result = _datastore.ExecuteReader(command, _entitySerialiazer).FirstOrDefault();
            return result;
        }

        private EntityChange FindDeleted(EntityChange entity)
        {
            if (_entityTombstoneSerialiazer == null)
                return null;

            var command = _entityTombstoneSerialiazer.FindExisting(_datastore, entity);
            var result = _datastore.ExecuteReader(command, _entityTombstoneSerialiazer).FirstOrDefault();
            return result;
        }

        public void ApplyInsert(IEntityConflict entityConflict, IConflictsManager conflictsManager)
        {
            foreach (var row in Insert)
            {
                conflictsManager.ApplyForeignKeyChange(row);
                var existing = FindExisting(row);
                if (existing != null)
                {
                    entityConflict.OnApplyInsertAlreadyExisting(existing, row);
                    continue;
                }

                if (conflictsManager.HasForeignKeyDeleted(row))
                {
                    continue;
                }

                ApplyInsertWhenNoExistInLocal(entityConflict, row);
            }
        }

        public void ApplyUpdate(IEntityConflict entityConflict)
        {
            var entity = GetSyncableEntity();
            foreach (var row in Update)
            {
                var existing = FindExisting(row);
                if (existing == null)
                    ApplyInsertWhenNoExistInLocal(entityConflict, row);
                else
                    ApplyUpdateWhenExistInLocal(entityConflict, existing, entity, row);
            }
        }

        public void ApplyDelete(IEntityConflict entityConflict)
        {
            var entity = GetSyncableEntity();
            foreach (var row in Delete)
            {
                var existing = FindExisting(row);
                if (existing == null)
                {
                    OrmDebug.Trace(string.Format("Entity:{0}. deleted on remote but does not exist locally.", EntityName));
                    continue;
                }

                ApplyDeletetWhenExistInLocal(entityConflict, existing, entity, row);
            }
        }

        public void ApplyLastSync(IEntityConflict entityConflict)
        {
            var entity = GetSyncableEntity();
            foreach (var row in LastSync)
            {
                var existing = FindExisting(row);
                if (existing == null)
                {
                    ApplyInsertWhenNoExistInLocal(entityConflict, row);
                    continue;
                }

                ApplyUpdateWhenExistInLocal(entityConflict, existing, entity, row);
            }
        }

        private void ApplyInsertWhenNoExistInLocal(IEntityConflict entityConflict, EntityChange row)
        {
            var deleted = FindDeleted(row);
            if (deleted != null)
            {
                entityConflict.OnApplyInsertDeletedOnRemote(row);
                return;
            }

            try
            {
                row.ApplyInsert(_datastore, _entitySerialiazer);
            }
            catch
            {
                entityConflict.OnApplyInsert(null, row);
            }
        }

        private void ApplyUpdateWhenExistInLocal(IEntityConflict entityConflict, EntityChange existing, ISyncableEntity entity,
            EntityChange row)
        {
            if (HasChangedOnBothSide(existing, entity))
            {
                entityConflict.OnApplyUpdateExistingUpdatedToo(existing, row);
                return;
            }

            try
            {
                row.ApplyUpdate(_datastore, _entitySerialiazer);
            }
            catch
            {
                entityConflict.OnApplyUpdate(existing, row);
            }
        }

        private void ApplyDeletetWhenExistInLocal(IEntityConflict entityConflict, EntityChange existing, ISyncableEntity entity,
            EntityChange row)
        {
            if (HasInsertedOnTarget(existing, entity))
            {
                entityConflict.OnApplyDeleteExistingInserted(existing);
                return;
            }

            if (HasChangedOnBothSide(existing, entity))
            {
                entityConflict.OnApplyDeleteExistingUpdated(existing);
                return;
            }

            try
            {
                row.ApplyDelete(_datastore, EntityName, _entitySerialiazer);
            }
            catch
            {
                entityConflict.OnApplyDelete(existing, row);
            }
        }

        public void RemoveInvolve(List<Conflict> conflicts)
        {
            RemoveInvolve(Insert, conflicts);
            RemoveInvolve(Update, conflicts);
            RemoveInvolve(Delete, conflicts);
            RemoveInvolve(LastSync, conflicts);
        }

        public static EntityChangeset Create(ISqlDataStore dataStore, IEntityInfo syncEntity, ISyncSessionInfo syncSessionInfo)
        {
            var entityName = syncEntity.GetNameInStore();

            var result = new EntityChangeset(entityName) { _syncSession = syncSessionInfo };
            result.SetDatastore(dataStore);
            return result;
        }

        private bool HasInsertedOnTarget(EntityChange existing, ISyncableEntity entity)
        {
            return existing.HasChangedInSession(entity.CreationTrackingColumn, _syncSession);
        }

        private bool HasChangedOnBothSide(EntityChange existing, ISyncableEntity entity)
        {
            return existing.HasChangedInSession(entity.UpdateTrackingColumn, _syncSession)
                   || existing.HasChangedInSession(entity.LastSyncTrackingColumn, _syncSession);
        }

        private static void RemoveInvolve(ICollection<EntityChange> list, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                var remoteChange = conflict.RemoteChange;
                var item = list.FirstOrDefault(_ => _.IsSameEntity(remoteChange));
                list.Remove(item);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityChangeset);
        }

        protected bool Equals(EntityChangeset other)
        {
            if (other == null)
                return false;

            return string.Equals(EntityName, other.EntityName)
                && Insert.IsEquals(other.Insert)
                && Update.IsEquals(other.Update)
                && Delete.IsEquals(other.Delete)
                && Equals(_syncSession, other._syncSession);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EntityName != null ? EntityName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Insert != null ? Insert.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Update != null ? Update.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Delete != null ? Delete.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_syncSession != null ? _syncSession.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Inserted:{0}. Updated:{1}. Deleted:{2}.", Insert.Count, Update.Count, Delete.Count);
        }
    }
}