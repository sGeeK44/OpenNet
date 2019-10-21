using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class ByteField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "tinyint";
        }
    }
}