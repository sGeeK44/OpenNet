using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.Testkit.Entities
{
    public class BookRepository : Repository<Book, Book>
    {
        public BookRepository(IDataStore dataStore) : base(dataStore) { }
    }
}
