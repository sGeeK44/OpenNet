using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class SingleField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "real";
        }
    }
}