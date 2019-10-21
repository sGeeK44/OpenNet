 using System.Collections.Generic;
using System.Diagnostics;
using OpenNet.Orm.Interfaces;
 using OpenNet.Orm.Sql;
 using OpenNet.Orm.Sync.Changes;
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable MergeConditionalExpression
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sync.Conflicts
{
    public class SolvableEntityConflict : IEntityConflict
    {
        private readonly IEntityInfo _entityInfo;
        private readonly ISyncSessionInfo _syncSessionInfo;

        public SolvableEntityConflict()
        {
            Conflicts = new List<Conflict>();
        }

        public SolvableEntityConflict(IEntityInfo entityInfo, ISyncSessionInfo syncSessionInfo)
            : this()
        {
            _entityInfo = entityInfo;
            _syncSessionInfo = syncSessionInfo;
            EntityName = entityInfo.GetNameInStore();
        }

        public List<Conflict> Conflicts { get; set; }
        public string EntityName { get; set; }

        private void Add(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine(local != null ? local.Headers : remote.Headers);
            Debug.WriteLine(string.Format("Local value:{0}", local != null ? local.ToString() : "null"));
            Debug.WriteLine(string.Format("Remote value:{0}", remote != null ? remote.ToString() : "null"));
            Conflicts.Add(new Conflict(_entityInfo, local, remote, _syncSessionInfo));
        }

        public void OnApplyUpdate(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine(string.Format("Entity:{0}. updated on remote but failed to apply locally.", EntityName));
            Add(local, remote);
        }

        public void OnApplyUpdateExistingUpdatedToo(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine(string.Format("Entity:{0}. updated on remote and locally.", EntityName));
            Add(local, remote);
        }

        public void OnApplyInsert(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine(string.Format("Entity:{0}. inserted on remote but failed to apply locally.", EntityName));
            Add(local, remote);
        }

        public void OnApplyInsertAlreadyExisting(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine(string.Format("Entity:{0}. inserted on remote already exist locally.", EntityName));
            Add(local, remote);
        }

        public void OnApplyDelete(EntityChange local, EntityChange remote)
        {
            Debug.WriteLine(string.Format("Entity:{0}. deleted on remote failed to apply locally.", EntityName));
            Add(local, remote);
        }

        public void OnApplyDeleteExistingUpdated(EntityChange local)
        {
            Debug.WriteLine(string.Format("Entity:{0}. deleted on remote and updated locally.", EntityName));
            Add(local, null);
        }

        public void OnApplyDeleteExistingInserted(EntityChange local)
        {
            Debug.WriteLine(string.Format("Entity:{0}. deleted on remote and inserted locally.", EntityName));
            Add(local, null);
        }

        public void OnApplyInsertDeletedOnRemote(EntityChange remote)
        {
            Debug.WriteLine(string.Format("Entity:{0}. deleted on remote and inserted locally.", EntityName));
            Add(null, remote);
        }

        public void RemoveInvole(EntityChangeset entityChangeset)
        {
            if (entityChangeset == null)
                return;

            entityChangeset.RemoveInvolve(Conflicts);
        }

        public void ResolveConflicts(IDataStore localDataStore, IConflictsManager conflictsManager)
        {
            if (Conflicts == null || Conflicts.Count == 0)
                return;
            
            foreach (var conflict in Conflicts)
            {
                var identityChange = conflict.Resolve(localDataStore);
                conflictsManager.AddNewIdentityChange(identityChange);
            }
        }

        public void ApplyRemoteResolution(ISqlDataStore dataStore, ISyncSessionInfo syncSessionInfo)
        {
            if (Conflicts == null || Conflicts.Count == 0)
                return;

            foreach (var conflict in Conflicts)
            {
                conflict.ApplyRemotePreResolution(dataStore, syncSessionInfo);
            }

            foreach (var conflict in Conflicts)
            {
                conflict.ApplyRemoteResolution(dataStore, syncSessionInfo);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SolvableEntityConflict);
        }

        protected bool Equals(SolvableEntityConflict other)
        {
            if (other == null)
                return false;

            return Equals(Conflicts, other.Conflicts)
                && string.Equals(EntityName, other.EntityName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Conflicts != null ? Conflicts.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (EntityName != null ? EntityName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}