namespace OpenNet.Orm.SqlCe.Fields
{
    public class Int64OrUInt64Field : IntegerField
    {
        public override string GetDataTypeDefinition()
        {
            return "bigint";
        }
    }
}