using System.Collections.Generic;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Constants;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Entity.References;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.Testkit.Entities
{
    public enum BookType
    {
        Fiction,
        NonFiction
    }

    [Entity]
    public class Book : EntityBase<Book>
    {
        public const string AuthorIdColName = "AuthorId";
        private readonly ReferenceCollectionHolder<Book, BookVersion> _versionList;

        public Book() : this(null) { }

        public Book(IRepository<BookVersion> repo)
        {
            _versionList = new ReferenceCollectionHolder<Book, BookVersion>(repo, this);
        }

        [Field]
        public int AuthorId { get; set; }

        [Reference(typeof(Author), Author.PrimaryKeyName, AuthorIdColName)]
        public Author Author { get; set; }

        [Field]
        public string Title { get; set; }
        

        [Field(SearchOrder=FieldSearchOrder.Ascending)]
        public BookType BookType { get; set; }

        [Field(IsRowVersion=true)]
        public long RowVersion { get; set; }

        [Reference(typeof(BookVersion), BookVersion.BookIdColName, IdColumnName)]
        public List<BookVersion> BookVersions
        {
            get { return _versionList.ObjectCollection; }
            set { _versionList.ObjectCollection = value; }
        }
    }
}
