using System;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class NumericField : FieldProperties
    {
        private const int DefaultNumericFieldPrecision = 16;

        public NumericField(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        /// Precision for floating number
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Scale for floating number
        /// </summary>
        public int Scale { get; set; }

        public override string GetDataTypeDefinition()
        {
            return "NUMERIC";
        }

        public override object ToSqlValue(object instanceValue, out bool needToUpdateInstance)
        {
            needToUpdateInstance = false;
            return Math.Round((decimal)instanceValue, Scale);
        }
    }
}