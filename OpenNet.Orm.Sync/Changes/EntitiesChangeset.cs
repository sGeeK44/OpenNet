using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Conflicts;
using OpenNet.Orm.Sync.Entity;
// ReSharper disable UseNameofExpression
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace OpenNet.Orm.Sync.Changes
{
    public class EntitiesChangeset
    {
        private class ChangeSet
        {
            private readonly EntityChangeset _entityChangeset;
            private readonly IEntityConflict _conflic;
            private readonly IConflictsManager _conflictsManager;

            public ChangeSet(EntityChangeset entityChangeset, IEntityConflict conflic, IConflictsManager conflictsManager)
            {
                _entityChangeset = entityChangeset;
                _conflic = conflic;
                _conflictsManager = conflictsManager;
            }

            public void ApplyInsert()
            {
                _entityChangeset.ApplyInsert(_conflic, _conflictsManager);
            }

            public void ApplyUpdate()
            {
                _entityChangeset.ApplyUpdate(_conflic);
            }

            public void ApplyLastSync()
            {
                _entityChangeset.ApplyLastSync(_conflic);
            }

            public void ApplyDelete()
            {
                _entityChangeset.ApplyDelete(_conflic);
            }

            public void ResolveConflicts(IDataStore dataStore)
            {
                _conflic.ResolveConflicts(dataStore, _conflictsManager);
            }
        }
        private readonly ISqlDataStore _dataStore;
        private ISyncSessionInfo _syncSessionInfo;

        public List<EntityChangeset> EntityChangeset { get; set; }

        public EntitiesChangeset() { }

        public EntitiesChangeset(ISqlDataStore dataStore, ISyncSessionInfo syncSessionInfo)
        {
            _dataStore = dataStore;
            EntityChangeset = new List<EntityChangeset>();
            SetSyncSession(syncSessionInfo);
        }

        public void SetSyncSession(ISyncSessionInfo syncSessionInfo)
        {
            _syncSessionInfo = syncSessionInfo;
            foreach (var entityChangeset in EntityChangeset)
            {
                entityChangeset.SetSyncSession(syncSessionInfo);
            }
        }

        public void AddEntityChangeset(EntityChangeset entityChange)
        {
            EntityChangeset.Add(entityChange);
        }

        public void ApplyChanges(ISqlDataStore datastore, ISyncStatProvider statProvider, IConflictsManager conflictsManager)
        {
            if (EntityChangeset.Count == 0)
                return;

            var progression = new LinearProgression(EntityChangeset.Count * 3);
            statProvider.SetNewState(SyncStates.ApplyingRemoteChange, progression);
            
            var changeSets = new List<ChangeSet>();
            foreach (var entityChangeset in EntityChangeset)
            {
                entityChangeset.SetDatastore(datastore);
                var conflic = conflictsManager.CreateEntityConflict(entityChangeset, _syncSessionInfo);
                changeSets.Add(new ChangeSet(entityChangeset, conflic, conflictsManager));
            }

            // Update FK changes before delete
            foreach (var entityChange in changeSets)
            {
                entityChange.ApplyUpdate();
                progression.CurrentStepFinished();
            }

            // Delete FK before delete PK
            changeSets.Reverse();
            foreach (var entityChange in changeSets)
            {
                entityChange.ApplyDelete();
                progression.CurrentStepFinished();
            }

            // Insert PK before insert FK
            changeSets.Reverse();
            foreach (var entityChange in changeSets)
            {
                entityChange.ApplyInsert();
                entityChange.ApplyLastSync();
                entityChange.ResolveConflicts(datastore);
                progression.CurrentStepFinished();
            }
        }

        public void Build(SyncStatProvider statProvider)
        {
            var syncableEntities = _dataStore.Entities
                // ReSharper disable once RedundantTypeArgumentsOfMethod Need for vs 2008
                                            .Select<IEntityInfo, SyncEntity>(SyncEntity.Create)
                                            .Where(_ => _ != null && !ShouldSkipLocalChange(_))
                                            .ToList();

            var progression = new LinearProgression(syncableEntities.Count);
            statProvider.SetNewState(GetStepName(), progression);
            foreach (var syncableEntity in syncableEntities)
            {
                var entityName = syncableEntity.GetNameInStore();
                var entityChange = CreateEntityChangeset(syncableEntity);
                entityChange.Populate();
                statProvider.Info("Local change ({0}) {1}", entityName, entityChange);
                progression.CurrentStepFinished();
            }

            statProvider.SetLocalChanges(this);
        }

        protected virtual SyncStates GetStepName()
        {
            return SyncStates.ComputeLocalChange;
        }

        protected virtual bool ShouldSkipLocalChange(SyncEntity syncableEntity)
        {
            return false;
        }

        private EntityChangeset CreateEntityChangeset(IEntityInfo syncEntity)
        {
            var newEntityChangeset = Changes.EntityChangeset.Create(_dataStore, syncEntity, _syncSessionInfo);
            AddEntityChangeset(newEntityChangeset);
            return newEntityChangeset;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntitiesChangeset);
        }

        protected bool Equals(EntitiesChangeset other)
        {
            if (other == null)
                return false;

            return EntityChangeset.IsEquals(other.EntityChangeset);
        }

        public override int GetHashCode()
        {
            return EntityChangeset != null ? EntityChangeset.GetHashCode() : 0;
        }
    }
}