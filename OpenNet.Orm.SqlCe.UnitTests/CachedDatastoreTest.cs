using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Repositories;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.SqlCe.UnitTests
{
    [TestFixture]
    public class CachedDatastoreTest
    {
        public IRepository<BookVersion> NoCachedBookVersionRepository { get; set; }
        public IRepository<BookVersion> BookVersionRepository { get; set; }
        public IRepository<Book> BookRepository { get; set; }
        public ISqlDataStore DataStore { get; set; }
        public ISqlDataStore DatastoreNoCache { get; set; }

        [SetUp]
        public void Init()
        {
            var datasource = TestContext.CurrentContext.Test.MethodName + ".sdf";
            Debug.WriteLine(string.Format("Database is available here:{0}", Path.Combine(TestContext.CurrentContext.WorkDirectory, datasource)));
            DataStore = SqlCeFactory.CreateStore(datasource);

            if (DataStore.StoreExists)
            {
                DataStore.DeleteStore();
            }
            DataStore.AddType<Book>();
            DataStore.AddType<BookVersion>();
            DataStore.CreateOrUpdateStore();
            DataStore.Cache = new EntitiesCache();
            BookRepository = new BookRepository(DataStore);
            DatastoreNoCache = SqlCeFactory.CreateStore(datasource);
            DatastoreNoCache.AddType<Book>();
            DatastoreNoCache.AddType<BookVersion>();
            NoCachedBookVersionRepository = new BookVersionRepository(DatastoreNoCache);
            BookVersionRepository = new BookVersionRepository(DataStore);
        }
        
        [TearDown]
        public void CleanUp()
        {
            DataStore.Dispose();
            DatastoreNoCache.Dispose();
        }

        [Test]
        public void GetById_SavedObject_ShouldReturnCachedObject()
        {
            var book = new Book();
            BookRepository.Save(book);

            var result = BookRepository.GetById(book.Id);

            Assert.AreSame(book, result);
        }

        [Test]
        public void Delete_CachedObject_ShouldReturnNull()
        {
            var book = new Book();
            BookRepository.Save(book);

            BookRepository.Delete(book);
            var result = BookRepository.GetById(book.Id);

            Assert.IsNull(result);
        }

        [Test]
        public void GetAllReference_SavedObject_ShouldReturnCachedObject()
        {
            var book = new Book();
            BookRepository.Save(book);
            var version = new BookVersion { BookId = book.Id };
            BookVersionRepository.Save(version);

            var result = BookVersionRepository.GetAllReference<Book>(book.Id);

            Assert.AreSame(version, result.First());
        }

        [Test]
        public void GetAllReference_NotCachedObject_ShouldReturnInstanceFromDb()
        {
            var book = new Book();
            BookRepository.Save(book);
            var version = new BookVersion { BookId = book.Id };
            NoCachedBookVersionRepository.Save(version);

            var result = BookVersionRepository.GetAllReference<Book>(book.Id);

            Assert.AreNotSame(version, result.First());
            Assert.AreEqual(version, result.First());
        }

        [Test]
        public void GetAllReference_NotCachedObject_ShouldUpdateCache()
        {
            var book = new Book();
            BookRepository.Save(book);
            var version = new BookVersion { BookId = book.Id };
            NoCachedBookVersionRepository.Save(version);

            version = BookVersionRepository.GetAllReference<Book>(book.Id).First();
            var result = BookVersionRepository.GetAllReference<Book>(book.Id);

            Assert.AreEqual(version, result.First());
        }

        [Test]
        public void GetAll_NotCachedObject_ShouldReturnInstanceFromDb()
        {
            var book = new Book();
            BookRepository.Save(book);
            var version = new BookVersion { BookId = book.Id };
            NoCachedBookVersionRepository.Save(version);

            var result = BookVersionRepository.GetAll();

            Assert.AreNotSame(version, result.First());
            Assert.AreEqual(version, result.First());
        }

        [Test]
        public void GetAll_NotCachedObject_ShouldUpdateCache()
        {
            var book = new Book();
            BookRepository.Save(book);
            var version = new BookVersion { BookId = book.Id };
            NoCachedBookVersionRepository.Save(version);

            version = BookVersionRepository.GetAll().First();
            var result = BookVersionRepository.GetAll();

            Assert.AreEqual(version, result.First());
        }
    }
}
