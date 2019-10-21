using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable MergeConditionalExpression
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable ArrangeAccessorOwnerBody

namespace OpenNet.Orm.Sync.Changes
{
    public class EntityChange : IDataRecord
    {
        private readonly IEntityInfo _entityInfo;

        public string EntityName { get; set; }
        public bool HasAutoIncrement { get; set; }
        public List<EntityField> Fields { get; set; }

        public string Headers { get { return Fields.Select(_ => "| " + _.Key).Aggregate((s, s1) => s + s1); } }

        public EntityChange()
        {
            Fields = new List<EntityField>();
        }

        public EntityChange(IEntityInfo entityInfo)
            : this()
        {
            _entityInfo = entityInfo;
            EntityName = entityInfo.GetNameInStore();
            HasAutoIncrement = entityInfo.PrimaryKey.KeyScheme == KeyScheme.Identity;
        }

        public void AddFieldValue(IDataReader results, int index)
        {
            var columnName = results.GetName(index);
            var field = _entityInfo.Fields.FirstOrDefault(_ => _.FieldName == columnName);

            // Ignore unmanage entity field
            if (field == null)
                return;

            var value = results.GetValue(index);
            var entityField = new EntityField(columnName, value, field);
            Fields.Add(entityField);
        }

        public void Add(string key, object value)
        {
            var entityField = new EntityField{ Key = key, FieldValue = FieldValue.Create(value)};
            Fields.Add(entityField);
        }

        public EntityField GetField(string fieldKey)
        {
            return Fields.FirstOrDefault(_ => _.Key == fieldKey);
        }

        public bool HasChangedInSession(string trackColName, ISyncSessionInfo syncSession)
        {
            var trackField = GetField(trackColName);
            if (trackField == null)
                return false;

            return trackField.HasChangedInSession(syncSession);
        }

        public override string ToString()
        {
            var result = new StringBuilder("| ");
            foreach (var field in Fields)
            {
                result.AppendFormat("{0} |", field.GetFieldValue());
            }
            return result.ToString();
        }

        public bool IsSameEntity(EntityChange entityChange)
        {
            if (entityChange == null)
                return false;

            var primary = GetPrimaryField();
            var otherPrimary = entityChange.GetPrimaryField();
            return primary.GetFieldValue().Equals(otherPrimary.GetFieldValue());
        }

        private EntityField GetPrimaryField()
        {
            return Fields.FirstOrDefault(_ => _.IsPrimaryKey);
        }

        object IDataRecord.this[string name]
        {
            get
            {
                var field = Fields.FirstOrDefault(_ => _.Key == name);
                return field == null ? null : field.GetFieldValue();
            }
        }

        #region IDataRecord unused Member

        public string GetName(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotSupportedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotSupportedException();
        }

        public object GetValue(int i)
        {
            throw new NotSupportedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotSupportedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        public byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i)
        {
            throw new NotSupportedException();
        }

        public int GetInt32(int i)
        {
            throw new NotSupportedException();
        }

        public long GetInt64(int i)
        {
            throw new NotSupportedException();
        }

        public float GetFloat(int i)
        {
            throw new NotSupportedException();
        }

        public double GetDouble(int i)
        {
            throw new NotSupportedException();
        }

        public string GetString(int i)
        {
            throw new NotSupportedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotSupportedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotSupportedException();
        }

        public int FieldCount { get { return Fields.Count; } }

        object IDataRecord.this[int i]
        {
            get { throw new NotSupportedException(); }
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityChange);
        }

        protected bool Equals(EntityChange other)
        {
            if (other == null)
                return false;

            return string.Equals(EntityName, other.EntityName)
                && Fields.IsEquals(other.Fields);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_entityInfo != null ? _entityInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EntityName != null ? EntityName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Fields != null ? Fields.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void DisableAutoIncrement(ISqlDataStore dataStore)
        {
            if (!HasAutoIncrement)
                return;

            dataStore.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}] ON;", EntityName));
        }

        public void RestoreAutoIncrement(ISqlDataStore datastore)
        {
            if (!HasAutoIncrement)
                return;

            datastore.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}] OFF;", EntityName));
            var primaryKey = Fields.FirstOrDefault(_ => _.IsPrimaryKey);
            if (primaryKey == null)
                return;

            var primaryKeyField = primaryKey.Name;
            var maxIdDbValue = datastore.ExecuteScalar(string.Format("SELECT MAX({0}) FROM [{1}]", primaryKeyField, EntityName));
            var maxId = maxIdDbValue == DBNull.Value ? 1 : (long)maxIdDbValue;
            datastore.ExecuteNonQuery(string.Format("ALTER TABLE [{0}] ALTER COLUMN {1} IDENTITY ({2}, 1);", EntityName, primaryKeyField, maxId + 1));
        }

        public void ApplyInsert(ISqlDataStore datastore, EntityChangesetBuilder entitySerialiazer)
        {
            try
            {
                DisableAutoIncrement(datastore);
                using (var command = entitySerialiazer.ToInsert(datastore, EntityName, this))
                {
                    datastore.ExecuteNonQuery(command);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erreur lors de l'insertion {0}. Exception: {1}", this, e));
            }
            finally
            {
                RestoreAutoIncrement(datastore);
            }
        }

        public void ApplyUpdate(ISqlDataStore datastore, EntityChangesetBuilder entitySerialiazer)
        {
            try
            {
                using (var command = entitySerialiazer.ToUpdate(datastore, EntityName, this))
                {
                    datastore.ExecuteNonQuery(command);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erreur lors de l'update {0}. Exception: {1}", this, e));
            }
        }

        public void ApplyDelete(ISqlDataStore datastore, string entityName, EntityChangesetBuilder entitySerialiazer)
        {
            try
            {
                using (var command = entitySerialiazer.ToDelete(datastore, entityName, this))
                {
                    datastore.ExecuteNonQuery(command);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erreur lors de la suppression {0}. Exception: {1}", this, e));
            }
        }

        public static EntityChange Create(IEntityInfo entityInfo, IEntity entity)
        {
            var result = new EntityChange(entityInfo);
            foreach (var field in entityInfo.Fields)
            {
                var entityField = new EntityField(field.FieldName, field.GetEntityValue(entity), field);
                result.Fields.Add(entityField);
            }
            return result;
        }

        /// <summary>
        /// Get the foreign key field of the given entity if it exists
        /// </summary>
        /// <param name="entityName">Foreign key Entity name</param>
        /// <returns>Foreign key entity field if exists else null</returns>
        public EntityField GetForeignKeyOf(string entityName)
        {
            return Fields.FirstOrDefault(entityField => entityField.IsForeignKeyOf(entityName));
        }

        public string GetEntityNameFromTombstone()
        {
            return EntityName.Replace(EntityTombstoneAttribute.DefaultTableSuffixe, string.Empty);
        }

        public object GetPrimaryKeyValue()
        {
            return Fields.First(_ => _.IsPrimaryKey).FieldValue.Value;
        }
    }
}
