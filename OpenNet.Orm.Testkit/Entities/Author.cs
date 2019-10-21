using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity.Constraints;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity(Serializer = typeof(AuthorSerializer))]
    public class Author
    {
        public const string PrimaryKeyName = "Id";

        [PrimaryKey(KeyScheme.Identity, Indexes = new []{ "MonIndex" })]
        public int Id { get; set; }

        [Reference(typeof(Book), Book.AuthorIdColName, PrimaryKeyName)]
        public Book[] Books { get; set; }

        [Field(SearchOrder = FieldSearchOrder.Ascending, Indexes = new[] { "MonIndex" })]
        public string Name { get; set; }
    }
}
