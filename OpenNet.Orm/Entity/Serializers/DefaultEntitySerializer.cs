using System;
using System.Data;
using System.Reflection;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Interfaces;

// ReSharper disable UseNameofExpression
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Entity.Serializers
{
    /// <summary>
    /// Provide method to convert object in according with Field Attribute
    /// </summary>
    public class DefaultEntitySerializer :  IEntitySerializer
    {
        public IEntityInfo Entity { get; set; }
        public IEntityCache EntityCache { get; set; }

        public DefaultEntitySerializer(IEntityInfo entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entity = entity;
        }

        /// <summary>
        /// Indicate if current serializer should use full field name data column access. (Depending on select used)
        /// </summary>
        public bool UseFullName { get; set; }

        /// <summary>
        /// Convert specified database result into specified intance type
        /// </summary>
        /// <param name="dbResult">DataReader get from select</param>
        /// <returns>Instance initialized</returns>
        public object Deserialize(IDataRecord dbResult)
        {
            if (dbResult == null)
                return null;

            var primaryKeyField = Entity.PrimaryKey;
            var primaryKeyName = GetReaderFieldName(primaryKeyField);
            var primaryKeyValue = primaryKeyField.Convert(dbResult[primaryKeyName]);

            if (primaryKeyValue == DBNull.Value)
                return null;

            var result = GetFromCache(primaryKeyValue);
            if (result != null)
                return result;

            result = Entity.CreateNewInstance();
            UpdateCache(result, primaryKeyValue);

            PopulateFields(result, dbResult);
            return result;
        }

        private void UpdateCache(object result, object primaryKeyValue)
        {
            if (EntityCache != null)
                EntityCache.Cache(result, primaryKeyValue);
        }

        private object GetFromCache(object primaryKeyValue)
        {
            return EntityCache != null
                 ? EntityCache.GetOrDefault(Entity.EntityType, primaryKeyValue)
                 : null;
        }

        private string GetReaderFieldName(Field field)
        {
            return UseFullName ? field.AliasFieldName : field.FieldName;
        }

        public void PopulateFields(object item, IDataRecord dbResult)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            foreach (var field in Entity.Fields)
            {
                var fieldName = GetReaderFieldName(field);
                var value = dbResult[fieldName];
                
                if (value == DBNull.Value)
                    continue;

                value = field.Convert(value);
                try
                {
                    field.SetEntityValue(item, value);
                }
                catch (Exception ex)
                {
                    var reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    throw new TargetInvocationException(string.Format("An exception occurs when entity's field was setted. Entity:{0}. Field:{1}. Value setted:{2}. Reason:{3}.", field.Entity.GetNameInStore(), field.FieldName, value, reason), ex);
                }
            }
        }
    }
}