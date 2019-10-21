using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.Testkit.Entities
{
    public class BookVersionRepository : Repository<BookVersion, BookVersion>
    {
        public BookVersionRepository(IDataStore dataStore)
            : base(dataStore)
        {
        }

        public override List<BookVersion> GetAllReference<TForeignEntity>(long id)
        {
            if (typeof (TForeignEntity) == typeof (Book))
                return GetAllBookReference(id);

            return base.GetAllReference<TForeignEntity>(id);
        }

        public List<BookVersion> GetAllBookReference(long id)
        {
            var condition = DataStore.Condition<BookVersion>(BookVersion.BookIdColName, id,
                FilterOperator.Equals);

            return DataStore.Select<BookVersion>().Where(condition).GetValues().ToList();
        }
    }
}
