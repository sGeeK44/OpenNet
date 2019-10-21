namespace OpenNet.Orm.SqlCe.Fields
{
    public class Int32OrUInt32Field : IntegerField
    {
        public override string GetDataTypeDefinition()
        {
            return "integer";
        }
    }
}