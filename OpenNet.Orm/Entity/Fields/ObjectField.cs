using System;
using System.Text;
using OpenNet.Orm.Attributes;

namespace OpenNet.Orm.Entity.Fields
{
    public class ObjectField : FieldProperties
    {
        private readonly FieldProperties _specificFieldDefition;

        public ObjectField(IFieldPropertyFactory fieldPropertyFactory, FieldAttribute fieldAttribute)
        {
            if (fieldAttribute != null && fieldAttribute.SpecificDataType != null)
            {
                _specificFieldDefition = fieldPropertyFactory.Create(fieldAttribute.SpecificDataType, fieldAttribute);
            }
        }

        public override string GetDataTypeDefinition()
        {
            return _specificFieldDefition != null ? _specificFieldDefition.GetDataTypeDefinition() : "image";
        }

        public override void GetFieldCreationAttributes(StringBuilder definition)
        {
            if (_specificFieldDefition != null)
                _specificFieldDefition.GetFieldCreationAttributes(definition);
            else
                base.GetFieldCreationAttributes(definition);
        }

        public override object Convert(object value)
        {
            if (!typeof(ICustomSqlField).IsAssignableFrom(PropertyType))
                throw new DefinitionException("Object field have to implement ICustomSqlField.");

            var result = (ICustomSqlField) Activator.CreateInstance(PropertyType);
            result.FromSqlValue(value);
            return result;
        }

        public override object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            if (instanceValue != null && instanceValue != DBNull.Value)
                instanceValue = ((ICustomSqlField) instanceValue).ToSqlValue();

            return base.ToSqlValue(instanceValue, out needToUpdateInstance);
        }
    }
}