using System.Collections.Generic;
using System.Data;
using System.Text;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sync.Entity;

// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sync.Changes
{
    public class EntityChangesetBuilder : IEntityBuilder<EntityChange>
    {
        private readonly IEntityInfo _entityInfo;
        private readonly ISyncSessionInfo _syncSessionInfo;

        public EntityChangesetBuilder(IEntityInfo entityInfo, ISyncSessionInfo syncSessionInfo)
        {
            _entityInfo = entityInfo;
            _syncSessionInfo = syncSessionInfo;
        }

        public int Offset { get; set; }

        public IEntityCache EntityCache { get; set; }

        public EntityChange Deserialize(IDataReader results)
        {
            var entityFields = new EntityChange(_entityInfo);
            for (var i = 0; i < results.FieldCount; i++)
            {
                entityFields.AddFieldValue(results, i);
            }

            return entityFields;
        }

        public IDbCommand FindExisting(ISqlDataStore dataStore, EntityChange entityChange)
        {
            const string sqlCommandText = "SELECT * FROM [{0}] WHERE {1}";
            var @params = new List<IDataParameter>();

            var sqlFactory = dataStore.SqlFactory;
            var where = BuildWhereIdentity(entityChange, _entityInfo, sqlFactory, @params);

            var select = sqlFactory.CreateCommand();
            select.CommandText = string.Format(sqlCommandText, _entityInfo.GetNameInStore(), where);
            SetCommandParam(@params, select);
            return select;
        }

        public IDbCommand ToInsert(IDataStore dataStore, string entityName, EntityChange entityChange)
        {
            const string sqlCommandText = "INSERT INTO [{0}] ({1}) VALUES({2})";
            StringBuilder key = null;
            StringBuilder value = null;
            var @params = new List<IDataParameter>();

            var sqlFactory = dataStore.SqlFactory;
            foreach (var field in entityChange.Fields)
            {
                var fieldValue = GetFieldValue(field);
                var paramName = sqlFactory.AddParam(fieldValue, @params);
                key = AddValue(key, field.Name);
                value = AddValue(value, paramName);
            }
            var insert = sqlFactory.CreateCommand();
            insert.CommandText = string.Format(sqlCommandText, entityName, key, value);
            SetCommandParam(@params, insert);
            return insert;
        }

        public IDbCommand ToUpdate(IDataStore dataStore, string entityName, EntityChange entityChange)
        {
            const string sqlCommandText = "UPDATE [{0}] SET {1} WHERE {2}";
            var @params = new List<IDataParameter>();

            var sqlFactory = dataStore.SqlFactory;
            var entity = dataStore.Entities[entityName];
            var values = BuildUpdateSetClause(entityChange, entity, sqlFactory, @params);
            var where = BuildWhereIdentity(entityChange, entity, sqlFactory, @params);

            var update = sqlFactory.CreateCommand();
            update.CommandText = string.Format(sqlCommandText, entityName, values, where);
            SetCommandParam(@params, update);
            return update;
        }

        public IDbCommand ToDelete(IDataStore dataStore, string entityName, EntityChange entityChange)
        {
            const string sqlCommandText = "DELETE FROM [{0}] WHERE {1}";
            var @params = new List<IDataParameter>();

            var sqlFactory = dataStore.SqlFactory;
            var entity = GetSyncableEntity(dataStore, entityName);
            var where = BuildWhereIdentity(entityChange, entity.EntityTombstoneInfo, sqlFactory, @params);
            var delete = sqlFactory.CreateCommand();
            delete.CommandText = string.Format(sqlCommandText, entityName, where);
            SetCommandParam(@params, delete);
            return delete;
        }

        private static ISyncableEntity GetSyncableEntity(IDataStore dataStore, string entityName)
        {
            var entity = dataStore.Entities[entityName];
            return SyncEntity.Create(entity);
        }

        private string GetValue(ISqlFactory sqlFactory, EntityField field, List<IDataParameter> @params)
        {
            var fieldValue = GetFieldValue(field);
            var paramName = sqlFactory.AddParam(fieldValue, @params);
            return string.Format("{0} = {1}", field.Name, paramName);
        }

        private object GetFieldValue(EntityField field)
        {
            var fieldValue = field.IsLastSyncColumn ? _syncSessionInfo.HighBoundaryAnchor : field.GetFieldValue();
            return fieldValue;
        }

        private StringBuilder BuildUpdateSetClause(EntityChange entityChange, IEntityInfo entity, ISqlFactory sqlFactory,
            List<IDataParameter> @params)
        {
            StringBuilder values = null;
            foreach (var field in entityChange.Fields)
            {
                var fieldAttribute = entity.Fields[field.Key];
                if (fieldAttribute.IsPrimaryKey)
                    continue;

                var value = GetValue(sqlFactory, field, @params);
                values = AddValue(values, value);
            }
            return values;
        }

        private StringBuilder BuildWhereIdentity(EntityChange entityChange, IEntityInfo entity, ISqlFactory sqlFactory,
            List<IDataParameter> @params)
        {
            StringBuilder where = null;
            foreach (var field in entityChange.Fields)
            {
                if (!entity.Fields.Contains(field.Key))
                    continue;

                var fieldAttribute = entity.Fields[field.Key];
                if (!fieldAttribute.IsPrimaryKey)
                    continue;

                var value = GetValue(sqlFactory, field, @params);
                where = AddWhere(where, value);
            }
            return where;
        }

        private static void SetCommandParam(IEnumerable<IDataParameter> @params, IDbCommand insert)
        {
            foreach (var param in @params)
            {
                insert.Parameters.Add(param);
            }
        }

        private static StringBuilder AddWhere(StringBuilder list, string valueToAdd)
        {
            return Join(list, valueToAdd, "AND");
        }

        private static StringBuilder AddValue(StringBuilder list, string valueToAdd)
        {
            return Join(list, valueToAdd, ",");
        }

        private static StringBuilder Join(StringBuilder list, string valueToAdd, string join)
        {
            if (list == null)
                return new StringBuilder(valueToAdd);

            list.AppendFormat("{0} {1}", join, valueToAdd);
            return list;
        }
    }
}