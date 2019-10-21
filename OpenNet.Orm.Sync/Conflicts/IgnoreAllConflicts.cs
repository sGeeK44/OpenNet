using OpenNet.Orm.Sync.Changes;

namespace OpenNet.Orm.Sync.Conflicts
{
    public class IgnoreAllConflicts : IConflictsManager
    {
        public IEntityConflict CreateEntityConflict(EntityChangeset entityChangeset, ISyncSessionInfo syncSessionInfo)
        {
            return new IgnoreAllEntityConflict();
        }

        public int Count { get { return 0; } }
        public void AddDeletedEntityChange(EntitiesChangeset localEntitiesChangeset) { }
        public void ApplyForeignKeyChange(EntityChange entity) { }
        public bool HasForeignKeyDeleted(EntityChange entity) { return false; }

        public void AddNewIdentityChange(IdentityChange identityChange) { }
    }
}