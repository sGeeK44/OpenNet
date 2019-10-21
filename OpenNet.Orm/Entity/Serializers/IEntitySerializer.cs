using System.Data;
using OpenNet.Orm.Caches;

namespace OpenNet.Orm.Entity.Serializers
{
    /// <summary>
    /// Provider methods to convert and convert back object with database format
    /// </summary>
    public interface IEntitySerializer
    {
        /// <summary>
        /// Indicate if current serializer should use full field name data column access. (Depending on select used)
        /// </summary>
        bool UseFullName { get; set; }

        /// <summary>
        /// Get or set Entity cache to use for checking existing instance before made convert back
        /// </summary>
        IEntityCache EntityCache { get; set; }

        /// <summary>
        /// Convert specified database result into specified intance type
        /// </summary>
        /// <param name="dbResult">DataReader get from select</param>
        /// <returns>Instance initialized</returns>
        object Deserialize(IDataRecord dbResult);

        /// <summary>
        /// Initialize specified object item with data record value
        /// </summary>
        /// <param name="item">Object to fill</param>
        /// <param name="dbResult">DataReader get from select</param>
        void PopulateFields(object item, IDataRecord dbResult);
    }
}