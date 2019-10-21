using System.Reflection;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Entity.Fields
{
    public class Field : IDistinctable
    {
        public Field(IEntityInfo entity, PropertyInfo prop, FieldAttribute fieldAttribute)
        {
            Entity = entity;
            PropertyInfo = prop;
            if (fieldAttribute != null)
            {
                RequireUniqueValue = fieldAttribute.RequireUniqueValue;
                SearchOrder = fieldAttribute.SearchOrder;
                FieldName = fieldAttribute.FieldName ?? prop.Name;
                FullFieldName = string.Format("[{0}].[{1}]", Entity.GetNameInStore(), FieldName);
                AliasFieldName = string.Format("{0}{1}", Entity.GetNameInStore(), FieldName);

                IsCreationTracking = fieldAttribute.IsCreationTracking;
                IsUpdateTracking = fieldAttribute.IsUpdateTracking;
                IsDeletionTracking = fieldAttribute.IsDeletionTracking;
                IsLastSyncTracking = fieldAttribute.IsLastSyncTracking;
            }

            if (prop != null)
            {
                FieldProperties = FieldPropertyFactory.Create(prop.PropertyType, fieldAttribute);
            }
        }

        private IFieldPropertyFactory FieldPropertyFactory { get { return Entity.FieldPropertyFactory; } }

        protected FieldProperties FieldProperties { get; private set; }

        public IEntityInfo Entity { get; private set; }

        public virtual string Key
        {
            get { return FieldName; }
        }

        /// <summary>
        /// Field name in datastore
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Get fully qualified name (TableName.FieldName)
        /// </summary>
        public string FullFieldName { get; private set; }

        /// <summary>
        /// Get fully qualified name valid for column name select (TableNameFieldName)
        /// </summary>
        public string AliasFieldName { get; private set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect inserted row (False by default)
        /// </summary>
        public bool IsCreationTracking { get; private set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect updated row (False by default)
        /// </summary>
        public bool IsUpdateTracking { get; private set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect row in tombstone table (False by default)
        /// </summary>
        public bool IsDeletionTracking { get; private set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect updated row in lastsync (False by default)
        /// </summary>
        public bool IsLastSyncTracking { get; private set; }

        protected PropertyInfo PropertyInfo { get; private set; }

        public string GetFieldDefinition()
        {
            return FieldProperties.GetDefinition();
        }

        protected virtual string GetFieldCreationAttributes()
        {
            return FieldProperties.GetFieldCreationAttributes();
        }

        public bool RequireUniqueValue { get; set; }

        public FieldSearchOrder SearchOrder { get; set; }

        public bool IsPrimaryKey
        {
            get { return this == Entity.PrimaryKey; }
        }

        public virtual bool IsForeignKey
        {
            get { return false; }
        }

        /// <summary>
        /// Indicate if property can be setted or managed by sgbd
        /// </summary>
        public bool Settable
        {
            get
            {
                return !(IsPrimaryKey && Entity.PrimaryKey.KeyScheme == KeyScheme.Identity) && FieldProperties.IsSettable;
            }
        }

        public string GetFieldDefinitionSqlQuery()
        {
            return string.Format("[{0}] {1}{2}",
                FieldName,
                GetFieldDefinition(),
                GetFieldCreationAttributes());
        }

        public void SetEntityValue(object item, object value)
        {
            PropertyInfo.SetValue(item, value, null);
        }

        public object GetEntityValue(object item)
        {
            return PropertyInfo.GetValue(item, null);
        }

        public object Convert(object value)
        {
            return FieldProperties.Convert(value);
        }

        public static Field Create(IEntityInfo entity, PropertyInfo prop, FieldAttribute fieldAttribute)
        {
            return new Field(entity, prop, fieldAttribute);
        }

        public virtual object ToSqlValue(object item)
        {
            var instanceValue = GetEntityValue(item);
            bool needToUpdateInstance;
            var result = FieldProperties.ToSqlValue(instanceValue, out needToUpdateInstance);
            if (needToUpdateInstance)
            {
                SetEntityValue(item, result);
            }
            return result;
        }
    }
}