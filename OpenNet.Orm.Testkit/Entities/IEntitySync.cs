using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    public interface IEntitySync : ISyncable
    {
        string UniqueIdentifier { get; set; }
        string StringField { get; set; }
    }
}