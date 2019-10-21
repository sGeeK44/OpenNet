using System.Collections.Generic;
using Moq;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.UnitTests
{
    public class RepositoryTester : Repository<ConcreteEntity, ConcreteEntity>
    {
        public Mock<IRepository<ConcreteEntity>> Mock { get; set; }

        public RepositoryTester()
        {
            Mock = new Mock<IRepository<ConcreteEntity>>();
        }

        public override List<ConcreteEntity> GetAllReference<TForeignEntity>(long id)
        {
            return Mock.Object.GetAllReference<TForeignEntity>(id);
        }
    }
}