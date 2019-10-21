using System.Data;

namespace OpenNet.Orm.Sql
{
    public interface IDbEngine
    {
        /// <summary>
        /// Indicate if database exist
        /// </summary>
        bool DatabaseExists { get; }

        /// <summary>
        /// Get database's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Delete database
        /// </summary>
        void DeleteDatabase();

        /// <summary>
        /// Create a new database with current db settings
        /// </summary>
        void CreateDatabase();

        /// <summary>
        /// Create a new connection on current database
        /// </summary>
        /// <returns></returns>
        IDbConnection GetNewConnection();

        /// <summary>
        /// Compacts and shrinks database.
        /// </summary>
        void Compact();

        /// <summary>
        /// Repairs the given corrupted database trying to recover all possible rows.
        /// </summary>
        void RepairDb();
    }
}