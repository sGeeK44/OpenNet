using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.SqlCe.Fields
{
    public class BinaryField : FieldProperties
    {
        private const int MaxSizedBinaryLength = 8000;
        private readonly int _length;

        public BinaryField(int length)
        {
            _length = length;
        }

        public override string GetDataTypeDefinition()
        {
            // default to varbinary unless a Length is specifically supplied and it is >= 8000
            if (_length >= MaxSizedBinaryLength)
            {
                return "image";
            }
            // if no length was supplied, default to DefaultVarBinaryLength (8000)
            return string.Format("varbinary({0})", _length == 0 ? MaxSizedBinaryLength : _length);
        }
    }
}