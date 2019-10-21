using System;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class RowVersionField : FieldProperties
    {
        public override object Convert(object value)
        {
            // sql stores this an 8-byte array
            return BitConverter.ToInt64((byte[]) value, 0);
        }

        public override string GetDataTypeDefinition()
        {
            return "BLOB";
        }

        /// <summary>
        /// Indicate if property can be setted or managed by sgbd
        /// </summary>
        public override bool IsSettable
        {
            get { return false; }
        }

        public override object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            // read-only, so do nothing*
            needToUpdateInstance = false;
            return null;
        }
    }
}