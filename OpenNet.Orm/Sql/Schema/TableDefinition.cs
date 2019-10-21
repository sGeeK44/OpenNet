using System;
using System.Collections.Generic;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace OpenNet.Orm.Sql.Schema
{
    public class TableDefinition : IEquatable<TableDefinition>
    {
        private readonly List<ColumnDefinition> _columnFormatList = new List<ColumnDefinition>();

        public TableDefinition(string entityName)
        {
            EntityName = entityName;
        }

        /// <summary>
        /// Get table name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Add new column format
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="ordinal">Column position (Index start at 0)</param>
        /// <param name="isNullable">Indicate if current culumn can contain null value</param>
        /// <param name="dbType">Column db type</param>
        public void AddColumn(string columnName, int ordinal, bool isNullable, string dbType)
        {
            AddColumn(
                new ColumnDefinition
                {
                    ColumnName = columnName,
                    Ordinal = ordinal,
                    IsNullable = isNullable,
                    DbType = dbType
                }
            );
        }

        /// <summary>
        /// Add new column format
        /// </summary>
        /// <param name="columnFormat">Column definition to add</param>
        public void AddColumn(ColumnDefinition columnFormat)
        {
            _columnFormatList.Add(columnFormat);
        }

        /// <summary>Indique si l'objet actuel est égal à un autre objet du même type.</summary>
        /// <returns>true si l'objet en cours est égal au paramètre <paramref name="other" /> ; sinon, false.</returns>
        /// <param name="other">Objet à comparer avec cet objet.</param>
        public bool Equals(TableDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._columnFormatList.IsEquals(_columnFormatList) && string.Equals(EntityName, other.EntityName);
        }

        /// <summary>Détermine si l'objet spécifié est identique à l'objet actuel.</summary>
        /// <returns>true si l'objet spécifié est égal à l'objet actuel ; sinon, false.</returns>
        /// <param name="obj">Objet à comparer à l'objet actuel. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TableDefinition)obj);
        }

        /// <summary>Fait office de fonction de hachage par défaut. </summary>
        /// <returns>Code de hachage pour l'objet actuel.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_columnFormatList != null ? _columnFormatList.GetHashCode() : 0) * 397) ^ (EntityName != null ? EntityName.GetHashCode() : 0);
            }
        }
    }
}