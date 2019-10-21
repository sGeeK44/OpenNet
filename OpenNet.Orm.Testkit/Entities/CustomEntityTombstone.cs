using System;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Repositories;
using OpenNet.Orm.Sync.Entity;

namespace OpenNet.Orm.Testkit.Entities
{
    public class CustomEntityTombstone<TEntity, TIEntity> : EntityTombstone<TEntity, TIEntity>
        where TIEntity : IDistinctableEntity
    {
        public CustomEntityTombstone() { }

        public CustomEntityTombstone(IRepository<IEntityTombstone> entityTombstoneRepo, TIEntity deletedEntity, DateTime deathTime)
            : base(entityTombstoneRepo, deletedEntity, deathTime) { }

        [Field]
        public string CustomProp { get; set; }
    }
}