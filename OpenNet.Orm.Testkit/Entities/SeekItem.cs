using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Constraints;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity]
    public class SeekItem
    {
        [PrimaryKey(KeyScheme.Identity)]
        public int ID { get; set; }

        [Field(SearchOrder=FieldSearchOrder.Ascending)]
        public int SeekField { get; set; }

        [Field]
        public string Data { get; set; }

    }
}
