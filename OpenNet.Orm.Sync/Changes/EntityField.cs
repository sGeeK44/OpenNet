// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable ArrangeAccessorOwnerBody

using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Sync.Conflicts;

namespace OpenNet.Orm.Sync.Changes
{
    public class EntityField
    {
        public EntityField() { }

        public EntityField(string key, object value, Field field)
        {
            Key = key;
            FieldValue = FieldValue.Create(value);
            IsPrimaryKey = field.IsPrimaryKey;
            IsLastSyncColumn = field.IsLastSyncTracking;
            if (field.IsForeignKey)
            {
                ForeignKeyEntityName = ((ForeignKey)field).ForeignEntityInfo.GetNameInStore();
            }
        }

        public string Key { get; set; }
        public string Name { get { return string.Concat("[", Key, "]"); } }
        public FieldValue FieldValue { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsLastSyncColumn { get; set; }
        public string ForeignKeyEntityName { get; set; }
        
        public bool IsForeignKeyOf(string entityName)
        {
            return string.Equals(ForeignKeyEntityName, entityName);
        }

        public object GetFieldValue()
        {
            return FieldValue.Value;
        }

        public bool HasChangedInSession(ISyncSessionInfo syncSession)
        {
            return FieldValue.IsIncludeInSyncSession(syncSession);
        }

        public bool IsValueEquals(object deletedPrimaryId)
        {
            return FieldValue.Value != null && FieldValue.Value.Equals(deletedPrimaryId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityField);
        }

        protected bool Equals(EntityField other)
        {
            if (other == null)
                return false;

            return string.Equals(Key, other.Key) && Equals(FieldValue, other.FieldValue) && IsPrimaryKey == other.IsPrimaryKey;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FieldValue != null ? FieldValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsPrimaryKey.GetHashCode();
                return hashCode;
            }
        }

        internal void ApplyForeignKeyChange(IdentityChange identityChange)
        {
            if (!string.Equals(ForeignKeyEntityName, identityChange.EntityName))
                return;

            if (!FieldValue.Value.Equals(identityChange.OldValue))
                return;

            FieldValue = FieldValue.Create(identityChange.NewValue);
        }
    }
}