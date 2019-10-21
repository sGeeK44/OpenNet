namespace OpenNet.Orm.SqlCe.Fields
{
    public class StringField : StringFixedLengthField
    {
        public StringField(int length)
            : base(length) { }

        public override string GetDataTypeDefinition()
        {
            return Length > MaxSizedStringLength ? "ntext" : "nvarchar";
        }
    }
}