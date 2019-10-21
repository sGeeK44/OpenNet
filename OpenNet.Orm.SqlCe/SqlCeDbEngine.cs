using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using OpenNet.Orm.Sql;

// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.SqlCe
{
    public class SqlCeDbEngine : IDbEngine
    {
        private const int MaxSize = 256;
        private readonly string _fileName;
        private readonly string _connectionString;

        public SqlCeDbEngine(string fileName, string password)
        {
            _fileName = fileName;
            _connectionString = string.Format("Data Source={0};Persist Security Info=False;Max Database Size={1};", _fileName, MaxSize);

            if (!string.IsNullOrEmpty(password))
            {
                _connectionString += string.Format("Password={0};", password);
            }
        }

        public bool DatabaseExists
        {
            get { return File.Exists(_fileName); }
        }

        public string Name
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Delete database
        /// </summary>
        public void DeleteDatabase()
        {
            if (!File.Exists(_fileName))
                return;

            File.Delete(_fileName);
        }

        /// <summary>
        /// Create a new database with current db settings
        /// </summary>
        public void CreateDatabase()
        {
            if (DatabaseExists)
            {
                throw new StoreAlreadyExistsException();
            }

            // create the file
            using (var engine = new SqlCeEngine(_connectionString))
            {
                engine.CreateDatabase();
            }
        }

        public IDbConnection GetNewConnection()
        {
            return new SqlCeConnection(_connectionString);
        }

        /// <summary>
        /// Compacts and shrinks the given database.
        /// </summary>
        public void Compact()
        {
            using (var eng = new SqlCeEngine(_connectionString))
            {
                eng.Compact(_connectionString);
                eng.Shrink();
            }
        }

        /// <summary>
        /// Repairs the given corrupted database trying to recover all possible rows.
        /// </summary>
        public void RepairDb()
        {
            using (var eng = new SqlCeEngine(_connectionString))
            {
#if WindowsCE
                eng.Repair(null, RepairOption.RecoverCorruptedRows);
#else
                eng.Repair(null, RepairOption.RecoverAllPossibleRows);
#endif
            }
        }
    }
}