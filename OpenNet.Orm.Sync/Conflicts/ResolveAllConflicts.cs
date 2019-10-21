using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Changes;

namespace OpenNet.Orm.Sync.Conflicts
{
    public class ResolveAllConflicts : IConflictsManager
    {
        private readonly List<IdentityChange> _primaryKeyChanges;
        private readonly List<EntityChange> _primaryKeyDeleted;
        private bool _hasForeignKeyDeleted;

        public ResolveAllConflicts()
        {
            _primaryKeyChanges = new List<IdentityChange>();
            _primaryKeyDeleted = new List<EntityChange>();
            EntitiesConflict = new List<SolvableEntityConflict>();
        }

        public int Count { get { return EntitiesConflict.SelectMany(_ => _.Conflicts).Count(); } }

        public List<SolvableEntityConflict> EntitiesConflict { get; set; }

        public bool ShouldFullSync { get { return _primaryKeyChanges.Any() || _hasForeignKeyDeleted; } }

        public void AddNewIdentityChange(IdentityChange identityChange)
        {
            if (identityChange == null)
                return;

            _primaryKeyChanges.Add(identityChange);
        }

        public void ApplyForeignKeyChange(EntityChange entity)
        {
            foreach (var identityChange in _primaryKeyChanges)
            {
                var foreignKey = entity.GetForeignKeyOf(identityChange.EntityName);
                if (foreignKey == null)
                    continue;

                foreignKey.ApplyForeignKeyChange(identityChange);
            }
        }

        public bool HasForeignKeyDeleted(EntityChange entity)
        {
            _hasForeignKeyDeleted = _primaryKeyDeleted.Any(
                _ =>
                {
                    var entityNameDeleted = _.GetEntityNameFromTombstone();
                    var entityForeignKey = entity.GetForeignKeyOf(entityNameDeleted);
                    var hasForeignKey = entityForeignKey != null;
                    if (!hasForeignKey)
                        return false;

                    var deletedPrimaryId = _.GetPrimaryKeyValue();
                    return entityForeignKey.IsValueEquals(deletedPrimaryId);
                });

            return _hasForeignKeyDeleted;
        }

        public void AddDeletedEntityChange(EntitiesChangeset localEntitiesChangeset)
        {
            var entityDeleted = localEntitiesChangeset.EntityChangeset.SelectMany(_ => _.Delete).ToList();
            _primaryKeyDeleted.AddRange(entityDeleted);
        }

        public IEntityConflict CreateEntityConflict(EntityChangeset entityChangeset, ISyncSessionInfo syncSessionInfo)
        {
            var entityInfo = entityChangeset.GetSyncableEntity();
            var entityConflict = new SolvableEntityConflict(entityInfo, syncSessionInfo);
            EntitiesConflict.Add(entityConflict);
            return entityConflict;
        }

        public void RemoveInvolve(EntitiesChangeset localChange)
        {
            foreach (var entityConflict in EntitiesConflict)
            {
                var entityChangeset = localChange.EntityChangeset.FirstOrDefault(_ => _.EntityName == entityConflict.EntityName);
                entityConflict.RemoveInvole(entityChangeset);
            }
        }

        public void ApplyRemoteResolution(ISqlDataStore dataStore, ISyncSessionInfo syncSession, ISyncStatProvider statProvider)
        {
            if (EntitiesConflict == null || EntitiesConflict.Count == 0)
                return;

            var progression = new LinearProgression(EntitiesConflict.Count);
            statProvider.SetNewState(SyncStates.ApplyRemoteResolution, progression);
            
            foreach (var entityConflict in EntitiesConflict)
            {
                entityConflict.ApplyRemoteResolution(dataStore, syncSession);
                progression.CurrentStepFinished();
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResolveAllConflicts);
        }

        protected bool Equals(ResolveAllConflicts other)
        {
            if (other == null)
                return false;

            return EntitiesConflict.IsEquals(other.EntitiesConflict);
        }

        public override int GetHashCode()
        {
            return EntitiesConflict != null ? EntitiesConflict.GetHashCode() : 0;
        }
    }
}