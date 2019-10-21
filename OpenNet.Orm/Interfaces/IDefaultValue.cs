using OpenNet.Orm.Constants;

namespace OpenNet.Orm.Interfaces
{
    public interface IDefaultValue
    {
        DefaultType DefaultType { get; }
        object GetDefaultValue();
    }
}
