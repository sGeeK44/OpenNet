using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Entity.References;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.Testkit.Entities
{
    [Entity]
    public class BookVersion : EntityBase<BookVersion>
    {
        public const string BookIdColName = "BookId";
        private readonly NullableReferenceHolder<Book> _book;

        public BookVersion() : this(null) { }

        public BookVersion(IRepository<Book> repo)
        {
            _book = new NullableReferenceHolder<Book>(repo);
        }

        [ForeignKey(typeof(Book))]
        public long? BookId
        {
            get { return _book.Id; }
            set { _book.Id = value; }
        }

        [Reference(typeof(Book), IdColumnName, BookIdColName)]
        public Book Book
        {
            get { return _book.Object; }
            set { _book.Object = value; }
        }
    }
}