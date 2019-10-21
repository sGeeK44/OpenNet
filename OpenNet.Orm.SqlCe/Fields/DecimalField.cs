using System.Text;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class DecimalField : FieldProperties
    {
        private const int DefaultNumericFieldPrecision = 16;

        public DecimalField(int precision, int scale)
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
            return "numeric";
        }

        public override void GetFieldCreationAttributes(StringBuilder definition)
        {
            var p = Precision == 0 ? DefaultNumericFieldPrecision : Precision;
            definition.AppendFormat("({0},{1}) ", p, Scale);
        }
    }
}