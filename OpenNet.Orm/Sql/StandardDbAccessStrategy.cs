using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Sql
{
    public class StandardDbAccessStrategy : IDbAccessStrategy
    {
        private readonly ISqlDataStore _datastore;

        public StandardDbAccessStrategy(ISqlDataStore dataStore)
        {
            _datastore = dataStore;
        }

        public object SelectByPrimayKey(Type objectType, PrimaryKey primaryKey, object value)
        {
            var condition = _datastore.Condition(objectType, primaryKey.FieldName, value, FilterOperator.Equals);
            return _datastore.Select(objectType).Where(condition).GetValues().FirstOrDefault();
        }

        public void Insert(object item)
        {
            var itemType = item.GetType();
            var entityName = _datastore.Entities.GetNameForType(itemType);

            if (entityName == null)
            {
                throw new EntityNotFoundException(item.GetType());
            }

            using (var command = ToInsertCommand(_datastore.Entities[entityName], item))
            {
                OrmDebug.Info(command.CommandText);
                command.ExecuteNonQuery();

                // did we have an identity field?  If so, we need to update that value in the item
                var primaryKey = _datastore.Entities[entityName].PrimaryKey;
                if (primaryKey.KeyScheme == KeyScheme.Identity)
                {
                    var id = GetIdentity(primaryKey);
                    primaryKey.SetEntityValue(item, id);
                }

                if (_datastore.Cache != null)
                    _datastore.Cache.Cache(item, primaryKey.GetEntityValue(item));
            }
        }

        public void Update(object item)
        {
            var itemType = item.GetType();

            var entityName = _datastore.Entities.GetNameForType(itemType);

            if (entityName == null)
            {
                throw new EntityNotFoundException(itemType);
            }

            if (_datastore.Entities[entityName].PrimaryKey == null)
            {
                throw new PrimaryKeyRequiredException("A primary key is required on an Entity in order to perform Updates");
            }

            using (var command = ToUpdateCommand(_datastore.Entities[entityName], item))
            {
                OrmDebug.Info(command.CommandText);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(object item)
        {
            var itemType = item.GetType();

            var entityName = _datastore.Entities.GetNameForType(itemType);

            if (entityName == null)
            {
                throw new EntityNotFoundException(itemType);
            }

            if (_datastore.Entities[entityName].PrimaryKey == null)
            {
                throw new PrimaryKeyRequiredException("A primary key is required on an Entity in order to perform delete");
            }

            using (var command = ToDeleteCommand(_datastore.Entities[entityName], item))
            {
                OrmDebug.Info(command.CommandText);
                command.ExecuteNonQuery();
            }
        }

        public string[] GetTableNames()
        {
            var connection = _datastore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";
                OrmDebug.Info(command.CommandText);

                using (var reader = command.ExecuteReader())
                {
                    var names = new List<string>(); 
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        if (name == "sqlite_sequence")
                            continue;

                        names.Add(name);
                    }
                    return names.ToArray();
                }
            }
        }

        public void ValidateTable(IEntityInfo entity)
        {
            var entityName = entity.GetNameInStore();
            var connection = _datastore.GetConnection();

            // first make sure the table exists
            if (_datastore.TableExists(entityName))
                return;

            _datastore.CreateTable(connection, entity);
        }

        private IDbCommand ToInsertCommand(IEntityInfo entity, object item)
        {
            const string sqlCommandText = "INSERT INTO [{0}] ({1}) VALUES ({2})";
            StringBuilder key = null;
            StringBuilder value = null;
            var @params = new List<IDataParameter>();

            var sqlFactory = _datastore.SqlFactory;
            foreach (var field in entity.Fields)
            {
                if (field.IsPrimaryKey && ((PrimaryKey)field).KeyScheme == KeyScheme.Identity)
                    continue;

                var fieldValue = field.ToSqlValue(item);
                var paramName = sqlFactory.AddParam(fieldValue, @params);
                key = AddValue(key, field.FieldName);
                value = AddValue(value, paramName);
            }

            var connection = _datastore.GetConnection();
            var insert = connection.CreateCommand();
            insert.CommandText = string.Format(sqlCommandText, entity.GetNameInStore(), key, value);
            SetCommandParam(@params, insert);
            return insert;
        }

        public IDbCommand ToUpdateCommand(IEntityInfo entity, object item)
        {
            const string sqlCommandText = "UPDATE [{0}] SET {1} WHERE {2}";
            StringBuilder value = null;
            StringBuilder where = null;
            var @params = new List<IDataParameter>();

            var sqlFactory = _datastore.SqlFactory;
            foreach (var field in entity.Fields)
            {
                var fieldValue = field.ToSqlValue(item);
                var paramName = sqlFactory.AddParam(fieldValue, @params);
                if (field.IsPrimaryKey)
                {
                    where = AddWhere(where, string.Format("{0} = {1}", field.FieldName, paramName));
                    continue;
                }

                value = AddValue(value, string.Format("{0} = {1}", field.FieldName, paramName));
            }

            var connection = _datastore.GetConnection();
            var update = connection.CreateCommand();
            update.CommandText = string.Format(sqlCommandText, entity.GetNameInStore(), value, where);
            SetCommandParam(@params, update);
            return update;
        }

        public IDbCommand ToDeleteCommand(IEntityInfo entity, object item)
        {
            const string sqlCommandText = "DELETE FROM [{0}] WHERE {1}";
            StringBuilder where = null;
            var @params = new List<IDataParameter>();

            var sqlFactory = _datastore.SqlFactory;
            foreach (var field in entity.Fields)
            {
                var fieldValue = field.ToSqlValue(item);
                var paramName = sqlFactory.AddParam(fieldValue, @params);
                if (!field.IsPrimaryKey)
                    continue;

                @where = AddWhere(@where, string.Format("{0} = {1}", field.FieldName, paramName));
            }

            var connection = _datastore.GetConnection();
            var delete = connection.CreateCommand();
            delete.CommandText = string.Format(sqlCommandText, entity.GetNameInStore(), where);
            SetCommandParam(@params, delete);
            return delete;
        }

        private int GetIdentity(PrimaryKey primaryKey)
        {
            var connection = _datastore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT last_insert_rowid() FROM {0}", primaryKey.Entity.GetNameInStore());
                var id = command.ExecuteScalar();
                return Convert.ToInt32(id);
            }
        }

        private static void SetCommandParam(IEnumerable<IDataParameter> @params, IDbCommand insert)
        {
            foreach (var param in @params)
            {
                insert.Parameters.Add(param);
            }
        }

        private static StringBuilder AddWhere(StringBuilder list, string paramName)
        {
            return Join(list, paramName, "AND");
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