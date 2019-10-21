using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Moq;
using NUnit.Framework;
using OpenNet.Orm.Entity;
using OpenNet.Orm.Repositories;
using OpenNet.Orm.Sync.Agents;
using OpenNet.Orm.Sync.Changes;
using OpenNet.Orm.Sync.Conflicts;
using OpenNet.Orm.Sync.Entity;
using OpenNet.Orm.Testkit;
using OpenNet.Orm.Testkit.Entities;

namespace OpenNet.Orm.Sync.UnitTests.Agents
{
    [TestFixture]
    public class SyncAgentTest : SyncOneRemoteTest
    {
        protected override void Setup()
        {
            base.Setup();
            AddEntityType<EntitySync, IEntitySync>();
            SyncSession = new SyncSessionInfoMock();
        }

        public SyncSessionInfoMock SyncSession { get; set; }

        [TestCase]
        public void GetLocalChange_EmptyDataStore_ShouldGettingNothing()
        {
            var localChange = GetLocalChange();

            Assert.AreEqual(6, localChange.EntityChangeset.Count);
            Assert.AreEqual(0, localChange.EntityChangeset[0].Insert.Count);
            Assert.AreEqual(0, localChange.EntityChangeset[0].Update.Count);
            Assert.AreEqual(0, localChange.EntityChangeset[0].Delete.Count);
            Assert.AreEqual(0, localChange.EntityChangeset[1].Insert.Count);
            Assert.AreEqual(0, localChange.EntityChangeset[1].Update.Count);
            Assert.AreEqual(0, localChange.EntityChangeset[1].Delete.Count);
        }

        [TestCase]
        public void GetLocalChange_EmptyDataStore_EntityNameShouldSet()
        {
            var localChange = GetLocalChange();
            
            Assert.AreEqual("entity", localChange.EntityChangeset[0].EntityName);
            Assert.AreEqual("entity_tombstone", localChange.EntityChangeset[1].EntityName);
        }

        [TestCase]
        public void GetLocalChange_NewEntity_ShouldGettingIt()
        {
            var entity =  new EntitySync();
            Desktop.Save<EntitySync, IEntitySync>(entity);

            var localChange = GetLocalChange();

            var entityChangeset = localChange.EntityChangeset[0];
            Assert.AreEqual(1, entityChangeset.Insert.Count);
            var insertEntity = entityChangeset.Insert[0];
            AssertFieldCount(insertEntity);
            AssertField(insertEntity.Fields[0], EntityBase<object>.IdColumnName, entity.Id);
        }

        [TestCase]
        public void GetLocalChange_UpdateEntity_ShouldGettingIt()
        {
            SyncSession.LowBoundaryAnchor = SyncDateTimeProvider.UtcNow;

            var entity = new EntitySync();
            Desktop.Save<EntitySync, IEntitySync>(entity);
            SomeTimeLater();

            Desktop.Save<EntitySync, IEntitySync>(entity);
            SomeTimeLater();

            var localChange = GetLocalChange();

            var entityChangeset = localChange.EntityChangeset[0];
            Assert.AreEqual(1, entityChangeset.Update.Count);
            var updateEntity = entityChangeset.Update[0];
            AssertFieldCount(updateEntity);
            AssertField(updateEntity.Fields[0], EntitySync.IdColumnName, entity.Id);
        }

        private EntitiesChangeset GetLocalChange()
        {
            var syncAgent = new SyncAgentTester(Desktop.DataStore);
            SyncSession.HighBoundaryAnchor = SyncDateTimeProvider.UtcNow;
            syncAgent.SetSyncSession(SyncSession);
            syncAgent.CreateStatProvider();
            syncAgent.ComputeLocalChange();
            return syncAgent.LocalEntitiesChangeset;
        }

        [TestCase]
        public void GetLocalChange_DeleteEntity_ShouldGettingIt()
        {
            var entity = new EntitySync();
            Desktop.Save<EntitySync, IEntitySync>(entity);
            Desktop.Delete<EntitySync, IEntitySync>(entity);

            var localChange = GetLocalChange();

            var entityChangeset = localChange.EntityChangeset[0];
            Assert.AreEqual(1, entityChangeset.Delete.Count);
            Assert.AreEqual(EntitySync.TableName, entityChangeset.EntityName);
            var deleteEntity = entityChangeset.Delete[0];
            AssertTombstoneFieldCount(deleteEntity);
            AssertField(deleteEntity.Fields[0], EntitySync.ColumnNameCreatedAt, SyncDateTimeProvider.UtcNow);
            AssertField(deleteEntity.Fields[1], EntitySync.ColumnNameUpdatedAt, DBNull.Value);
            AssertField(deleteEntity.Fields[2], EntitySync.ColumnNameLastSyncAt, DBNull.Value);
            AssertField(deleteEntity.Fields[3], EntitySync.IdColumnName, entity.Id);
        }

        [TestCase]
        public void ApplyChanges_NoEntity_ShouldNotThrowException()
        {
            var localChange = new EntitiesChangeset(Desktop.DataStore, SyncSession);

            Assert.DoesNotThrow(() => localChange.ApplyChanges(Desktop.DataStore, new Mock<ISyncStatProvider>().Object, new Mock<IConflictsManager>().Object));
        }

        [TestCase]
        public void ApplyChanges_OneEntityNoChange_ShouldNotThrowException()
        {
            var localChange = new EntitiesChangeset(Desktop.DataStore, SyncSession);
            localChange.AddEntityChangeset(new EntityChangeset(EntitySync.TableName));

            Assert.DoesNotThrow(() => localChange.ApplyChanges(Desktop.DataStore, new Mock<ISyncStatProvider>().Object, new ResolveAllConflicts()));
        }

        [TestCase]
        public void ApplyChanges_RemoteInsertLocalNotExist_InsertInDataStoreWithSameId()
        {
            var entity = CreateEntity();

            var localChange = CreateInsertDataset(entity);

            ApplyChanges(localChange, new ResolveAllConflicts());

            AssertEntityAreEqual(Desktop.Repository<EntitySync, IEntitySync>(), entity);
        }

        [TestCase]
        public void ApplyChanges_RemoteInsertLocalExist_ShouldReturnConflict()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            var localChange = CreateInsertDataset(entity);

            var conflict = new ResolveAllConflicts();
            ApplyChanges(localChange, conflict);

            Assert.AreEqual(1, conflict.Count);
            AssertEntityAreEqual(Desktop.Repository<EntitySync, IEntitySync>(), entity);
        }

        [TestCase]
        public void ApplyChanges_RemoteUpdateLocalExistUnchanged_ShouldUpdateProperties()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();
            MadeChange(entity);

            var localChange = CreateUpdateDataSet(entity);
            
            ApplyChanges(localChange, new ResolveAllConflicts());

            AssertEntityAreEqual(Desktop.Repository<EntitySync, IEntitySync>(), entity);
        }

        [TestCase]
        public void ApplyChanges_RemoteUpdateLocalUpdate_ReturnConflict()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();

            MadeChange(entity);
            Desktop.Save<EntitySync, IEntitySync>(entity);
            var beforeSync = Desktop.GetById<EntitySync, IEntitySync>(entity.Id);
            AssertEntityAreEqual(Desktop.Repository<EntitySync, IEntitySync>(), beforeSync);

            MadeChange(entity);
            var localChange = CreateUpdateDataSet(entity);

            var conflict = new ResolveAllConflicts();
            ApplyChanges(localChange, conflict);

            Assert.AreEqual(1, conflict.Count);
            AssertEntityAreEqual(Desktop.Repository<EntitySync, IEntitySync>(), beforeSync);
        }

        [TestCase]
        public void ApplyChanges_RemoteDeleteLocalItemExistUnchanged_ShouldDeleteIt()
        {
            var entity = Desktop.InsertNewEntity<EntitySync, IEntitySync>();
            SomeTimeLater();

            SyncSession.LowBoundaryAnchor = SyncDateTimeProvider.UtcNow;
            var localChange = CreateDeleteDataSet(entity);

            ApplyChanges(localChange, new ResolveAllConflicts());

            var syncEntity = Desktop.GetById<EntitySync, IEntitySync>(entity.Id);

            Assert.IsNull(syncEntity);
        }

        public static EntitySync CreateEntity()
        {
            var entity = new EntitySync
            {
                Id = 2,
                CreatedAt = RoundToSqlDateTime(DateTime.UtcNow)
            };
            return entity;
        }
        
        public static DateTime RoundToSqlDateTime(DateTime datetime)
        {
            return new SqlDateTime(datetime).Value;
        }

        private void ApplyChanges(EntitiesChangeset localChange, IConflictsManager conflictsManager)
        {
            SyncSession.HighBoundaryAnchor = SyncDateTimeProvider.UtcNow;
            localChange.ApplyChanges(Desktop.DataStore, new Mock<ISyncStatProvider>().Object, conflictsManager);
        }

        private void MadeChange(EntitySync entity)
        {
            entity.UpdatedAt = SyncDateTimeProvider.UtcNow;
            if (entity.StringField == null)
                entity.StringField = string.Empty;

            entity.StringField += "Updated!";
        }

        private EntitiesChangeset CreateInsertDataset(IEntitySync entity)
        {
            var fields = CreateEntityFields(entity);

            var newEntity = new EntityChangeset(EntitySync.TableName)
            {
                Insert = new List<EntityChange> { fields }
            };
            newEntity.SetSyncSession(SyncSession);

            return CreateEntitiesChangeset(newEntity);
        }

        private EntitiesChangeset CreateUpdateDataSet(IEntitySync entity)
        {
            var fields = CreateEntityFields(entity);

            var newEntity = new EntityChangeset(EntitySync.TableName)
            {
                Update = new List<EntityChange> {fields}
            };
            newEntity.SetSyncSession(SyncSession);

            return CreateEntitiesChangeset(newEntity);
        }

        private EntitiesChangeset CreateDeleteDataSet(IEntitySync entity)
        {
            var fields = CreateTombstoneEntityFields(entity);

            var newEntity = new EntityChangeset(EntitySync.TableName)
            {
                Delete = new List<EntityChange> {fields}
            };
            newEntity.SetSyncSession(SyncSession);

            return CreateEntitiesChangeset(newEntity);
        }

        private static EntityChange CreateEntityFields(IEntitySync entity)
        {
            var fields = new EntityChange();
            fields.EntityName = EntitySync.TableName;
            fields.HasAutoIncrement = true;
            fields.Add(EntitySync.IdColumnName, entity.Id);
            fields.Add(EntitySync.ColumnNameCreatedAt, entity.CreatedAt);
            fields.Add(EntitySync.ColumnNameUpdatedAt, entity.UpdatedAt);
            fields.Add(EntitySync.ColumnNameLastSyncAt, entity.LastSyncAt);
            fields.Add(EntitySync.ColumnNameUniqueIdentifier, entity.UniqueIdentifier);
            fields.Add(EntitySync.ColumnNameStringField, entity.StringField);
            return fields;
        }

        private static EntityChange CreateTombstoneEntityFields(IEntitySync entity)
        {
            var fields = new EntityChange();
            fields.Add(EntityTombstone<EntitySync, IEntitySync>.IdColumnName, entity.Id);
            return fields;
        }

        private EntitiesChangeset CreateEntitiesChangeset(EntityChangeset newEntity)
        {
            var localChange = new EntitiesChangeset(Desktop.DataStore, SyncSession);
            localChange.AddEntityChangeset(newEntity);
            return localChange;
        }

        private static void AssertFieldCount(EntityChange insertEntity)
        {
            Assert.AreEqual(EntityBase<IEntitySync>.GetDbFields<EntitySync>().Count, insertEntity.Fields.Count);
        }

        private static void AssertTombstoneFieldCount(EntityChange insertEntity)
        {
            Assert.AreEqual(EntityBase<IEntitySync>.GetDbFields<EntityTombstone<EntitySync, IEntitySync>>().Count, insertEntity.Fields.Count);
        }

        private void AssertField(EntityField item, string fieldName, object value)
        {
            Assert.AreEqual(fieldName, item.Key);
            Assert.AreEqual(value, item.GetFieldValue());
        }

        public void AssertEntityAreEqual(IRepository<IEntitySync> repository, IEntitySync entity)
        {
            var syncEntity = repository.GetById(entity.Id);
            Assert.AreEqual(syncEntity.Id, entity.Id);
            Assert.AreEqual(syncEntity.CreatedAt, entity.CreatedAt);
            Assert.AreEqual(syncEntity.UpdatedAt, entity.UpdatedAt);
            Assert.AreEqual(syncEntity.UniqueIdentifier, entity.UniqueIdentifier);
            Assert.AreEqual(syncEntity.StringField, entity.StringField);
        }
    }
}
