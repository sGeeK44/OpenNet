using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Sqlite.Fields
{
    public class RealField : FieldProperties
    {
        public override string GetDataTypeDefinition()
        {
            return "REAL";
        }

        public override object Convert(object value)
        {
            var unboxValue = (double)value;
            if (PropertyType == typeof(float))
                return base.Convert((float)unboxValue);
            return base.Convert(unboxValue);
        }
    }
}