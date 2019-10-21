namespace OpenNet.Orm.SqlCe.Fields
{
    public class Int16OrUInt16Field : IntegerField
    {
        public override string GetDataTypeDefinition()
        {
            return "smallint";
        }
    }
}