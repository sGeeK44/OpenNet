using System;
using System.Data.SqlTypes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class DateTimeField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            //SQLite does not have a storage class set aside for storing dates and/or times.
            //Instead, the built-in Date And Time Functions of SQLite are capable of storing dates and times as TEXT, REAL, or INTEGER values
            //TEXT as ISO8601 strings("YYYY-MM-DD HH:MM:SS.SSS").
            return "TEXT";
        }

        public override object Convert(object value)
        {
            return DateTime.Parse((string)value);
        }

        public override object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            needToUpdateInstance = false;
            if (instanceValue == null || Equals(DateTime.MinValue, instanceValue))
            {
                if (AllowsNulls || DefaultType.CurrentDateTime.Equals(DefaultValue))
                {
                    return null;
                }
                return SqlDateTime.MinValue;
            }
            if (instanceValue == DBNull.Value)
                return instanceValue;

            needToUpdateInstance = true;
            instanceValue = RoundToSqlDateTime((DateTime) instanceValue);

            return instanceValue;
        }

        private static DateTime RoundToSqlDateTime(DateTime iv)
        {
            return new SqlDateTime(iv).Value;
        }
    }
}