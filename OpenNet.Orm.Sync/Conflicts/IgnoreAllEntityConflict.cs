using System.Diagnostics;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sync.Changes;

namespace OpenNet.Orm.Sync.Conflicts
{
    public class IgnoreAllEntityConflict : IEntityConflict
    {
        public void OnApplyUpdate(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine("Unattented Conflict occurs when updated entity.");
        }

        public void OnApplyUpdateExistingUpdatedToo(EntityChange local, EntityChange remote) { }

        public void OnApplyInsert(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine("Unattented Conflict occurs when inserted entity.");
        }

        public void OnApplyInsertAlreadyExisting(EntityChange local, EntityChange remote) { }

        public void OnApplyDelete(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine("Unattented Conflict occurs when deleted entity.");
        }

        public void OnApplyDeleteExistingUpdated(EntityChange local) { }

        public void OnApplyDeleteExistingInserted(EntityChange local) { }

        public void OnApplyInsertDeletedOnRemote(EntityChange remote) { }

        public void ResolveConflicts(IDataStore localDataStore, IConflictsManager conflictsManager) { }
    }
}