using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Changes;

namespace OpenNet.Orm.Sync.Conflicts
{
    public interface IEntityConflict
    {
        void OnApplyUpdate(EntityChange local, EntityChange remote);
        void OnApplyUpdateExistingUpdatedToo(EntityChange local, EntityChange remote);
        void OnApplyInsert(EntityChange local, EntityChange remote);
        void OnApplyInsertAlreadyExisting(EntityChange local, EntityChange remote);
        void OnApplyDelete(EntityChange local, EntityChange remote);
        void OnApplyDeleteExistingUpdated(EntityChange local);
        void OnApplyDeleteExistingInserted(EntityChange local);
        void OnApplyInsertDeletedOnRemote(EntityChange remote);
        void ResolveConflicts(IDataStore localDataStore, IConflictsManager conflictsManager);
    }
}