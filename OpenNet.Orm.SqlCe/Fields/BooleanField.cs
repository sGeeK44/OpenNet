using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class BooleanField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "bit";
        }
    }
}