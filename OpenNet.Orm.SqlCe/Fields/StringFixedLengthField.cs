using System.Text;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class StringFixedLengthField : FieldProperties
    {
        protected const int DefaultStringFieldSize = 200;
        protected const int MaxSizedStringLength = 4000;

        public StringFixedLengthField(int lenght)
        {
            Length = lenght;
        }

        protected int Length { get; private set; }

        public override string GetDataTypeDefinition()
        {
            return "nchar";
        }

        public override void GetFieldCreationAttributes(StringBuilder definition)
        {
            if (Length > 0)
            {
                if (Length <= MaxSizedStringLength)
                {
                    definition.AppendFormat("({0}) ", Length);
                }
            }
            else
            {
                definition.AppendFormat("({0}) ", DefaultStringFieldSize);
            }
        }
    }
}