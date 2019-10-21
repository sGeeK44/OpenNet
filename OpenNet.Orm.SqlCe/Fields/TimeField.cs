using System;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class TimeField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "time";
        }

        public override object Convert(object value)
        {
            // SQL Compact doesn't support Time, so we're convert to ticks in both directions
            return new TimeSpan((long) value);
        }

        public override object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            return base.ToSqlValue(((TimeSpan) instanceValue).Ticks, out needToUpdateInstance);
        }
    }
}