using System;
using System.Reflection;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class FieldPropertyFactory : IFieldPropertyFactory
    {
        /// <summary>
        /// Convert specified .Net type to field definition
        /// </summary>
        /// <param name="type">.Net type to converter</param>
        /// <param name="fieldAttribute">FieldAttribute where datatype specific value is set</param>
        /// <returns>New field definition</returns>
        public FieldProperties Create(Type type, FieldAttribute fieldAttribute)
        {
            var trueType = GetTrueType(type);
            FieldProperties result;
            if (trueType == typeof(String))
                result = new StringField(fieldAttribute.Length);
            else if (trueType == typeof(Boolean))
                result = new BooleanField();
            else if (trueType == typeof(Int16) || trueType == typeof(UInt16))
                result = new Int16OrUInt16Field();
            else if (trueType == typeof(Int32) || trueType == typeof(UInt32))
                result = new Int32OrUInt32Field();
            else if (trueType == typeof(DateTime))
                result = new DateTimeField();
            else if (trueType == typeof(TimeSpan))
                result = new TimeField();
            else if (trueType == typeof(Single))
                result = new SingleField();
            else if (trueType == typeof(Decimal))
                result = new DecimalField(fieldAttribute.Precision, fieldAttribute.Scale);
            else if (trueType == typeof(Double))
                result = new DoubleField();
            else if (trueType == typeof(Int64) || trueType == typeof(UInt64))
            {
                if (fieldAttribute != null && fieldAttribute.IsRowVersion)
                    result = new RowVersionField();
                else
                    result = new Int64OrUInt64Field();
            }
            else if (trueType == typeof(Byte) || trueType == typeof(Char))
                result = new ByteField();
            else if (trueType == typeof(Guid))
                result = new GuildField();
            else if (trueType == typeof(Byte[]))
                result = new BinaryField(fieldAttribute.Length);
            else result = new ObjectField(this, fieldAttribute);

            result.PropertyType = type;
            if (fieldAttribute == null)
                return result;

            result.AllowsNulls = fieldAttribute.AllowsNulls;
            result.DefaultValue = fieldAttribute.DefaultValue;
            return result;
        }


        private static Type GetTrueType(Type type)
        {
            var result = type.IsNullable()
                ? type.GetGenericArguments()[0]
                : type;

            return result.IsEnum ? GetEnumUnderlyingType(result) : result;
        }

        public static Type GetEnumUnderlyingType(Type type)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fields == null || fields.Length != 1)
            {
                throw new ArgumentException("Unable to extract underlying type of enum.", "type");
            }
            return fields[0].FieldType;
        }
    }
}