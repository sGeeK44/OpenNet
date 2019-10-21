using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Repositories;

namespace OpenNet.Orm.UnitTests
{
    [Entity]
    public class ConcreteEntity : EntityBase<ConcreteEntity>
    {
        private IRepository<ConcreteEntity> _repository;

        public void SetRepository(IRepository<ConcreteEntity> repo)
        {
            _repository = repo;
        }

        public override IRepository<ConcreteEntity> Repository
        {
            get => _repository;
            set => _repository = value;
        }

        [Field]
        public string Name { get; set; }
    }
}