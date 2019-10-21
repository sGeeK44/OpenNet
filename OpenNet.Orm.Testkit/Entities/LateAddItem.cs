using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity.Constraints;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity]
    public class LateAddItem
    {
        [PrimaryKey(KeyScheme.Identity)]
        public int Id { get; set; }

        [Field]
        public string Name { get; set; }
    }
}