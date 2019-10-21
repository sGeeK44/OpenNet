using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace OpenNet.Orm.Sql.Schema
{
    public class ColumnDefinition : IEquatable<ColumnDefinition>
    {
        /// <summary>
        /// Column position (Index start at 0)
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Indicate if current culumn can contain null value
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Get column db type
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// Get column name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>Indique si l'objet actuel est égal à un autre objet du même type.</summary>
        /// <returns>true si l'objet en cours est égal au paramètre <paramref name="other" /> ; sinon, false.</returns>
        /// <param name="other">Objet à comparer avec cet objet.</param>
        public bool Equals(ColumnDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Ordinal == other.Ordinal && IsNullable == other.IsNullable && string.Equals(DbType, other.DbType) && string.Equals(ColumnName, other.ColumnName);
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
            return Equals((ColumnDefinition)obj);
        }

        /// <summary>Fait office de fonction de hachage par défaut. </summary>
        /// <returns>Code de hachage pour l'objet actuel.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ordinal;
                hashCode = (hashCode * 397) ^ IsNullable.GetHashCode();
                hashCode = (hashCode * 397) ^ (DbType != null ? DbType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ColumnName != null ? ColumnName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}