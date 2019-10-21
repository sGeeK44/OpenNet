using System;
using System.Data.SqlTypes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class DateTimeField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "datetime";
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