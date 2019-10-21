using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenNet.Orm.Attributes;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity.References;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

// ReSharper disable ArrangeAccessorOwnerBody

namespace OpenNet.Orm.Sql.Queries
{
    public class Select<TIEntity> : IEntityBuilder<TIEntity>, ISelectable
        where TIEntity : class
    {
        private readonly Selectable<TIEntity> _selectable;
        private readonly Dictionary<KeyValuePair<object, Reference>, IList> _listToSet = new Dictionary<KeyValuePair<object, Reference>, IList>();

        public Select(Selectable<TIEntity> selectable)
        {
            _selectable = selectable;
        }

        public int Offset { get; set; }
        public IEntityCache EntityCache { get; set; }

        protected virtual string SelectVerb
        {
            get { return "SELECT "; }
        }

        public TIEntity Deserialize(IDataReader results)
        {
            var serializer = _selectable.Entity.GetSerializer();
            serializer.UseFullName = true;
            serializer.EntityCache = EntityCache;
            var item = serializer.Deserialize(results);
            if (item == null)
                return default(TIEntity);

            var deserializedItems = new Dictionary<Type, object> {{item.GetType(), item}};
            foreach (var join in _selectable.JoinList)
            {
                var entity = _selectable.Entities[join.EntityType1];
                if (entity == null)
                    continue;

                var referenceToFill = entity.GetReference(join.EntityType2);
                if (referenceToFill == null)
                    continue;

                var refSerializer = referenceToFill.GetReferenceSerializer();
                refSerializer.UseFullName = serializer.UseFullName;
                refSerializer.EntityCache = serializer.EntityCache;

                var refenceValue = refSerializer.Deserialize(results);
                if (refenceValue == null)
                    continue;

                if (!deserializedItems.ContainsKey(join.EntityType2))
                    deserializedItems.Add(join.EntityType2, refenceValue);

                if (referenceToFill.ReferenceType == ReferenceType.OneToMany)
                {
                    var hash = new KeyValuePair<object, Reference>(deserializedItems[join.EntityType1], referenceToFill);
                    if (_listToSet.ContainsKey(hash))
                        _listToSet[hash].Add(refenceValue);
                    else
                    {
                        var list = referenceToFill.CreateValue();
                        list.Add(refenceValue);
                        _listToSet.Add(hash, list);
                    }
                }
                else
                    referenceToFill.SetEntityValue(deserializedItems[join.EntityType1], refenceValue);
            }

            foreach (var list in _listToSet)
            {
                var hash = list.Key;
                if (hash.Key == item)
                    hash.Value.SetEntityValue(item, list.Value);
            }

            return (TIEntity)item;
        }

        public string SelectStatement()
        {
            var result = new StringBuilder(SelectVerb);
            var entityInvolves = _selectable.EntityInvolve.Values.ToList();
            for (var index = 0; index < entityInvolves.Count; index++)
            {
                var entity = entityInvolves[index];
                result.Append(GetSelectFieldList(entity, index == 0));
            }
            return result.ToString();
        }

        private static string GetSelectFieldList(IEntityInfo entity, bool isFirstEntity)
        {
            var result = new StringBuilder();
            for (var i = 0; i < entity.Fields.Count; i++)
            {
                var field = entity.Fields[i];
                if (!isFirstEntity || i != 0) result.Append(", ");
                result.Append(field.FullFieldName + " AS " + field.AliasFieldName);
            }
            return result.ToString();
        }
    }
}