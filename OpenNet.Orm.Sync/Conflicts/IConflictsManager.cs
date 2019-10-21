using OpenNet.Orm.Sync.Changes;

namespace OpenNet.Orm.Sync.Conflicts
{
    public interface IConflictsManager
    {
        IEntityConflict CreateEntityConflict(EntityChangeset entityChangeset, ISyncSessionInfo syncSessionInfo);
        int Count { get; }
        void AddNewIdentityChange(IdentityChange identityChange);
        void ApplyForeignKeyChange(EntityChange entity);
        bool HasForeignKeyDeleted(EntityChange entity);
    }
}