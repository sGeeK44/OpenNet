using System;
using System.Reflection;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
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
            if (trueType == typeof(string))
                result = new StringField();
            else if( trueType == typeof(Guid))
                result = new GuildField();
            else if (trueType == typeof(bool)
                || trueType == typeof(short)
                || trueType == typeof(ushort)
                || trueType == typeof(int)
                || trueType == typeof(uint)
                || trueType == typeof(byte)
                || trueType == typeof(char))
                result = new IntegerField();
            else if (trueType == typeof(DateTime))
                result = new DateTimeField();
            else if (trueType == typeof(TimeSpan))
                result = new TimeField();
            else if (trueType == typeof(float)
                || trueType == typeof(double))
                result = new RealField();
            else if (trueType == typeof(decimal))
                result = new NumericField(fieldAttribute.Precision, fieldAttribute.Scale);
            else if (trueType == typeof(long)
                || trueType == typeof(ulong))
            {
                if (fieldAttribute != null && fieldAttribute.IsRowVersion)
                    result = new RowVersionField();
                else
                    result = new IntegerField();
            }
            else if (trueType == typeof(byte[]))
                result = new BinaryField();
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