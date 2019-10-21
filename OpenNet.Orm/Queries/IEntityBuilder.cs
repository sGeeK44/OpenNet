using System.Data;
using OpenNet.Orm.Caches;

// ReSharper disable TypeParameterCanBeVariant

namespace OpenNet.Orm.Queries
{
    public interface IEntityBuilder<TIEntity>
    {
        int Offset { get; set; }
        IEntityCache EntityCache { get; set; }
        TIEntity Deserialize(IDataReader results);
    }

    public interface IEntityBuilder
    {
        int Offset { get; set; }
        IEntityCache EntityCache { get; set; }
        object Deserialize(IDataReader results);
    }
}