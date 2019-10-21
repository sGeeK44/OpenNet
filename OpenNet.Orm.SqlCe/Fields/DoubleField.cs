using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class DoubleField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "float";
        }
    }
}