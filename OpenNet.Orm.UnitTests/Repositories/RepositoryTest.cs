using Moq;
using NUnit.Framework;

namespace OpenNet.Orm.UnitTests.Repositories
{
    [TestFixture]
    public class RepositoryTest
    {
        [Test]
        public void GetAllReference_ShouldReturnObjectList()
        {
            var repo = new RepositoryTester();
            const long idSearch = 1;

            repo.GetAllReference<object>(idSearch);

            repo.Mock.Verify(_ => _.GetAllReference<object>(idSearch), Times.Once());
        }
    }
}
