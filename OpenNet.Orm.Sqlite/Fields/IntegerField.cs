using System;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class IntegerField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "INTEGER";
        }

        public override string GetIdentity()
        {
            return " PRIMARY KEY AUTOINCREMENT";
        }

        public override object Convert(object value)
        {
            if (value == DBNull.Value)
                return base.Convert(value);

            var unboxValue = (long) value;
            if (PropertyType == typeof(bool))
                return base.Convert(unboxValue == 1);
            if (PropertyType == typeof(int))
                return base.Convert((int) unboxValue);
            if (PropertyType == typeof(uint))
                return base.Convert((uint)unboxValue);
            if (PropertyType == typeof(short))
                return base.Convert((short)unboxValue);
            if (PropertyType == typeof(ushort))
                return base.Convert((ushort)unboxValue);
            if (PropertyType == typeof(byte))
                return base.Convert((byte)unboxValue);
            if (PropertyType == typeof(char))
                return base.Convert((char)unboxValue);
            return base.Convert(unboxValue);
        }
    }
}