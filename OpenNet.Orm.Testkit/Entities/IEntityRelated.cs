using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    public interface IEntityRelated: ISyncable
    {
        long? RelatedId { get; set; }
    }
}