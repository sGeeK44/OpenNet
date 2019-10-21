using System;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class GuildField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "TEXT";
        }

        public override object Convert(object value)
        {
            return new Guid((string)value);
        }

        public override object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            needToUpdateInstance = false;
            if (instanceValue == null)
                return DBNull.Value;

            return instanceValue.ToString();
        }
    }
}